using KEO_Baitest.Data.Entities;

namespace KEO_Baitest.Repository.Interfaces
{
    public interface IDonViTinhRepository : IBaseRepository<DonViTinh>
    {
        DonViTinh? GetDonViTinhByMa(string maDonViTinh);
    }
}
