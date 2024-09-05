using KEO_Baitest.Data.Entities;
using KEO_Baitest.Data;
using KEO_Baitest.Repository.Interfaces;

namespace KEO_Baitest.Repository.Implements
{
    public class KhoThanhPhamRepository : BaseRepository<KhoThanhPham>, IKhoThanhPhamRepository
    {
        public KhoThanhPhamRepository(ApplicationDbContext context) : base(context)
        {
        }
        public KhoThanhPham? GetKhoThanhPhamByMa(string makhoThanhPham)
        {
            return _context.KhoThanhPhams.FirstOrDefault(r =>
            r.MaKhoThanhPham.Equals(makhoThanhPham.Trim().ToUpper()
            .Replace(" ", string.Empty)) && (r.IsDeleted == false));
        }
    }
}
