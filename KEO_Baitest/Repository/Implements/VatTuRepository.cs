using KEO_Baitest.Data.Entities;
using KEO_Baitest.Data;
using KEO_Baitest.Repository.Interfaces;

namespace KEO_Baitest.Repository.Implements
{
    public class VatTuRepository : BaseRepository<VatTu>, IVatTuRepository
    {
        public VatTuRepository(ApplicationDbContext context) : base(context)
        {
        }
        public bool checkMaKeToan(string maKeToan)
        {
            return _context.VatTus.Any(r => r.MaKeToan.Equals(maKeToan.Trim().ToUpper()
            .Replace(" ", string.Empty)) && r.IsDeleted == false);
        }
    }
}
