using KEO_Baitest.Data.Entities;

namespace KEO_Baitest.Repository.Interfaces
{
    public interface IKhoThanhPhamRepository : IBaseRepository<KhoThanhPham>
    {
        KhoThanhPham? GetKhoThanhPhamByMa(string makhoThanhPham);
    }
}
