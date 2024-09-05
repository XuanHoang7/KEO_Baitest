using KEO_Baitest.Data.Entities;
using KEO_Baitest.Data;
using KEO_Baitest.Repository.Interfaces;

namespace KEO_Baitest.Repository.Implements
{
    public class PhieuThanhPhamRepository : BaseRepository<PhieuThanhPham>, IPhieuThanhPhamRepository
    {
        public PhieuThanhPhamRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
