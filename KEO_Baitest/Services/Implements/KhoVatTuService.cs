using KEO_Baitest.Data.DTOs;
using KEO_Baitest.Data.Entities;
using KEO_Baitest.Repository.Interfaces;
using KEO_Baitest.Services.Interfaces;
using KiemTraThuViec1.Data;

namespace KEO_Baitest.Services.Implements
{
    public class KhoVatTuService : GenericService<KhoVatTu, KhoVatTuDTO>
    {
        public KhoVatTuService(IKhoVatTuRepository repository, IUserService userService) : base(repository, userService)
        {
        }

        protected override KhoVatTu? getEntityByDto(KhoVatTuDTO dto)
        {
            var result = _repository.GetById(dto.Id);
            return result.IsDeleted ? null : result;
        }

        protected override KhoVatTu? getEntityByMa(string ma)
        {
            var result = _repository.GetById(ma);
            return result.IsDeleted ? null : result;
        }

        protected override KhoVatTuDTO MapToDto(KhoVatTu entity)
        {
            return new KhoVatTuDTO()
            {
                Id = entity.Id.ToString(),
                MaKhoVatTu = entity.MaNhaKhoVatTu,
                TenKhoVatTu = entity.Name
            };
        }

        protected override KhoVatTu MapToEntity(KhoVatTuDTO dto)
        {
            return new KhoVatTu()
            {
                MaNhaKhoVatTu = dto.MaKhoVatTu,
                Name = dto.TenKhoVatTu
            };
        }

        protected override KhoVatTu? UpdateEntityF(KhoVatTu entity, KhoVatTuDTO dto)
        {
            entity.MaNhaKhoVatTu = dto.MaKhoVatTu.Trim().ToUpper().Replace(" ", string.Empty);
            entity.Name = dto.TenKhoVatTu;
            return entity;
        }

        protected override ResponseDTO? ValidateDTO(KhoVatTuDTO dto, bool isAdd = true)
        {
            if (string.IsNullOrWhiteSpace(dto.MaKhoVatTu))
                return new ResponseDTO { Code = 400, Message = "Mã kho vật tư là null or only whitespace" };

            if (string.IsNullOrWhiteSpace(dto.TenKhoVatTu))
                return new ResponseDTO { Code = 400, Message = "Tên kho vật tư là null or only whitespace" };
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
                    && r.MaNhaKhoVatTu.Equals(dto.MaKhoVatTu) && !r.Id.Equals(entity.Id));
                    if (entityAnotherMa.Count != 0)
                    {
                        return new ResponseDTO { Code = 400, Message = "Mã này đã tồn tại" };
                    }
                }
            }
            else
            {
                var entity = _repository.Find(r => (r.IsDeleted == false) && r.MaNhaKhoVatTu.Equals(dto.MaKhoVatTu.Trim().ToUpper().Replace(" ", string.Empty)))
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
