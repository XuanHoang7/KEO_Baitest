using KEO_Baitest.Data.Entities;
using KEO_Baitest.Data;
using KEO_Baitest.Repository.Interfaces;

namespace KEO_Baitest.Repository.Implements
{
    public class ThanhPhamRepository : BaseRepository<ThanhPham>, IThanhPhamRepository
    {
        public ThanhPhamRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
