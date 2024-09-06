using KEO_Baitest.Data.DTOs;
using KEO_Baitest.Data.Entities;
using KiemTraThuViec1.Data;

namespace KEO_Baitest.Services.Interfaces
{
    public interface IQRThanhPhamService
    {
        public ResponseDTO Add(QRThanhPhamE dto);
        public ResponseDTO Delete(string Id);
        public QRThanhPham? GetByQRCode(string qr);
        public ResponseGetDTO<QRThanhPhamO> GetAll(int page, string name);
        public ResponseDTO Update(UpdateQRThanhPhamDTO dto);
    }
}
