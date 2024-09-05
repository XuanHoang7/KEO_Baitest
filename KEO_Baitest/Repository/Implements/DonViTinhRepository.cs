using KEO_Baitest.Data.Entities;
using KEO_Baitest.Data;
using KEO_Baitest.Repository.Interfaces;

namespace KEO_Baitest.Repository.Implements
{
    public class DonViTinhRepository : BaseRepository<DonViTinh>, IDonViTinhRepository
    {
        public DonViTinhRepository(ApplicationDbContext context) : base(context)
        {
        }

        public DonViTinh? GetDonViTinhByMa(string maDonViTinh)
        {
            return _context.DonViTinhs.FirstOrDefault(r =>
                r.MaDonViTinh.Equals(maDonViTinh.Trim().ToUpper()
                .Replace(" ", string.Empty)) && r.IsDeleted == false);
        }
    }
}
