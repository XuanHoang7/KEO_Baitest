using KEO_Baitest.Data;
using KEO_Baitest.Data.Entities;
using KEO_Baitest.Repository.Interfaces;

namespace KEO_Baitest.Repository.Implements
{
    public class PhieuVatTuDetailRepository : BaseRepository<PhieuVatTuDetail>, IPhieuVatTuDetailRepository
    {
        public PhieuVatTuDetailRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
