using System.ComponentModel.DataAnnotations;

namespace KEO_Baitest.Data.Entities
{
    public class ThanhPham : Entities
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        private string _maThanhPham;

        [Required]
        [MaxLength(50)]
        public string MaThanhPham
        {
            get => _maThanhPham;
            set => _maThanhPham = value?.Trim().ToUpper().Replace(" ", string.Empty);
        }

        [Required]
        [MaxLength(50)]
        public string MaKeToan { get; set; }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        public Guid DonViTinhId { get; set; }
        public DonViTinh DonViTinh { get; set; }

        [Required]
        public Guid NhomThanhPhamId { get; set; }
        public NhomThanhPham NhomThanhPham { get; set; }

        public ICollection<QRThanhPham>? QRThanhPhams { get; set; }
        public ICollection<PhieuThanhPhamDetail>? PhieuThanhPhamDetails { get; set; }

    }
}
