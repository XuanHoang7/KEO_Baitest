using KEO_Baitest.Data;
using KEO_Baitest.Data.Entities;
using KEO_Baitest.Repository.Interfaces;

namespace KEO_Baitest.Repository.Implements
{
    public class KhachHangRepository : BaseRepository<KhachHang>, IKhachHangRepository
    {
        public KhachHangRepository(ApplicationDbContext context) : base(context)
        {
        } 

        public KhachHang? GetKhachHangByMa(string maKhachHang)
        {
            return _context.KhachHangs.FirstOrDefault(r =>
            r.MaKhachHang.Equals(maKhachHang.Trim().ToUpper()
            .Replace(" ", string.Empty)) && r.IsDeleted == false);
        }
    }
}
