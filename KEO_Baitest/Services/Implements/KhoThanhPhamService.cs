using KEO_Baitest.Data.DTOs;
using KEO_Baitest.Data.Entities;
using KEO_Baitest.Repository.Interfaces;
using KEO_Baitest.Services.Interfaces;
using KiemTraThuViec1.Data;
using Microsoft.AspNetCore.Server.IISIntegration;

namespace KEO_Baitest.Services.Implements
{
    public class KhoThanhPhamService : GenericService<KhoThanhPham, KhoThanhPhamDTO>
    {
        public KhoThanhPhamService(IKhoThanhPhamRepository repository, IUserService userService) : base(repository, userService)
        {
        }

        protected override KhoThanhPham? getEntityByDto(KhoThanhPhamDTO dto)
        {
            var result = _repository.GetById(dto.Id);
            return result.IsDeleted ? null : result;
        }

        protected override KhoThanhPham? getEntityByMa(string ma)
        {
            var result = _repository.GetById(ma);
            return result.IsDeleted ? null : result;
        }

        protected override KhoThanhPhamDTO MapToDto(KhoThanhPham entity)
        {
            return new KhoThanhPhamDTO()
            {
                Id = entity.Id.ToString(),
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
            entity.MaKhoThanhPham = dto.MaKhoThanhPham.Trim().ToUpper().Replace(" ", string.Empty);
            entity.Name = dto.TenKhoThanhPham;
            return entity;
        }

        protected override ResponseDTO? ValidateDTO(KhoThanhPhamDTO dto, bool isAdd = true)
        {
            if (string.IsNullOrWhiteSpace(dto.MaKhoThanhPham))
                return new ResponseDTO { Code = 400, Message = "Mã kho thành phẩm là null or only whitespace" };

            if (string.IsNullOrWhiteSpace(dto.TenKhoThanhPham))
                return new ResponseDTO { Code = 400, Message = "Tên kho thành phẩm là null or only whitespace" };
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
                    && r.MaKhoThanhPham.Equals(dto.MaKhoThanhPham) && !r.Id.Equals(entity.Id));
                    if (entityAnotherMa.Count != 0)
                    {
                        return new ResponseDTO { Code = 400, Message = "Mã này đã tồn tại" };
                    }
                }
            }
            else
            {
                var entity = _repository.Find(r => (r.IsDeleted == false) && r.MaKhoThanhPham.Equals(dto.MaKhoThanhPham.Trim().ToUpper().Replace(" ", string.Empty)))
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
