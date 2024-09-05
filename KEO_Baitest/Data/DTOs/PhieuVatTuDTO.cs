using System.Runtime.CompilerServices;

namespace KEO_Baitest.Data.DTOs
{
    public class PhieuVatTuDTO
    {
        public string MaKho {  get; set; }

        public string QRCode { get; set; }

        //public string MaVatTu { get; set; }
        //public string? SoLot {  get; set; }
        //public string? MaNhaCungCap { get; set; }
        public double SoLuong { get; set; }
        public bool IsNhapKho { get; set; } = true;
        //public string? Note { get; set; }

    }
}
