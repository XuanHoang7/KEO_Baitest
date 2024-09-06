using System.ComponentModel.DataAnnotations;

namespace KEO_Baitest.Data.Entities
{
    public class QRVatTu : Entities
    {
        [Required]
        public Guid VatTuId { get; set; }
        public VatTu vatTu { get; set; }
        public string? SoLot { get; set; }
        public Guid? NhaCungCapId { get; set; }
        public NhaCungCap? NhaCungCap { get; set; }
        public string QRCode { get; set; }
    }
}
