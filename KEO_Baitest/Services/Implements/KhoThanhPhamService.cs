using KEO_Baitest.Data.DTOs;
using KEO_Baitest.Data.Entities;
using KEO_Baitest.Repository.Interfaces;
using KEO_Baitest.Services.Interfaces;
using KiemTraThuViec1.Data;

namespace KEO_Baitest.Services.Implements
{
    public class KhoThanhPhamService : GenericService<KhoThanhPham, KhoThanhPhamDTO>
    {
        public KhoThanhPhamService(IKhoThanhPhamRepository repository, IUserService userService) : base(repository, userService)
        {
        }

        protected override KhoThanhPham? getEntityByDto(KhoThanhPhamDTO dto)
        {
            return _repository.Find(r => (r.IsDeleted == false) && r.MaKhoThanhPham.Equals(dto.MaKhoThanhPham.Trim().ToUpper().Replace(" ", string.Empty)))
                .FirstOrDefault();
        }

        protected override KhoThanhPham? getEntityByMa(string ma)
        {
            return _repository.Find(r => (r.IsDeleted == false) && r.MaKhoThanhPham.Equals(ma.Trim().ToUpper().Replace(" ", string.Empty)))
                .FirstOrDefault();
        }

        protected override KhoThanhPhamDTO MapToDto(KhoThanhPham entity)
        {
            return new KhoThanhPhamDTO()
            {
                MaKhoThanhPham = entity.MaKhoThanhPham,
                TenKhoThanhPham = entity.Name
            };
        }

        protected override KhoThanhPham MapToEntity(KhoThanhPhamDTO dto)
        {
            return new KhoThanhPham()
            {
                MaKhoThanhPham = dto.MaKhoThanhPham,
                Name = dto.TenKhoThanhPham
            };
        }

        protected override KhoThanhPham? UpdateEntityF(KhoThanhPham entity, KhoThanhPhamDTO dto)
        {
            entity.Name = dto.TenKhoThanhPham;
            return entity;
        }

        protected override ResponseDTO? ValidateDTO(KhoThanhPhamDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.MaKhoThanhPham))
                return new ResponseDTO { Code = 400, Message = "Mã kho thành phẩm là null or only whitespace" };

            if (string.IsNullOrWhiteSpace(dto.TenKhoThanhPham))
                return new ResponseDTO { Code = 400, Message = "Tên kho thành phẩm là null or only whitespace" };
            return null;
        }
    }
}
