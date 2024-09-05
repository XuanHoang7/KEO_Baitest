using KEO_Baitest.Data.DTOs;
using KiemTraThuViec1.Data;

namespace KEO_Baitest.Services.Interfaces
{
    public interface IPhieuVatTuService
    {
        public ResponseDTO Add(PhieuVatTuDTO dto);
        public ResponseDTO Delete(string ma);
        public ResponseDTO GetByMa(string ma);
        public ResponseDTO Update(PhieuVatTuUpdateDTO dto);
        public ResponseGetDTO<PhieuDTO> GetPhieus(DateTime? dateFrom, DateTime? dateTo, int page, bool isNhap);
        public ResponseGetDTO<BC_XuatNhapKhoVatTu> BC_N_X_KhoVatTu(DateTime? dateFrom, DateTime? dateTo, int page, bool status);
        public ResponseGetDTO<BC_N_X_T_VatTu_ThanhPhamDTO> BC_N_X_SumKhoVatTu(DateTime? dateFrom, DateTime? dateTo, int page);
    }
}
