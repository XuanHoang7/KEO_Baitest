using System.ComponentModel.DataAnnotations;

namespace KEO_Baitest.Data.Entities
{
    public class PhieuVatTuDetail : Entities
    {
        [Required]
        public Guid PhieuVatTuId { get; set; }
        public PhieuVatTu PhieuVatTu { get; set; }

        [Required]
        public Guid VatTuId { get; set; }
        public VatTu vatTu { get; set; }

        [Required]
        public double SoLuong { get; set; }

        public Guid? NhaCungCapID { get; set; }
        public NhaCungCap? NhaCungCap { get; set; }

        public string? SoLot { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        [Required]
        public DateTime ThoiGian { get; set; }

        public string? NguoiDuyet { get; set; }

        [Required]
        public bool Status { get; set; } = false;

        public string QRCode { get; set; }

    }
}
