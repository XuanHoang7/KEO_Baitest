using KEO_Baitest.Data.Entities;
using KEO_Baitest.Data;
using KEO_Baitest.Repository.Interfaces;

namespace KEO_Baitest.Repository.Implements
{
    public class NhomThanhPhamRepository : BaseRepository<NhomThanhPham>, INhomThanhPhamRepository
    {
        public NhomThanhPhamRepository(ApplicationDbContext context) : base(context)
        {
        }

        public NhomThanhPham? GetNhomThanhPhamByMa(string maNhomThanhPham)
        {
            return _context.NhomThanhPhams.FirstOrDefault(r =>
            r.MaNhomThanhPham.Equals(maNhomThanhPham.Trim().ToUpper()
            .Replace(" ", string.Empty)) && r.IsDeleted == false);
        }
    }
}
