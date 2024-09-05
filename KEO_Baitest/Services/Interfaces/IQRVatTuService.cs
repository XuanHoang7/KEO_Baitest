using KEO_Baitest.Data.DTOs;
using KEO_Baitest.Data.Entities;
using KiemTraThuViec1.Data;

namespace KEO_Baitest.Services.Interfaces
{
    public interface IQRVatTuService
    {
        public ResponseDTO Add(QRVatTuE dto);
        public ResponseDTO Delete(string Id);
        public QRVatTu? GetByQRCode(string qr);
        public ResponseGetDTO<QRVatTuO> GetAll(int page, string keyword);
    }
}
