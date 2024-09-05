using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using KEO_Baitest.Data.Entities;

namespace KEO_Baitest.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        public DbSet<DonViTinh> DonViTinhs { get; set; }
        public DbSet<KhachHang> KhachHangs { get; set; }
        public DbSet<KhoThanhPham> KhoThanhPhams { get; set; }
        public DbSet<KhoVatTu> KhoVatTus { get; set; }
        public DbSet<NhaCungCap> NhaCungCaps { get; set; }
        public DbSet<NhomThanhPham> NhomThanhPhams { get; set; }
        public DbSet<NhomVatTu> NhomVatTus { get; set; }
        public DbSet<PhieuThanhPham> PhieuThanhPhams { get; set; }
        public DbSet<PhieuThanhPhamDetail> PhieuThanhPhamDetails { get; set; }
        public DbSet<PhieuVatTu> PhieuVatTus { get; set; }
        public DbSet<PhieuVatTuDetail> PhieuVatTuDetails { get; set; }
        public DbSet<QRVatTu> QRVatus { get; set; }
        public DbSet<QRThanhPham> QRThanhPhams { get; set; }
        public DbSet<ThanhPham> ThanhPhams { get; set; }
        public DbSet<VatTu> VatTus { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }

    }
}
