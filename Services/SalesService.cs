using CafeManagement.Data;
using CafeManagement.Models.Entities;
using CafeManagement.Models.Enums;
using CafeManagement.Models.ViewModels;
using CafeManagement.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CafeManagement.Services;

public class SalesService : ISalesService
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<SalesService> _logger;

    public SalesService(ApplicationDbContext db, ILogger<SalesService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<List<SalesProductItemViewModel>> GetMenuAsync(string? keyword, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("GetMenuAsync called with keyword={Keyword}", keyword);
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
        _logger.LogDebug("GetProductAsync called for productId={ProductId}", productId);
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
        var strategy = _db.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                _logger.LogInformation("Checkout started by {User} with {ItemCount} items", currentUserEmail ?? "(anonymous)", request?.Items?.Count ?? 0);

                var employeeId = await EnsureEmployeeAsync(currentUserEmail, cancellationToken);
                _logger.LogDebug("Resolved employeeId={EmployeeId} for user={User}", employeeId, currentUserEmail);

                var customerId = await EnsureCustomerAsync(request.KhachhangId, cancellationToken);
                _logger.LogDebug("Resolved customerId={CustomerId}", customerId);

                var itemIds = request.Items.Select(x => x.SanphamId).Distinct().ToList();
                var products = await _db.Sanphams
                    .Where(x => itemIds.Contains(x.Id) && x.IsActive)
                    .ToDictionaryAsync(x => x.Id, cancellationToken);

                foreach (var line in request.Items)
                {
                    if (!products.TryGetValue(line.SanphamId, out var p))
                    {
                        _logger.LogWarning("Product {ProductId} not found or inactive", line.SanphamId);
                        throw new InvalidOperationException("San pham khong ton tai hoac da ngung kinh doanh");
                    }

                    if (line.SoLuong <= 0)
                    {
                        _logger.LogWarning("Invalid quantity {Qty} for product {ProductId}", line.SoLuong, line.SanphamId);
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

                _logger.LogDebug("Found {RecipeCount} recipe rows for items", recipeRows.Count);

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

                _logger.LogDebug("Loaded {StockCount} stock records for required stocks", stocks.Count);

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

                _logger.LogInformation("Creating Hoadon {MaHD} by user {User}", maHD, currentUserEmail ?? "(anonymous)");

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
                    _logger.LogDebug("Deducting {Qty} from stock {StockId} (before={BeforeQty})", req.Value, stock.Id, stock.SoLuongTon);
                    stock.SoLuongTon -= req.Value;
                    stock.NgayCapNhat = now;
                    _logger.LogDebug("Stock {StockId} now has {AfterQty}", stock.Id, stock.SoLuongTon);
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

                _logger.LogInformation("Saving changes for invoice {MaHD}", maHD);
                await _db.SaveChangesAsync(cancellationToken);
                await tx.CommitAsync(cancellationToken);
                _logger.LogInformation("Checkout committed: {MaHD}", maHD);

                return new SalesCheckoutResultViewModel
                {
                    HoadonId = hoadon.Id,
                    MaHD = hoadon.MaHD,
                    ThanhTien = hoadon.ThanhTien,
                    GiamGia = hoadon.GiamGia ?? 0,
                    TongCong = hoadon.TongCong
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Checkout failed for user {User}", currentUserEmail);
                await tx.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }

    public async Task<List<SalesInvoiceHistoryItemViewModel>> GetInvoiceHistoryAsync(int take = 50, CancellationToken cancellationToken = default)
    {
        var pageSize = Math.Clamp(take, 1, 200);
        _logger.LogDebug("GetInvoiceHistoryAsync called with take={Take}", pageSize);

        return await _db.Hoadons
            .AsNoTracking()
            .OrderByDescending(x => x.NgayLapHD)
            .Take(pageSize)
            .Select(x => new SalesInvoiceHistoryItemViewModel
            {
                Id = x.Id,
                MaHD = x.MaHD,
                TenKhach = x.Khachhang.TenKH,
                ThuNgan = x.Nhanvien.TenNV,
                NgayLap = x.NgayLapHD,
                ThanhTien = x.ThanhTien,
                GiamGia = x.GiamGia ?? 0,
                TongCong = x.TongCong,
                SoMon = x.HoadonCTs.Count
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<SalesPrintViewModel?> GetPrintDataAsync(Guid hoadonId, CancellationToken cancellationToken = default)
    {
        return await _db.Hoadons
            .AsNoTracking()
            .Where(x => x.Id == hoadonId)
            .Select(x => new SalesPrintViewModel
            {
                HoadonId = x.Id,
                MaHD = x.MaHD,
                TenKhach = x.Khachhang.TenKH,
                ThuNgan = x.Nhanvien.TenNV,
                NgayLap = x.NgayLapHD,
                ThanhTien = x.ThanhTien,
                GiamGia = x.GiamGia ?? 0,
                TongCong = x.TongCong,
                Lines = x.HoadonCTs.Select(ct => new SalesPrintLineViewModel
                {
                    TenSP = ct.Sanpham.TenSP,
                    SoLuong = ct.SoLuong,
                    DonGia = ct.DonGia,
                    ThanhTien = ct.ThanhTien
                }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<SalesInvoiceHistoryItemViewModel>> GetInvoiceHistoryFilteredAsync(DateTime? fromDate, DateTime? toDate, int take = 50, CancellationToken cancellationToken = default)
    {
        var pageSize = Math.Clamp(take, 1, 200);
        _logger.LogDebug("GetInvoiceHistoryFilteredAsync called with fromDate={FromDate}, toDate={ToDate}, take={Take}", fromDate, toDate, pageSize);

        var query = _db.Hoadons.AsNoTracking();

        if (fromDate.HasValue)
        {
            query = query.Where(x => x.NgayLapHD >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            // Include the entire end date
            var endOfDay = toDate.Value.AddDays(1);
            query = query.Where(x => x.NgayLapHD < endOfDay);
        }

        return await query
            .OrderByDescending(x => x.NgayLapHD)
            .Take(pageSize)
            .Select(x => new SalesInvoiceHistoryItemViewModel
            {
                Id = x.Id,
                MaHD = x.MaHD,
                TenKhach = x.Khachhang.TenKH,
                ThuNgan = x.Nhanvien.TenNV,
                NgayLap = x.NgayLapHD,
                ThanhTien = x.ThanhTien,
                GiamGia = x.GiamGia ?? 0,
                TongCong = x.TongCong,
                SoMon = x.HoadonCTs.Count
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<SalesInvoiceDetailViewModel?> GetInvoiceDetailsAsync(Guid hoadonId, CancellationToken cancellationToken = default)
    {
        return await _db.Hoadons
            .AsNoTracking()
            .Where(x => x.Id == hoadonId)
            .Select(x => new SalesInvoiceDetailViewModel
            {
                HoadonId = x.Id,
                MaHD = x.MaHD,
                TenKhach = x.Khachhang.TenKH,
                ThuNgan = x.Nhanvien.TenNV,
                NgayLap = x.NgayLapHD,
                ThanhTien = x.ThanhTien,
                GiamGia = x.GiamGia ?? 0,
                TongCong = x.TongCong,
                Lines = x.HoadonCTs.Select(ct => new SalesInvoiceDetailLineViewModel
                {
                    TenSP = ct.Sanpham.TenSP,
                    SoLuong = ct.SoLuong,
                    DonGia = ct.DonGia,
                    ThanhTien = ct.ThanhTien
                }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    private async Task<Guid> EnsureCustomerAsync(Guid? customerId, CancellationToken cancellationToken)
    {
        if (customerId.HasValue)
        {
            var exists = await _db.Khachhangs.AnyAsync(x => x.Id == customerId.Value, cancellationToken);
            if (exists)
            {
                _logger.LogDebug("Using provided customerId={CustomerId}", customerId.Value);
                return customerId.Value;
            }
        }

        var walkin = await _db.Khachhangs.FirstOrDefaultAsync(x => x.TenKH == "Khach le", cancellationToken);
        if (walkin is not null)
        {
            _logger.LogDebug("Found default walk-in customer {CustomerId}", walkin.Id);
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
        _logger.LogInformation("Created default walk-in customer {CustomerId}", walkin.Id);
        return walkin.Id;
    }

    private async Task<Guid> EnsureEmployeeAsync(string? currentUserEmail, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(currentUserEmail))
        {
            var byEmail = await _db.Nhanviens.FirstOrDefaultAsync(x => x.Email == currentUserEmail, cancellationToken);
            if (byEmail is not null)
            {
                _logger.LogDebug("Found employee by email {Email} -> {EmployeeId}", currentUserEmail, byEmail.Id);
                return byEmail.Id;
            }
        }

        var employee = await _db.Nhanviens.FirstOrDefaultAsync(cancellationToken);
        if (employee is not null)
        {
            _logger.LogDebug("Using existing employee {EmployeeId}", employee.Id);
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
        _logger.LogInformation("Created default employee {EmployeeId} (email={Email})", employee.Id, currentUserEmail);
        return employee.Id;
    }

    public async Task<List<DailySalesReportViewModel>> GetDailySalesReportAsync(DateTime? fromDate, DateTime? toDate, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("GetDailySalesReportAsync called with fromDate={FromDate}, toDate={ToDate}", fromDate, toDate);

        var query = _db.Hoadons.AsNoTracking();

        if (fromDate.HasValue)
        {
            query = query.Where(x => x.NgayLapHD >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            var endOfDay = toDate.Value.AddDays(1);
            query = query.Where(x => x.NgayLapHD < endOfDay);
        }

        var report = await query
            .Include(x => x.HoadonCTs)
            .ThenInclude(x => x.Sanpham)
            .GroupBy(x => x.NgayLapHD.Date)
            .OrderByDescending(x => x.Key)
            .Select(g => new DailySalesReportViewModel
            {
                Ngay = g.Key,
                TongSoHD = g.Count(),
                TongThanhTien = g.Sum(x => x.ThanhTien),
                TongGiamGia = g.Sum(x => x.GiamGia ?? 0),
                TongCong = g.Sum(x => x.TongCong),
                Products = g.SelectMany(x => x.HoadonCTs)
                    .GroupBy(x => x.Sanpham.TenSP)
                    .Select(pg => new DailySalesProductLineViewModel
                    {
                        TenSP = pg.Key,
                        SoLuong = pg.Sum(x => x.SoLuong),
                        DonGia = pg.First().DonGia,
                        ThanhTien = pg.Sum(x => x.ThanhTien)
                    }).ToList()
            })
            .ToListAsync(cancellationToken);

        return report;
    }
    }

