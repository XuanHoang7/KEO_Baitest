using KEO_Baitest.Data.Entities;
using KEO_Baitest.Data;
using KEO_Baitest.Repository.Interfaces;

namespace KEO_Baitest.Repository.Implements
{
    public class QRThanhPhamRepository : BaseRepository<QRThanhPham>, IQRThanhPhamRepository
    {
        public QRThanhPhamRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
