using KEO_Baitest.Data.Entities;

namespace KEO_Baitest.Repository.Interfaces
{
    public interface IKhachHangRepository : IBaseRepository<KhachHang>
    {
        KhachHang? GetKhachHangByMa(string maKhachHang);
    }
}
