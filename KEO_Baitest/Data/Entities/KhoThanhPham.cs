using System.ComponentModel.DataAnnotations;

namespace KEO_Baitest.Data.Entities
{
    public class KhoThanhPham : Entities
    {

        private string _maKhoThanhPham;

        [Required]
        [MaxLength(50)]
        public string MaKhoThanhPham
        {
            get => _maKhoThanhPham;
            set => _maKhoThanhPham = value?.Trim().ToUpper().Replace(" ", string.Empty);
        }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        public ICollection<PhieuThanhPham>? PhieuThanhPhamNhaps { get; set; }
    }
}
