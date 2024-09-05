using KEO_Baitest.Data;
using KEO_Baitest.Data.Entities;
using KEO_Baitest.Repository.Interfaces;

namespace KEO_Baitest.Repository.Implements
{
    public class NhaCungCapRepository : BaseRepository<NhaCungCap>, INhaCungCapRepository
    {
        public NhaCungCapRepository(ApplicationDbContext context) : base(context)
        {
        }

        public NhaCungCap? GetNhaCungCapByMa(string maNhaCungCap)
        {
            return _context.NhaCungCaps.FirstOrDefault(r =>
            r.MaNhaCungCap.Equals(maNhaCungCap.Trim().ToUpper()
            .Replace(" ", string.Empty)) && r.IsDeleted == false);
        }
    }
}
