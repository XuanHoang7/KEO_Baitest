using System.ComponentModel.DataAnnotations;

namespace KEO_Baitest.Data.Entities
{
    public class QRThanhPham : Entities
    {
        [Required]
        public Guid ThanhPhamId { get; set; }
        public ThanhPham thanhPham { get; set; }
        public string? SoLot { get; set; }
        public string QRCode { get; set; }
    }
}
