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
            return _repository.Find(r => (r.IsDeleted == false) && r.MaNhaKhoVatTu.Equals(dto.MaKhoVatTu.Trim().ToUpper().Replace(" ", string.Empty)))
                .FirstOrDefault();
        }

        protected override KhoVatTu? getEntityByMa(string ma)
        {
            return _repository.Find(r => (r.IsDeleted == false) && r.MaNhaKhoVatTu.Equals(ma.Trim().ToUpper().Replace(" ", string.Empty)))
                .FirstOrDefault();
        }

        protected override KhoVatTuDTO MapToDto(KhoVatTu entity)
        {
            return new KhoVatTuDTO()
            {
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
            entity.Name = dto.TenKhoVatTu;
            return entity;
        }

        protected override ResponseDTO? ValidateDTO(KhoVatTuDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.MaKhoVatTu))
                return new ResponseDTO { Code = 400, Message = "Mã kho vật tư là null or only whitespace" };

            if (string.IsNullOrWhiteSpace(dto.TenKhoVatTu))
                return new ResponseDTO { Code = 400, Message = "Tên kho vật tư là null or only whitespace" };
            return null;
        }
    }
}
