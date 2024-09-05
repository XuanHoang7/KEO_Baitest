using System.ComponentModel.DataAnnotations;

namespace KEO_Baitest.Data.Entities
{
    public class PhieuVatTu : Entities
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        private string _maPhieu;

        [Required]
        [MaxLength(50)]
        public string MaPhieu
        {
            get => _maPhieu;
            set => _maPhieu = value?.Trim().ToUpper().Replace(" ", string.Empty);
        }
        [Required]
        public Guid KhoVatTuId { get; set; }
        public KhoVatTu KhoVatTu { get; set; }

        [Required]
        public bool Status { get; set; } = false;

        [Required]
        public bool ImportOrExport { get; set; } = false;

        public ICollection<PhieuVatTuDetail> PhieuVatTuDetails { get; set; }
    }
}
