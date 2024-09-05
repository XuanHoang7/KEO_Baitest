using System.ComponentModel.DataAnnotations;

namespace KEO_Baitest.Data.Entities
{
    public class PhieuThanhPhamDetail : Entities
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid PhieuThanhPhamId { get; set; }
        public PhieuThanhPham PhieuThanhPham { get; set; }

        [Required]
        public Guid ThanhPhamId { get; set; }
        public ThanhPham ThanhPham { get; set; }

        [Required]
        public double SoLuong { get; set; }

        public Guid? NhaCungCapID { get; set; }
        public NhaCungCap NhaCungCap { get; set; }

        public string? SoLot { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        [Required]
        public DateTime ThoiGian { get; set; }

        // This field is only relevant if it's an Export (ImportOrExport = false)
        public Guid? KhachHangId { get; set; }
        public KhachHang KhachHang { get; set; }

        public string? NguoiDuyet { get; set; }

        [Required]
        public bool Status { get; set; } = false;
        [Required]
        public string QRCode { get; set; }
    }
}
