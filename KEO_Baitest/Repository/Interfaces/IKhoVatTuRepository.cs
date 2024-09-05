using KEO_Baitest.Data.Entities;

namespace KEO_Baitest.Repository.Interfaces
{
    public interface IKhoVatTuRepository : IBaseRepository<KhoVatTu>
    {
        KhoVatTu? GetKhoVatTuByMa(string maKhoVatTu);
    }
}
