using KEO_Baitest.Data.Entities;

namespace KEO_Baitest.Repository.Interfaces
{
    public interface INhomThanhPhamRepository : IBaseRepository<NhomThanhPham>
    {
        NhomThanhPham? GetNhomThanhPhamByMa(string nameNhomThanhPham);
    }
}
