using KEO_Baitest.Data;
using KEO_Baitest.Data.Entities;
using KEO_Baitest.Repository.Interfaces;

namespace KEO_Baitest.Repository.Implements
{
    public class PhieuVatTuRepository : BaseRepository<PhieuVatTu>, IPhieuVatTuRepository
    {
        public PhieuVatTuRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
