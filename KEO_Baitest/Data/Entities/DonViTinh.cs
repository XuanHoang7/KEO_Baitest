using System.ComponentModel.DataAnnotations;

namespace KEO_Baitest.Data.Entities
{
    public class DonViTinh : Entities
    {

        private string _maDonViTinh;

        [Required]
        [MaxLength(50)]
        public string MaDonViTinh
        {
            get => _maDonViTinh;
            set => _maDonViTinh = value?.Trim().ToUpper().Replace(" ", string.Empty);
        }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        public ICollection<VatTu> VatTus { get; set; }
        public ICollection<ThanhPham> ThanhPhams { get; set; }

    }
}
