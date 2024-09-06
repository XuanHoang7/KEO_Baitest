using KEO_Baitest.Data.DTOs;
using KEO_Baitest.Data.Entities;
using KEO_Baitest.Repository.Interfaces;
using KEO_Baitest.Services.Interfaces;
using KiemTraThuViec1.Data;

namespace KEO_Baitest.Services.Implements
{
    public class NhomVatTuService : GenericService<NhomVatTu, NhomVatTuDTO>
    {
        public NhomVatTuService(INhomVatTuRepository repository, IUserService userService) : base(repository, userService)
        {
        }

        protected override NhomVatTu? getEntityByDto(NhomVatTuDTO dto)
        {
            var result = _repository.GetById(dto.Id);
            return result.IsDeleted ? null : result;
        }

        protected override NhomVatTu? getEntityByMa(string ma)
        {
            var result = _repository.GetById(ma);
            return result.IsDeleted ? null : result;
        }

        protected override NhomVatTuDTO MapToDto(NhomVatTu entity)
        {
            return new NhomVatTuDTO()
            {
                Id = entity.Id.ToString(),
                MaNhomVatTu = entity.MaNhomVatTu,
                TenNhomVatTu = entity.Name
            };
        }

        protected override NhomVatTu MapToEntity(NhomVatTuDTO dto)
        {
            return new NhomVatTu()
            {
                MaNhomVatTu = dto.MaNhomVatTu,
                Name = dto.TenNhomVatTu
            };
        }

        protected override NhomVatTu? UpdateEntityF(NhomVatTu entity, NhomVatTuDTO dto)
        {
            entity.MaNhomVatTu = dto.MaNhomVatTu.Trim().ToUpper().Replace(" ", string.Empty);
            entity.Name = dto.TenNhomVatTu;
            return entity;
        }

        protected override ResponseDTO? ValidateDTO(NhomVatTuDTO dto, bool isAdd = true)
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
                    && r.MaNhomVatTu.Equals(dto.MaNhomVatTu) && !r.Id.Equals(entity.Id));
                    if (entityAnotherMa.Count != 0)
                    {
                        return new ResponseDTO { Code = 400, Message = "Mã này đã tồn tại" };
                    }
                }
            }
            else
            {
                var entity = _repository.Find(r => (r.IsDeleted == false) && r.MaNhomVatTu.Equals(dto.MaNhomVatTu.Trim().ToUpper().Replace(" ", string.Empty)))
                .FirstOrDefault();
                if (entity != null)
                {
                    return new ResponseDTO { Code = 400, Message = "Mã này đã tồn tại" };
                }
            }
            return null; // No errors
        }
    }
}
