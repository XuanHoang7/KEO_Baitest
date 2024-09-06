using System.ComponentModel.DataAnnotations;

namespace KEO_Baitest.Data.Entities
{
    public class NhaCungCap : Entities
    {

        private string _maNhaCungCap;

        [Required]
        [MaxLength(50)]
        public string MaNhaCungCap
        {
            get => _maNhaCungCap;
            set => _maNhaCungCap = value?.Trim().ToUpper().Replace(" ", string.Empty);
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
        public ICollection<PhieuVatTuDetail>? PhieuVatTuDetails { get; set; }
        public ICollection<PhieuThanhPhamDetail>? PhieuThanhPhamDetails { get; set; }
    }
}
