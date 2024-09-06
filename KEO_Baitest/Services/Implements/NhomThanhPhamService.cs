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
            var result = _repository.GetById(dto.Id);
            return result.IsDeleted ? null : result;
        }

        protected override NhomThanhPham? getEntityByMa(string ma)
        {
            var result = _repository.GetById(ma);
            return result.IsDeleted ? null : result;
        }

        protected override NhomThanhPhamDTO MapToDto(NhomThanhPham entity)
        {
            return new NhomThanhPhamDTO()
            {
                Id = entity.Id.ToString(),
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
            entity.MaNhomThanhPham = dto.MaNhomVatTu.Trim().ToUpper().Replace(" ", string.Empty);
            entity.Name = dto.TenNhomVatTu;
            return entity;
        }

        protected override ResponseDTO? ValidateDTO(NhomThanhPhamDTO dto, bool isAdd = true)
        {
            if (string.IsNullOrWhiteSpace(dto.MaNhomVatTu))
                return new ResponseDTO { Code = 400, Message = "Mã nhóm vật tư là null or only whitespace" };

            if (string.IsNullOrWhiteSpace(dto.TenNhomVatTu))
                return new ResponseDTO { Code = 400, Message = "Tên nhóm vật tư là null or only whitespace" };
            if (!isAdd)
            {
                if (string.IsNullOrWhiteSpace(dto.Id))
                    return new ResponseDTO { Code = 400, Message = "Id là null or only whitespace" };
                var entity = _repository.GetById(dto.Id!);
                if (entity?.IsDeleted != false)
                {
                    return new ResponseDTO { Code = 400, Message = "Id không exists" };
                }
                else
                {
                    var entityAnotherMa = _repository.Find(r => (r.IsDeleted == false)
                    && r.MaNhomThanhPham.Equals(dto.MaNhomVatTu) && !r.Id.Equals(entity.Id));
                    if (entityAnotherMa.Count != 0)
                    {
                        return new ResponseDTO { Code = 400, Message = "Mã này đã tồn tại" };
                    }
                }
            }
            else
            {
                var entity = _repository.Find(r => (r.IsDeleted == false) && r.MaNhomThanhPham.Equals(dto.MaNhomVatTu.Trim().ToUpper().Replace(" ", string.Empty)))
                .FirstOrDefault();
                if (entity != null)
                {
                    return new ResponseDTO { Code = 400, Message = "Mã này đã tồn tại" };
                }
            }
            return null;
        }
    }
}
