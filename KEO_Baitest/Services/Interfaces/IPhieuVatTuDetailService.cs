using KEO_Baitest.Data.DTOs;
using KiemTraThuViec1.Data;

namespace KEO_Baitest.Services.Interfaces
{
    public interface IPhieuVatTuDetailService
    {
        ResponseDTO AddPhieuVatTuDetail(PhieuVatTuDTO phieuVatTuDTO, bool status);
    }
}
