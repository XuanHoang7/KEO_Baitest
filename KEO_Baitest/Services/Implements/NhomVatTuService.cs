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
            return _repository.Find(r => (r.IsDeleted == false) && r.MaNhomVatTu.Equals(dto.MaNhomVatTu.Trim().ToUpper().Replace(" ", string.Empty)))
                .FirstOrDefault();
        }

        protected override NhomVatTu? getEntityByMa(string ma)
        {
            return _repository.Find(r => (r.IsDeleted == false) && r.MaNhomVatTu.Equals(ma.Trim().ToUpper().Replace(" ", string.Empty)))
                .FirstOrDefault();
        }

        protected override NhomVatTuDTO MapToDto(NhomVatTu entity)
        {
            return new NhomVatTuDTO()
            {
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
            entity.Name = dto.TenNhomVatTu;
            return entity;
        }

        protected override ResponseDTO? ValidateDTO(NhomVatTuDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.MaNhomVatTu))
                return new ResponseDTO { Code = 400, Message = "Mã nhóm vật tư là null or only whitespace" };

            if (string.IsNullOrWhiteSpace(dto.TenNhomVatTu))
                return new ResponseDTO { Code = 400, Message = "Tên nhóm vật tư là null or only whitespace" };
            return null; // No errors
        }
    }
}
