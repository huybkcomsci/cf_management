using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CafeManagement.Migrations
{
    /// <inheritdoc />
    public partial class SimpleUserAuthentication : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Khachhang",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenKH = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Sdt = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DiaChi = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CongNo = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "timestamp", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Khachhang", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Nhacungcap",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenNCC = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Sdt = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DiaChi = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    NguoiDaiDien = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    TaxID = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CongNo = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "timestamp", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nhacungcap", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Nhanvien",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenNV = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Sdt = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DiaChi = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CMND = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    NgaySinh = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ChucVu = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Luong = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    TrangThai = table.Column<int>(type: "integer", nullable: false),
                    NgayVaoLam = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NgayThaiPhuc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "timestamp", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nhanvien", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NhomSP",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TenNhom = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MoTa = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PhanLoai = table.Column<int>(type: "integer", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "timestamp", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NhomSP", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Price = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bangluong",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdNV = table.Column<Guid>(type: "uuid", nullable: false),
                    Nam = table.Column<int>(type: "integer", nullable: false),
                    Thang = table.Column<int>(type: "integer", nullable: false),
                    SoGio = table.Column<int>(type: "integer", nullable: false),
                    Luong = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    PhuCap = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    KhauTru = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    TongLuong = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    TrangThai = table.Column<int>(type: "integer", nullable: false),
                    GhiChu = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    NgayTao = table.Column<DateTime>(type: "timestamp", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bangluong", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bangluong_Nhanvien_IdNV",
                        column: x => x.IdNV,
                        principalTable: "Nhanvien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Chamcong",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdNV = table.Column<Guid>(type: "uuid", nullable: false),
                    Ngay = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    GioVao = table.Column<TimeSpan>(type: "interval", nullable: true),
                    GioRa = table.Column<TimeSpan>(type: "interval", nullable: true),
                    TrangThai = table.Column<int>(type: "integer", nullable: false),
                    GhiChu = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    NgayTao = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chamcong", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Chamcong_Nhanvien_IdNV",
                        column: x => x.IdNV,
                        principalTable: "Nhanvien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Hoadon",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MaHD = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IdKH = table.Column<Guid>(type: "uuid", nullable: false),
                    IdNV = table.Column<Guid>(type: "uuid", nullable: false),
                    NgayLapHD = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ThanhTien = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    GiamGia = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: true),
                    TongCong = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    PhuongThucThanhToan = table.Column<int>(type: "integer", nullable: false),
                    TrangThai = table.Column<int>(type: "integer", nullable: false),
                    GhiChu = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    NgayTao = table.Column<DateTime>(type: "timestamp", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hoadon", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Hoadon_Khachhang_IdKH",
                        column: x => x.IdKH,
                        principalTable: "Khachhang",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Hoadon_Nhanvien_IdNV",
                        column: x => x.IdNV,
                        principalTable: "Nhanvien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Phieuchi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MaPC = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IdNV = table.Column<Guid>(type: "uuid", nullable: true),
                    IdNCC = table.Column<Guid>(type: "uuid", nullable: true),
                    NgayLapPC = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    SoTien = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    PhuongThucThanhToan = table.Column<int>(type: "integer", nullable: false),
                    LoaiChiPhi = table.Column<int>(type: "integer", nullable: false),
                    TrangThai = table.Column<int>(type: "integer", nullable: false),
                    GhiChu = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    NgayTao = table.Column<DateTime>(type: "timestamp", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phieuchi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Phieuchi_Nhacungcap_IdNCC",
                        column: x => x.IdNCC,
                        principalTable: "Nhacungcap",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Phieuchi_Nhanvien_IdNV",
                        column: x => x.IdNV,
                        principalTable: "Nhanvien",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Phieunhap",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MaPN = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IdNCC = table.Column<Guid>(type: "uuid", nullable: false),
                    IdNV = table.Column<Guid>(type: "uuid", nullable: false),
                    NgayLapPN = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TongTien = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    TrangThai = table.Column<int>(type: "integer", nullable: false),
                    GhiChu = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    NgayTao = table.Column<DateTime>(type: "timestamp", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phieunhap", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Phieunhap_Nhacungcap_IdNCC",
                        column: x => x.IdNCC,
                        principalTable: "Nhacungcap",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Phieunhap_Nhanvien_IdNV",
                        column: x => x.IdNV,
                        principalTable: "Nhanvien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sanpham",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdNhom = table.Column<Guid>(type: "uuid", nullable: false),
                    TenSP = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MoTa = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    GiaBan = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    GiaNhap = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    SoLuongTon = table.Column<int>(type: "integer", nullable: false),
                    SLTonMin = table.Column<int>(type: "integer", nullable: false),
                    DonViTinh = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    NgayTao = table.Column<DateTime>(type: "timestamp", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sanpham", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sanpham_NhomSP_IdNhom",
                        column: x => x.IdNhom,
                        principalTable: "NhomSP",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Dinhluong",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdSP = table.Column<Guid>(type: "uuid", nullable: false),
                    IdThanhPhan = table.Column<Guid>(type: "uuid", nullable: false),
                    SoLuong = table.Column<int>(type: "integer", nullable: false),
                    DonVi = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    GhiChu = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    NgayTao = table.Column<DateTime>(type: "timestamp", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "timestamp", nullable: false),
                    NhanvienId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dinhluong", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dinhluong_Nhanvien_NhanvienId",
                        column: x => x.NhanvienId,
                        principalTable: "Nhanvien",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Dinhluong_Sanpham_IdSP",
                        column: x => x.IdSP,
                        principalTable: "Sanpham",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Dinhluong_Sanpham_IdThanhPhan",
                        column: x => x.IdThanhPhan,
                        principalTable: "Sanpham",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Dongnhap",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdSP = table.Column<Guid>(type: "uuid", nullable: false),
                    IdPhieuNhap = table.Column<Guid>(type: "uuid", nullable: false),
                    SoLuong = table.Column<int>(type: "integer", nullable: false),
                    DonGia = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    GhiChu = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dongnhap", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Dongnhap_Phieunhap_IdPhieuNhap",
                        column: x => x.IdPhieuNhap,
                        principalTable: "Phieunhap",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Dongnhap_Sanpham_IdSP",
                        column: x => x.IdSP,
                        principalTable: "Sanpham",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HoadonCT",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdHD = table.Column<Guid>(type: "uuid", nullable: false),
                    IdSP = table.Column<Guid>(type: "uuid", nullable: false),
                    SoLuong = table.Column<int>(type: "integer", nullable: false),
                    DonGia = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    ThanhTien = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    GhiChu = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoadonCT", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HoadonCT_Hoadon_IdHD",
                        column: x => x.IdHD,
                        principalTable: "Hoadon",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HoadonCT_Sanpham_IdSP",
                        column: x => x.IdSP,
                        principalTable: "Sanpham",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tieuhao",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdSP = table.Column<Guid>(type: "uuid", nullable: false),
                    IdHoadonCT = table.Column<Guid>(type: "uuid", nullable: false),
                    SoLuong = table.Column<int>(type: "integer", nullable: false),
                    DonGiaVon = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    NgayTao = table.Column<DateTime>(type: "timestamp", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tieuhao", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tieuhao_HoadonCT_IdHoadonCT",
                        column: x => x.IdHoadonCT,
                        principalTable: "HoadonCT",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tieuhao_Sanpham_IdSP",
                        column: x => x.IdSP,
                        principalTable: "Sanpham",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bangluong_IdNV_Nam_Thang",
                table: "Bangluong",
                columns: new[] { "IdNV", "Nam", "Thang" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chamcong_IdNV_Ngay",
                table: "Chamcong",
                columns: new[] { "IdNV", "Ngay" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dinhluong_IdSP_IdThanhPhan",
                table: "Dinhluong",
                columns: new[] { "IdSP", "IdThanhPhan" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dinhluong_IdThanhPhan",
                table: "Dinhluong",
                column: "IdThanhPhan");

            migrationBuilder.CreateIndex(
                name: "IX_Dinhluong_NhanvienId",
                table: "Dinhluong",
                column: "NhanvienId");

            migrationBuilder.CreateIndex(
                name: "IX_Dongnhap_IdPhieuNhap",
                table: "Dongnhap",
                column: "IdPhieuNhap");

            migrationBuilder.CreateIndex(
                name: "IX_Dongnhap_IdSP",
                table: "Dongnhap",
                column: "IdSP");

            migrationBuilder.CreateIndex(
                name: "IX_Hoadon_IdKH",
                table: "Hoadon",
                column: "IdKH");

            migrationBuilder.CreateIndex(
                name: "IX_Hoadon_IdNV",
                table: "Hoadon",
                column: "IdNV");

            migrationBuilder.CreateIndex(
                name: "IX_Hoadon_MaHD",
                table: "Hoadon",
                column: "MaHD",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HoadonCT_IdHD",
                table: "HoadonCT",
                column: "IdHD");

            migrationBuilder.CreateIndex(
                name: "IX_HoadonCT_IdSP",
                table: "HoadonCT",
                column: "IdSP");

            migrationBuilder.CreateIndex(
                name: "IX_Phieuchi_IdNCC",
                table: "Phieuchi",
                column: "IdNCC");

            migrationBuilder.CreateIndex(
                name: "IX_Phieuchi_IdNV",
                table: "Phieuchi",
                column: "IdNV");

            migrationBuilder.CreateIndex(
                name: "IX_Phieuchi_MaPC",
                table: "Phieuchi",
                column: "MaPC",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Phieunhap_IdNCC",
                table: "Phieunhap",
                column: "IdNCC");

            migrationBuilder.CreateIndex(
                name: "IX_Phieunhap_IdNV",
                table: "Phieunhap",
                column: "IdNV");

            migrationBuilder.CreateIndex(
                name: "IX_Phieunhap_MaPN",
                table: "Phieunhap",
                column: "MaPN",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sanpham_IdNhom",
                table: "Sanpham",
                column: "IdNhom");

            migrationBuilder.CreateIndex(
                name: "IX_Tieuhao_IdHoadonCT",
                table: "Tieuhao",
                column: "IdHoadonCT");

            migrationBuilder.CreateIndex(
                name: "IX_Tieuhao_IdSP",
                table: "Tieuhao",
                column: "IdSP");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bangluong");

            migrationBuilder.DropTable(
                name: "Chamcong");

            migrationBuilder.DropTable(
                name: "Dinhluong");

            migrationBuilder.DropTable(
                name: "Dongnhap");

            migrationBuilder.DropTable(
                name: "Phieuchi");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Tieuhao");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Phieunhap");

            migrationBuilder.DropTable(
                name: "HoadonCT");

            migrationBuilder.DropTable(
                name: "Nhacungcap");

            migrationBuilder.DropTable(
                name: "Hoadon");

            migrationBuilder.DropTable(
                name: "Sanpham");

            migrationBuilder.DropTable(
                name: "Khachhang");

            migrationBuilder.DropTable(
                name: "Nhanvien");

            migrationBuilder.DropTable(
                name: "NhomSP");
        }
    }
}
