using System.ComponentModel.DataAnnotations;

namespace KEO_Baitest.Data.Entities
{
    public class KhoVatTu : Entities
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        private string _maNhaKhoVatTu;

        [Required]
        [MaxLength(50)]
        public string MaNhaKhoVatTu
        {
            get => _maNhaKhoVatTu;
            set => _maNhaKhoVatTu = value?.Trim().ToUpper().Replace(" ", string.Empty);
        }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        public ICollection<PhieuVatTu>? PhieuVatTus { get; set; }
    }
}
