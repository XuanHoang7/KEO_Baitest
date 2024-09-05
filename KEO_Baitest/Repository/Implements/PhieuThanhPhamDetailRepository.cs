using KEO_Baitest.Data.Entities;
using KEO_Baitest.Data;
using KEO_Baitest.Repository.Interfaces;

namespace KEO_Baitest.Repository.Implements
{
    public class PhieuThanhPhamDetailRepository : BaseRepository<PhieuThanhPhamDetail>, IPhieuThanhPhamDetailRepository
    {
        public PhieuThanhPhamDetailRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
