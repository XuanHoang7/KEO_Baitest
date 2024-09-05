using System.ComponentModel.DataAnnotations;

namespace KEO_Baitest.Data.Entities
{
    public class KhachHang : Entities
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        private string _maKhachHang;

        [Required]
        [MaxLength(50)]
        public string MaKhachHang
        {
            get => _maKhachHang;
            set => _maKhachHang = value?.Trim().ToUpper().Replace(" ", string.Empty);
        }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        [MaxLength(255)]
        public string? DiaChi { get; set; }

        [Required]
        [MaxLength(15)]
        public string? SoDienThoai { get; set; }

        // Navigation property
        public ICollection<PhieuThanhPhamDetail>? PhieuThanhPhamDetails { get; set; }
        //public ICollection<SanPham> SanPhams { get; set; }
    }
}
