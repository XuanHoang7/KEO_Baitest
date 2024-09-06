using System.ComponentModel.DataAnnotations;

namespace KEO_Baitest.Data.Entities
{
    public class NhomVatTu : Entities
    {

        private string _maNhomVatTu;

        [Required]
        [MaxLength(50)]
        public string MaNhomVatTu
        {
            get => _maNhomVatTu;
            set => _maNhomVatTu = value?.Trim().ToUpper().Replace(" ", string.Empty);
        }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        public ICollection<VatTu> VatTus { get; set; }
    }
}
