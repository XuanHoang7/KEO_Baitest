using System.ComponentModel.DataAnnotations;

namespace KEO_Baitest.Data.Entities
{
    public class NhomThanhPham : Entities
    {

        private string _maNhomThanhPham;

        [Required]
        [MaxLength(50)]
        public string MaNhomThanhPham
        {
            get => _maNhomThanhPham;
            set => _maNhomThanhPham = value?.Trim().ToUpper().Replace(" ", string.Empty);
        }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        public ICollection<ThanhPham> ThanhPhams { get; set; }
    }
}
