using CafeManagement.Models;
using CafeManagement.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CafeManagement.Data;

/// <summary>
/// ApplicationDbContext - Simple database context with basic User authentication
/// and application domain entities. No Identity framework complexity.
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // Authentication
    public DbSet<User> Users { get; set; } = null!;

    // Domain entities
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<NhomSP> NhomSPs { get; set; } = null!;
    public DbSet<Sanpham> Sanphams { get; set; } = null!;
    public DbSet<Khachhang> Khachhangs { get; set; } = null!;
    public DbSet<Nhanvien> Nhanviens { get; set; } = null!;
    public DbSet<Hoadon> Hoadons { get; set; } = null!;
    public DbSet<HoadonCT> HoadonCTs { get; set; } = null!;
    public DbSet<Nhacungcap> Nhacungcaps { get; set; } = null!;
    public DbSet<Phieunhap> Phieunhaps { get; set; } = null!;
    public DbSet<Dongnhap> Dongnhaps { get; set; } = null!;
    public DbSet<Phieuchi> Phieuchis { get; set; } = null!;
    public DbSet<Tieuhao> Tieuhaos { get; set; } = null!;
    public DbSet<Dinhluong> Dinhluongs { get; set; } = null!;
    public DbSet<Chamcong> Chamcongs { get; set; } = null!;
    public DbSet<Bangluong> Bangluongs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // ============= User (Simple Authentication) =============
        builder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Email).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Password).IsRequired();
            entity.Property(e => e.Role).HasMaxLength(50).IsRequired();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        // ============= Product (legacy) =============
        builder.Entity<Product>(entity =>
        {
            entity.ToTable("Products");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
            entity.Property(e => e.Price).HasPrecision(10, 2);
        });

        // ============= NhomSP (Product Category) =============
        builder.Entity<NhomSP>(entity =>
        {
            entity.ToTable("NhomSP");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TenNhom).HasMaxLength(100).IsRequired();
            entity.Property(e => e.MoTa).HasMaxLength(500);
            entity.Property(e => e.NgayTao).HasColumnType("timestamp");
            entity.Property(e => e.NgayCapNhat).HasColumnType("timestamp");
            
            entity.HasMany(e => e.SanPhams)
                .WithOne(e => e.NhomSP)
                .HasForeignKey(e => e.IdNhom)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ============= Sanpham (Product) =============
        builder.Entity<Sanpham>(entity =>
        {
            entity.ToTable("Sanpham");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TenSP).HasMaxLength(100).IsRequired();
            entity.Property(e => e.MoTa).HasMaxLength(500);
            entity.Property(e => e.GiaBan).HasPrecision(10, 2);
            entity.Property(e => e.GiaNhap).HasPrecision(10, 2);
            entity.Property(e => e.DonViTinh).HasMaxLength(50);
            entity.Property(e => e.NgayTao).HasColumnType("timestamp");
            entity.Property(e => e.NgayCapNhat).HasColumnType("timestamp");
            
            entity.HasMany(e => e.HoadonCTs)
                .WithOne(e => e.Sanpham)
                .HasForeignKey(e => e.IdSP)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasMany(e => e.Dongnhaps)
                .WithOne(e => e.Sanpham)
                .HasForeignKey(e => e.IdSP)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasMany(e => e.Tieuhaos)
                .WithOne(e => e.Sanpham)
                .HasForeignKey(e => e.IdSP)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasMany(p => p.Dinhluongs)
                .WithOne(d => d.Sanpham)
                .HasForeignKey(d => d.IdSP)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ============= Khachhang (Customer) =============
        builder.Entity<Khachhang>(entity =>
        {
            entity.ToTable("Khachhang");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TenKH).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Sdt).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.DiaChi).HasMaxLength(200);
            entity.Property(e => e.CongNo).HasPrecision(10, 2);
            entity.Property(e => e.NgayTao).HasColumnType("timestamp");
            entity.Property(e => e.NgayCapNhat).HasColumnType("timestamp");
            
            entity.HasMany(e => e.Hoadons)
                .WithOne(e => e.Khachhang)
                .HasForeignKey(e => e.IdKH)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ============= Nhanvien (Employee) =============
        builder.Entity<Nhanvien>(entity =>
        {
            entity.ToTable("Nhanvien");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TenNV).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Sdt).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.DiaChi).HasMaxLength(200);
            entity.Property(e => e.CMND).HasMaxLength(20);
            entity.Property(e => e.ChucVu).HasMaxLength(50);
            entity.Property(e => e.Luong).HasPrecision(10, 2);
            entity.Property(e => e.NgayTao).HasColumnType("timestamp");
            entity.Property(e => e.NgayCapNhat).HasColumnType("timestamp");
            
            entity.HasMany(e => e.Hoadons)
                .WithOne(e => e.Nhanvien)
                .HasForeignKey(e => e.IdNV)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            
            entity.HasMany(e => e.Chamcongs)
                .WithOne(e => e.Nhanvien)
                .HasForeignKey(e => e.IdNV)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasMany(e => e.Bangluongs)
                .WithOne(e => e.Nhanvien)
                .HasForeignKey(e => e.IdNV)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
            
            entity.HasMany(e => e.Phieunhaps)
                .WithOne(e => e.Nhanvien)
                .HasForeignKey(e => e.IdNV)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
            
        });

        // ============= Hoadon (Invoice) =============
        builder.Entity<Hoadon>(entity =>
        {
            entity.ToTable("Hoadon");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MaHD).HasMaxLength(50).IsRequired();
            entity.Property(e => e.ThanhTien).HasPrecision(12, 2);
            entity.Property(e => e.GiamGia).HasPrecision(12, 2);
            entity.Property(e => e.TongCong).HasPrecision(12, 2);
            entity.Property(e => e.GhiChu).HasMaxLength(200);
            entity.Property(e => e.NgayTao).HasColumnType("timestamp");
            entity.Property(e => e.NgayCapNhat).HasColumnType("timestamp");
            entity.HasIndex(e => e.MaHD).IsUnique();
            
            entity.HasMany(e => e.HoadonCTs)
                .WithOne(e => e.Hoadon)
                .HasForeignKey(e => e.IdHD)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ============= HoadonCT (Invoice Detail) =============
        builder.Entity<HoadonCT>(entity =>
        {
            entity.ToTable("HoadonCT");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DonGia).HasPrecision(10, 2);
            entity.Property(e => e.ThanhTien).HasPrecision(12, 2);
            entity.Property(e => e.GhiChu).HasMaxLength(200);
            
            entity.HasMany(e => e.Tieuhaos)
                .WithOne(e => e.HoadonCT)
                .HasForeignKey(e => e.IdHoadonCT)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ============= Nhacungcap (Supplier) =============
        builder.Entity<Nhacungcap>(entity =>
        {
            entity.ToTable("Nhacungcap");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TenNCC).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Sdt).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.DiaChi).HasMaxLength(200);
            entity.Property(e => e.NguoiDaiDien).HasMaxLength(100);
            entity.Property(e => e.TaxID).HasMaxLength(20);
            entity.Property(e => e.CongNo).HasPrecision(10, 2);
            entity.Property(e => e.NgayTao).HasColumnType("timestamp");
            entity.Property(e => e.NgayCapNhat).HasColumnType("timestamp");
            
            entity.HasMany(e => e.Phieunhaps)
                .WithOne(e => e.Nhacungcap)
                .HasForeignKey(e => e.IdNCC)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
        });

        // ============= Phieunhap (Purchase Receipt) =============
        builder.Entity<Phieunhap>(entity =>
        {
            entity.ToTable("Phieunhap");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MaPN).HasMaxLength(50).IsRequired();
            entity.Property(e => e.TongTien).HasPrecision(12, 2);
            entity.Property(e => e.GhiChu).HasMaxLength(200);
            entity.Property(e => e.NgayTao).HasColumnType("timestamp");
            entity.Property(e => e.NgayCapNhat).HasColumnType("timestamp");
            entity.HasIndex(e => e.MaPN).IsUnique();
            
            entity.HasMany(e => e.Dongnhaps)
                .WithOne(e => e.Phieunhap)
                .HasForeignKey(e => e.IdPhieuNhap)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });

        // ============= Dongnhap (Purchase Line Item) =============
        builder.Entity<Dongnhap>(entity =>
        {
            entity.ToTable("Dongnhap");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DonGia).HasPrecision(10, 2);
            entity.Property(e => e.GhiChu).HasMaxLength(200);
        });

        // ============= Phieuchi (Expense Voucher) =============
        builder.Entity<Phieuchi>(entity =>
        {
            entity.ToTable("Phieuchi");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MaPC).HasMaxLength(50).IsRequired();
            entity.Property(e => e.SoTien).HasPrecision(12, 2);
            entity.Property(e => e.GhiChu).HasMaxLength(200);
            entity.Property(e => e.NgayTao).HasColumnType("timestamp");
            entity.Property(e => e.NgayCapNhat).HasColumnType("timestamp");
            entity.HasIndex(e => e.MaPC).IsUnique();
        });

        // ============= Tieuhao (Waste/Usage) =============
        builder.Entity<Tieuhao>(entity =>
        {
            entity.ToTable("Tieuhao");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DonGiaVon).HasPrecision(10, 2);
            entity.Property(e => e.NgayTao).HasColumnType("timestamp");
        });

        // ============= Dinhluong (Recipe/Formula) =============
        builder.Entity<Dinhluong>(entity =>
        {
            entity.ToTable("Dinhluong");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.DonVi).HasMaxLength(50);
            entity.Property(e => e.GhiChu).HasMaxLength(200);
            entity.Property(e => e.NgayTao).HasColumnType("timestamp");
            entity.Property(e => e.NgayCapNhat).HasColumnType("timestamp");

            entity.HasOne(e => e.Sanpham)
                .WithMany(e => e.Dinhluongs)
                .HasForeignKey(e => e.IdSP)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.ThanhphanSanpham)
                .WithMany(e => e.DinhluongThanhPhans)
                .HasForeignKey(e => e.IdThanhPhan)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => new { e.IdSP, e.IdThanhPhan }).IsUnique();
        });

        // ============= Chamcong (Attendance) =============
        builder.Entity<Chamcong>(entity =>
        {
            entity.ToTable("Chamcong");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.GhiChu).HasMaxLength(200);
            entity.Property(e => e.NgayTao).HasColumnType("timestamp");
            entity.HasIndex(e => new { e.IdNV, e.Ngay }).IsUnique();
        });

        // ============= Bangluong (Payroll) =============
        builder.Entity<Bangluong>(entity =>
        {
            entity.ToTable("Bangluong");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Luong).HasPrecision(12, 2);
            entity.Property(e => e.PhuCap).HasPrecision(12, 2);
            entity.Property(e => e.KhauTru).HasPrecision(12, 2);
            entity.Property(e => e.TongLuong).HasPrecision(12, 2);
            entity.Property(e => e.GhiChu).HasMaxLength(200);
            entity.Property(e => e.NgayTao).HasColumnType("timestamp");
            entity.Property(e => e.NgayCapNhat).HasColumnType("timestamp");
            entity.HasIndex(e => new { e.IdNV, e.Nam, e.Thang }).IsUnique();
        });
    }
}
