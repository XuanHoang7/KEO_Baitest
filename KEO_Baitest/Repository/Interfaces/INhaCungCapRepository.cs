using KEO_Baitest.Data.Entities;

namespace KEO_Baitest.Repository.Interfaces
{
    public interface INhaCungCapRepository : IBaseRepository<NhaCungCap>
    {
        NhaCungCap? GetNhaCungCapByMa(string maNhaCungCap);
    }
}
