using KEO_Baitest.Data.DTOs;
using KEO_Baitest.Data.Entities;
using KEO_Baitest.Repository.Interfaces;
using KEO_Baitest.Services.Interfaces;
using KiemTraThuViec1.Data;
using System.Net.WebSockets;

namespace KEO_Baitest.Services.Implements
{
    public class DonViTinhService : GenericService<DonViTinh, DonViTinhDTO>
    {
        public DonViTinhService(IDonViTinhRepository repository, IUserService userService) : base(repository, userService)
        {
        }

        protected override DonViTinh? getEntityByDto(DonViTinhDTO dto)
        {
            var result = _repository.GetById(dto.Id);
            return result.IsDeleted ? null : result;

        }

        protected override DonViTinh? getEntityByMa(string ma)
        {
            var result = _repository.GetById(ma);
            return  result.IsDeleted ? null : result;
        }

        protected override DonViTinhDTO MapToDto(DonViTinh entity)
        {
            return new DonViTinhDTO()
            {
                Id = entity.Id.ToString(),
                MaDonViTinh = entity.MaDonViTinh,
                TenDonViTinh = entity.Name
            };
        }

        protected override DonViTinh MapToEntity(DonViTinhDTO dto)
        {
            return new DonViTinh()
            {
                MaDonViTinh = dto.MaDonViTinh,
                Name = dto.TenDonViTinh
            };
        }

        protected override DonViTinh? UpdateEntityF(DonViTinh entity, DonViTinhDTO dto)
        {
            entity.MaDonViTinh = dto.MaDonViTinh.Trim().ToUpper().Replace(" ", string.Empty);
            entity.Name = dto.TenDonViTinh;
            return entity;
        }

        protected override ResponseDTO? ValidateDTO(DonViTinhDTO dto, bool isADD = true)
        {
            if (string.IsNullOrWhiteSpace(dto.MaDonViTinh))
                return new ResponseDTO { Code = 400, Message = "Mã đơn vị tính là null or only whitespace" };
            if (string.IsNullOrWhiteSpace(dto.TenDonViTinh))
                return new ResponseDTO { Code = 400, Message = "Tên đơn vị tính là null or only whitespace" };
            if (!isADD)
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
                    var entityAnotherMa = _repository.Find(r => !r.IsDeleted
                        && r.MaDonViTinh == dto.MaDonViTinh
                        && r.Id != entity.Id);
                    if (entityAnotherMa.Count != 0)
                    {
                        return new ResponseDTO { Code = 400, Message = "Mã này đã tồn tại" };
                    }
                }
            }
            else
            {
                var entity = _repository.Find(r => (r.IsDeleted == false) && r.MaDonViTinh.Equals(dto.MaDonViTinh.Trim().ToUpper().Replace(" ", string.Empty)))
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
