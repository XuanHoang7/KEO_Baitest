using KEO_Baitest.Data.DTOs;
using KEO_Baitest.Data.Entities;
using KEO_Baitest.Repository.Interfaces;
using KEO_Baitest.Services.Interfaces;
using KiemTraThuViec1.Data;

namespace KEO_Baitest.Services.Implements
{
    public class DonViTinhService : GenericService<DonViTinh, DonViTinhDTO>
    {
        public DonViTinhService(IDonViTinhRepository repository, IUserService userService) : base(repository, userService)
        {
        }

        protected override DonViTinh? getEntityByDto(DonViTinhDTO dto)
        {
            return _repository.Find(r => (r.IsDeleted == false) && r.MaDonViTinh.Equals(dto.MaDonViTinh.Trim().ToUpper().Replace(" ", string.Empty)))
                .FirstOrDefault();
        }

        protected override DonViTinh? getEntityByMa(string ma)
        {
            return _repository.Find(r => (r.IsDeleted == false) && r.MaDonViTinh.Equals(ma.Trim().ToUpper().Replace(" ", string.Empty)))
                .FirstOrDefault();
        }

        protected override DonViTinhDTO MapToDto(DonViTinh entity)
        {
            return new DonViTinhDTO()
            {
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
            entity.Name = dto.TenDonViTinh;
            return entity;
        }

        protected override ResponseDTO? ValidateDTO(DonViTinhDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.MaDonViTinh))
                return new ResponseDTO { Code = 400, Message = "Mã đơn vị tính là null or only whitespace" };

            if (string.IsNullOrWhiteSpace(dto.TenDonViTinh))
                return new ResponseDTO { Code = 400, Message = "Tên đơn vị tính là null or only whitespace" };
            return null; // No errors
        }
    }
}
