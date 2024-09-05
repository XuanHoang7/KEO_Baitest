using KEO_Baitest.Data.Entities;

namespace KEO_Baitest.Repository.Interfaces
{
    public interface INhomVatTuRepository : IBaseRepository<NhomVatTu>
    {
        NhomVatTu? GetNhomVatTuByMa(string maNhomVatTu);
    }
}
