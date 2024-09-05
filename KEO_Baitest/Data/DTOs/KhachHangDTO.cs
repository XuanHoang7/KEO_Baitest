using System.ComponentModel.DataAnnotations;

namespace KEO_Baitest.Data.DTOs
{
    public class KhachHangDTO
    {
        public string MaKhachHang { get; set; }

        public string TenKhachHang { get; set; }

        public string? DiaChi { get; set; }
        public string? SoDienThoai { get; set; }
    }
}
