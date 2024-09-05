namespace KEO_Baitest.Data.DTOs
{
    public class PhieuVatTuUpdateDTO
    {
        public string Id { get; set; }
        public double? SoLuong { get; set; }
        public string? Notes { get; set; }
        public bool IsDuyet { get; set; } = false;  
    }
}
