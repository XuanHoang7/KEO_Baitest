using KEO_Baitest.Data.Entities;

namespace KEO_Baitest.Repository.Interfaces
{
    public interface IVatTuRepository : IBaseRepository<VatTu>
    {
        bool checkMaKeToan(string maKeToan);
    }
}
