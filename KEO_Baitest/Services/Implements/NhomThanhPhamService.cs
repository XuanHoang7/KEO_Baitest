using KEO_Baitest.Data.DTOs;
using KEO_Baitest.Data.Entities;
using KEO_Baitest.Repository.Interfaces;
using KEO_Baitest.Services.Interfaces;
using KiemTraThuViec1.Data;

namespace KEO_Baitest.Services.Implements
{
    public class NhomThanhPhamService : GenericService<NhomThanhPham, NhomThanhPhamDTO>
    {
        public NhomThanhPhamService(INhomThanhPhamRepository repository, IUserService userService) : base(repository, userService)
        {

        }

        protected override NhomThanhPham? getEntityByDto(NhomThanhPhamDTO dto)
        {
            return _repository.Find(r => (r.IsDeleted == false) && r.MaNhomThanhPham.Equals(dto.MaNhomVatTu.Trim().ToUpper().Replace(" ", string.Empty)))
                .FirstOrDefault();
        }

        protected override NhomThanhPham? getEntityByMa(string ma)
        {
            return _repository.Find(r => (r.IsDeleted == false) && r.MaNhomThanhPham.Equals(ma.Trim().ToUpper().Replace(" ", string.Empty)))
                .FirstOrDefault();
        }

        protected override NhomThanhPhamDTO MapToDto(NhomThanhPham entity)
        {
            return new NhomThanhPhamDTO()
            {
                MaNhomVatTu = entity.MaNhomThanhPham,
                TenNhomVatTu = entity.Name
            };
        }

        protected override NhomThanhPham MapToEntity(NhomThanhPhamDTO dto)
        {
            return new NhomThanhPham()
            {
                MaNhomThanhPham = dto.MaNhomVatTu,
                Name = dto.TenNhomVatTu
            };
        }

        protected override NhomThanhPham? UpdateEntityF(NhomThanhPham entity, NhomThanhPhamDTO dto)
        {
            entity.Name = dto.TenNhomVatTu;
            return entity;
        }

        protected override ResponseDTO? ValidateDTO(NhomThanhPhamDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.MaNhomVatTu))
                return new ResponseDTO { Code = 400, Message = "Mã nhóm vật tư là null or only whitespace" };

            if (string.IsNullOrWhiteSpace(dto.TenNhomVatTu))
                return new ResponseDTO { Code = 400, Message = "Tên nhóm vật tư là null or only whitespace" };
            return null;
        }
    }
}
