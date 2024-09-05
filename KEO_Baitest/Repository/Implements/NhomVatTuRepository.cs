using KEO_Baitest.Data.Entities;
using KEO_Baitest.Data;
using KEO_Baitest.Repository.Interfaces;

namespace KEO_Baitest.Repository.Implements
{
    public class NhomVatTuRepository : BaseRepository<NhomVatTu>, INhomVatTuRepository
    {
        public NhomVatTuRepository(ApplicationDbContext context) : base(context)
        {
        }
        public NhomVatTu? GetNhomVatTuByMa(string maNhomVatTu)
        {
            return _context.NhomVatTus.FirstOrDefault(r =>
            r.MaNhomVatTu.Equals(maNhomVatTu.Trim().ToUpper()
            .Replace(" ", string.Empty)) && r.IsDeleted == false);
        }
    }
}
