namespace KEO_Baitest.Data.DTOs
{
    public class NhapXuatKhoVatTuDetailDTO
    {
        public Guid Id { get; set; }
        public string MaKyThuat { get; set; }
        public string MaKeToan {  get; set; }
        public string TenVatTu {  get; set; }
        public string TenDvt { get; set; }
        public double SoLuong { get; set; }
        public string? TenNhaCungCap { get; set; }
        public string? SoLot {  get; set; }
        public string? Note { get; set; }
        public string ThoiGian { get; set; }
        public string NguoiNhap { get; set; }
        public string? NguoiDuyet { get; set; }
        public bool IsDuyet { get; set; } = false;

    }
}
