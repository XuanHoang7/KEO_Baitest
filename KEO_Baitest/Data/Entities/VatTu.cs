using System.ComponentModel.DataAnnotations;

namespace KEO_Baitest.Data.Entities
{
    public class VatTu : Entities
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        private string _maKyThuat;

        [Required]
        [MaxLength(50)]
        public string MaKyThuat
        {
            get => _maKyThuat;
            set => _maKyThuat = value?.Trim().ToUpper().Replace(" ", string.Empty);
        }

        [Required]
        [MaxLength(50)]
        public string _maKeToan { get; set; }
        public string MaKeToan
        {
            get => _maKeToan;
            set => _maKeToan = value?.Trim().ToUpper().Replace(" ", string.Empty);
        }

        [Required]
        [MaxLength(255)]
        public string Name { get; set; }

        [Required]
        public Guid DonViTinhId { get; set; }
        public DonViTinh DonViTinh { get; set; }

        [Required]
        public Guid NhomVatTuId { get; set; }
        public NhomVatTu NhomVatTu { get; set; }

        public ICollection<QRVatTu> QRVatTus { get; set; }
        public ICollection<PhieuVatTuDetail> PhieuVatTuDetails { get; set; }
    }
}
