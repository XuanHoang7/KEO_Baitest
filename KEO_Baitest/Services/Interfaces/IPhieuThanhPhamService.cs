using KEO_Baitest.Data.DTOs;
using KiemTraThuViec1.Data;

namespace KEO_Baitest.Services.Interfaces
{
    public interface IPhieuThanhPhamService
    {
        public ResponseDTO Add(PhieuVatPhamDTO dto);
        public ResponseDTO Delete(string ma);
        public ResponseDTO GetByMa(string ma);
        public ResponseDTO Update(PhieuVatTuUpdateDTO dto);
        public ResponseGetDTO<PhieuDTO> GetPhieus(DateTime? dateFrom, DateTime? DateTo, int page, bool isNhap);
        public ResponseGetDTO<BC_NhapKhoThanhPhamDTO> BC_NhapKhoThanhPham(DateTime? dateFrom, DateTime? dateTo, int page);
        public ResponseGetDTO<BC_XuatKhoThanhPhamDTO> BC_XuatKhoThanhPham(DateTime? dateFrom, DateTime? dateTo, int page);
        public ResponseGetDTO<BC_N_X_T_VatTu_ThanhPham> BC_N_X_SumThanhPham(DateTime? dateFrom, DateTime? dateTo, int page);
    }
}
