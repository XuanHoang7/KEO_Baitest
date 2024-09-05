using KEO_Baitest.Data.Entities;
using KEO_Baitest.Data;
using KEO_Baitest.Repository.Interfaces;

namespace KEO_Baitest.Repository.Implements
{
    public class KhoVatTuRepository : BaseRepository<KhoVatTu>, IKhoVatTuRepository
    {
        public KhoVatTuRepository(ApplicationDbContext context) : base(context)
        {
        }
        
        public KhoVatTu? GetKhoVatTuByMa(string maKhoVatTu)
        {
            return _context.KhoVatTus.FirstOrDefault(r =>
            r.MaNhaKhoVatTu.Equals(maKhoVatTu.Trim().ToUpper()
            .Replace(" ", string.Empty)) && r.IsDeleted == false);
        }
    }
}
