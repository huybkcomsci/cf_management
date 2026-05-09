using CafeManagement.Data;
using CafeManagement.Models.Entities;
using CafeManagement.Models.Enums;
using CafeManagement.Models.ViewModels;
using CafeManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Services;

public class SalesService : ISalesService
{
    private readonly ApplicationDbContext _db;

    public SalesService(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<List<SalesProductItemViewModel>> GetMenuAsync(string? keyword, CancellationToken cancellationToken = default)
    {
        var query = _db.Sanphams
            .AsNoTracking()
            .Include(x => x.NhomSP)
            .Where(x => x.IsActive);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(x => x.TenSP.Contains(keyword) || (x.MoTa != null && x.MoTa.Contains(keyword)));
        }

        return await query
            .OrderBy(x => x.TenSP)
            .Select(x => new SalesProductItemViewModel
            {
                Id = x.Id,
                TenSP = x.TenSP,
                TenNhom = x.NhomSP.TenNhom,
                GiaBan = x.GiaBan,
                SoLuongTon = x.SoLuongTon,
                DonViTinh = x.DonViTinh
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<SalesProductItemViewModel?> GetProductAsync(Guid productId, CancellationToken cancellationToken = default)
    {
        return await _db.Sanphams
            .AsNoTracking()
            .Include(x => x.NhomSP)
            .Where(x => x.Id == productId && x.IsActive)
            .Select(x => new SalesProductItemViewModel
            {
                Id = x.Id,
                TenSP = x.TenSP,
                TenNhom = x.NhomSP.TenNhom,
                GiaBan = x.GiaBan,
                SoLuongTon = x.SoLuongTon,
                DonViTinh = x.DonViTinh
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<SalesCheckoutResultViewModel> CheckoutAsync(SalesCheckoutRequestViewModel request, string? currentUserEmail, CancellationToken cancellationToken = default)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var employeeId = await EnsureEmployeeAsync(currentUserEmail, cancellationToken);
            var customerId = await EnsureCustomerAsync(request.KhachhangId, cancellationToken);

            var itemIds = request.Items.Select(x => x.SanphamId).Distinct().ToList();
            var products = await _db.Sanphams
                .Where(x => itemIds.Contains(x.Id) && x.IsActive)
                .ToDictionaryAsync(x => x.Id, cancellationToken);

            foreach (var line in request.Items)
            {
                if (!products.TryGetValue(line.SanphamId, out var p))
                {
                    throw new InvalidOperationException("San pham khong ton tai hoac da ngung kinh doanh");
                }

                if (line.SoLuong <= 0)
                {
                    throw new InvalidOperationException("So luong khong hop le");
                }
            }

            // Optimized ingredient requirement aggregation:
            // - If product has recipe (Dinhluong), consume ingredients by formula.
            // - Otherwise consume product stock itself.
            var recipeRows = await _db.Dinhluongs
                .AsNoTracking()
                .Where(x => itemIds.Contains(x.IdSP))
                .ToListAsync(cancellationToken);

            var recipeMap = recipeRows
                .GroupBy(x => x.IdSP)
                .ToDictionary(g => g.Key, g => g.ToList());

            var requiredStocks = new Dictionary<Guid, int>();
            foreach (var line in request.Items)
            {
                if (recipeMap.TryGetValue(line.SanphamId, out var recipe) && recipe.Count > 0)
                {
                    foreach (var r in recipe)
                    {
                        var required = r.SoLuong * line.SoLuong;
                        if (!requiredStocks.TryAdd(r.IdThanhPhan, required))
                        {
                            requiredStocks[r.IdThanhPhan] += required;
                        }
                    }
                }
                else
                {
                    if (!requiredStocks.TryAdd(line.SanphamId, line.SoLuong))
                    {
                        requiredStocks[line.SanphamId] += line.SoLuong;
                    }
                }
            }

            var stockIds = requiredStocks.Keys.ToList();
            var stocks = await _db.Sanphams
                .Where(x => stockIds.Contains(x.Id) && x.IsActive)
                .ToDictionaryAsync(x => x.Id, cancellationToken);

            foreach (var req in requiredStocks)
            {
                if (!stocks.TryGetValue(req.Key, out var stockItem))
                {
                    throw new InvalidOperationException("Khong tim thay nguyen lieu trong kho");
                }

                if (stockItem.SoLuongTon < req.Value)
                {
                    throw new InvalidOperationException($"Khong du ton kho cho {stockItem.TenSP}. Can {req.Value}, con {stockItem.SoLuongTon}");
                }
            }

            var now = DateTime.UtcNow;
            var maHD = $"HD-{now:yyyyMMddHHmmss}-{Random.Shared.Next(100, 999)}";

            var details = new List<HoadonCT>();
            decimal thanhTien = 0m;

            foreach (var line in request.Items)
            {
                var p = products[line.SanphamId];
                var lineAmount = p.GiaBan * line.SoLuong;
                thanhTien += lineAmount;

                details.Add(new HoadonCT
                {
                    Id = Guid.NewGuid(),
                    IdSP = p.Id,
                    SoLuong = line.SoLuong,
                    DonGia = p.GiaBan,
                    ThanhTien = lineAmount
                });

                // Stock deduction handled in aggregated ingredient loop below.
            }

            foreach (var req in requiredStocks)
            {
                var stock = stocks[req.Key];
                stock.SoLuongTon -= req.Value;
                stock.NgayCapNhat = now;
            }

            var giamGia = request.GiamGia < 0 ? 0 : request.GiamGia;
            var tongCong = Math.Max(0, thanhTien - giamGia);

            var hoadon = new Hoadon
            {
                Id = Guid.NewGuid(),
                MaHD = maHD,
                IdKH = customerId,
                IdNV = employeeId,
                NgayLapHD = now,
                ThanhTien = thanhTien,
                GiamGia = giamGia,
                TongCong = tongCong,
                PhuongThucThanhToan = PhuongThucThanhToan.TienMat,
                TrangThai = TrangThai.HoanThanh,
                GhiChu = request.GhiChu,
                NgayTao = now,
                NgayCapNhat = now,
                HoadonCTs = details
            };

            await _db.Hoadons.AddAsync(hoadon, cancellationToken);

            // Tạo bản ghi tiêu hao theo nguyên liệu thực tế.
            foreach (var d in details)
            {
                if (recipeMap.TryGetValue(d.IdSP, out var recipe) && recipe.Count > 0)
                {
                    foreach (var r in recipe)
                    {
                        var ingredient = stocks[r.IdThanhPhan];
                        await _db.Tieuhaos.AddAsync(new Tieuhao
                        {
                            Id = Guid.NewGuid(),
                            IdSP = r.IdThanhPhan,
                            IdHoadonCT = d.Id,
                            SoLuong = r.SoLuong * d.SoLuong,
                            DonGiaVon = ingredient.GiaNhap,
                            NgayTao = now
                        }, cancellationToken);
                    }
                }
                else
                {
                    var p = products[d.IdSP];
                    await _db.Tieuhaos.AddAsync(new Tieuhao
                    {
                        Id = Guid.NewGuid(),
                        IdSP = d.IdSP,
                        IdHoadonCT = d.Id,
                        SoLuong = d.SoLuong,
                        DonGiaVon = p.GiaNhap,
                        NgayTao = now
                    }, cancellationToken);
                }
            }

            await _db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);

            return new SalesCheckoutResultViewModel
            {
                HoadonId = hoadon.Id,
                MaHD = hoadon.MaHD,
                ThanhTien = hoadon.ThanhTien,
                GiamGia = hoadon.GiamGia ?? 0,
                TongCong = hoadon.TongCong
            };
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<SalesPrintViewModel?> GetPrintDataAsync(Guid hoadonId, CancellationToken cancellationToken = default)
    {
        var data = await _db.Hoadons
            .AsNoTracking()
            .Include(x => x.Khachhang)
            .Include(x => x.Nhanvien)
            .Include(x => x.HoadonCTs)
                .ThenInclude(x => x.Sanpham)
            .FirstOrDefaultAsync(x => x.Id == hoadonId, cancellationToken);

        if (data is null)
        {
            return null;
        }

        return new SalesPrintViewModel
        {
            HoadonId = data.Id,
            MaHD = data.MaHD,
            TenKhach = data.Khachhang.TenKH,
            ThuNgan = data.Nhanvien.TenNV,
            NgayLap = data.NgayLapHD,
            ThanhTien = data.ThanhTien,
            GiamGia = data.GiamGia ?? 0,
            TongCong = data.TongCong,
            Lines = data.HoadonCTs.Select(x => new SalesPrintLineViewModel
            {
                TenSP = x.Sanpham.TenSP,
                SoLuong = x.SoLuong,
                DonGia = x.DonGia,
                ThanhTien = x.ThanhTien
            }).ToList()
        };
    }

    private async Task<Guid> EnsureCustomerAsync(Guid? customerId, CancellationToken cancellationToken)
    {
        if (customerId.HasValue)
        {
            var exists = await _db.Khachhangs.AnyAsync(x => x.Id == customerId.Value, cancellationToken);
            if (exists)
            {
                return customerId.Value;
            }
        }

        var walkin = await _db.Khachhangs.FirstOrDefaultAsync(x => x.TenKH == "Khach le", cancellationToken);
        if (walkin is not null)
        {
            return walkin.Id;
        }

        walkin = new Khachhang
        {
            Id = Guid.NewGuid(),
            TenKH = "Khach le",
            Sdt = null,
            Email = null,
            DiaChi = null,
            CongNo = 0,
            IsActive = true,
            NgayTao = DateTime.UtcNow,
            NgayCapNhat = DateTime.UtcNow
        };

        await _db.Khachhangs.AddAsync(walkin, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return walkin.Id;
    }

    private async Task<Guid> EnsureEmployeeAsync(string? currentUserEmail, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(currentUserEmail))
        {
            var byEmail = await _db.Nhanviens.FirstOrDefaultAsync(x => x.Email == currentUserEmail, cancellationToken);
            if (byEmail is not null)
            {
                return byEmail.Id;
            }
        }

        var employee = await _db.Nhanviens.FirstOrDefaultAsync(cancellationToken);
        if (employee is not null)
        {
            return employee.Id;
        }

        employee = new Nhanvien
        {
            Id = Guid.NewGuid(),
            TenNV = "Thu ngan mac dinh",
            Email = currentUserEmail,
            ChucVu = "Thu ngan",
            TrangThai = TrangThaiNhanvien.DangLam,
            NgayVaoLam = DateTime.UtcNow,
            Luong = 0,
            NgayTao = DateTime.UtcNow,
            NgayCapNhat = DateTime.UtcNow
        };

        await _db.Nhanviens.AddAsync(employee, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
        return employee.Id;
    }
}
