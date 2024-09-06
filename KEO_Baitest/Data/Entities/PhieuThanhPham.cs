using System.ComponentModel.DataAnnotations;

namespace KEO_Baitest.Data.Entities
{
    public class PhieuThanhPham : Entities
    {

        private string _maPhieu;

        [Required]
        [MaxLength(50)]
        public string MaPhieu
        {
            get => _maPhieu;
            set => _maPhieu = value?.Trim().ToUpper().Replace(" ", string.Empty);
        }

        [Required]
        public Guid KhoThanhPhamId { get; set; }
        public KhoThanhPham KhoThanhPham { get; set; }

        [Required]
        public bool Status { get; set; } = false;

        // Flag to distinguish between Import and Export
        [Required]
        public bool ImportOrExport { get; set; } = true;

        public ICollection<PhieuThanhPhamDetail>? PhieuThanhPhamDetails { get; set; }
    }
}
