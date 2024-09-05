using KEO_Baitest.Data.Entities;
using KEO_Baitest.Data;
using KEO_Baitest.Repository.Interfaces;

namespace KEO_Baitest.Repository.Implements
{
    public class QRVatTuRepository : BaseRepository<QRVatTu>, IQRVatTuRepository
    {
        public QRVatTuRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
