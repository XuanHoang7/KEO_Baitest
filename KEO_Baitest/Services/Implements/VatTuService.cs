using KEO_Baitest.Data.DTOs;
using KEO_Baitest.Data.Entities;
using KEO_Baitest.Repository.Interfaces;
using KEO_Baitest.Services.Interfaces;
using KiemTraThuViec1.Data;

namespace KEO_Baitest.Services.Implements
{
    public class VatTuService : GenericService<VatTu, VatTuDTO>
    {
        private readonly INhomVatTuRepository _nhomVatTuRepository;
        private readonly IDonViTinhRepository _donViTinhRepository;
        public VatTuService(IVatTuRepository repository, IUserService userService,
            INhomVatTuRepository nhomVatTuRepository,
            IDonViTinhRepository donViTinhRepository
            ) : base(repository, userService)
        {
            _nhomVatTuRepository = nhomVatTuRepository;
            _donViTinhRepository = donViTinhRepository;
        }

        protected override VatTu? getEntityByDto(VatTuDTO dto)
        {
            return _repository.Find(r => (r.IsDeleted == false) && r.MaKeToan.Equals(dto.MaKeToan.Trim().ToUpper().Replace(" ", string.Empty)))
                .FirstOrDefault();
        }

        protected override VatTu? getEntityByMa(string ma)
        {
            return _repository.Find(r => (r.IsDeleted == false) && r.MaKeToan.Equals(ma.Trim().ToUpper().Replace(" ", string.Empty)))
                .FirstOrDefault();
        }

        protected override VatTuDTO MapToDto(VatTu entity)
        {
            var donViTinh = _donViTinhRepository
                    .Find(r => (r.IsDeleted == false) && r.Id.Equals(entity.DonViTinhId))
                    .FirstOrDefault();
            var nhomVatTu = _nhomVatTuRepository
                    .Find(r => (r.IsDeleted == false) && r.Id.Equals(entity.NhomVatTuId))
                    .FirstOrDefault();
            return new VatTuDTO()
            {
                MaKeToan = entity.MaKeToan,
                MaKyThuat = entity.MaKyThuat,
                TenVatTu = entity.Name,
                MaDonViTinh = donViTinh != null ? donViTinh.MaDonViTinh : "",
                MaNhomVatTu = nhomVatTu != null ? nhomVatTu.MaNhomVatTu : "",
                NameDonViTinh = donViTinh != null ? donViTinh.Name : "",
                NameNhomVatTu = nhomVatTu != null ? nhomVatTu.Name : ""
            };
        }

        protected override VatTu MapToEntity(VatTuDTO dto)
        {
            var donViTinh = _donViTinhRepository.GetDonViTinhByMa(dto.MaDonViTinh);
            var nhomVatTu = _nhomVatTuRepository.GetNhomVatTuByMa(dto.MaNhomVatTu);
            return new VatTu()
            {
                MaKyThuat = dto.MaKyThuat,
                MaKeToan = dto.MaKeToan,
                Name = dto.TenVatTu,
                DonViTinhId = donViTinh != null ? donViTinh.Id : Guid.Empty,
                NhomVatTuId = nhomVatTu != null ? nhomVatTu.Id : Guid.Empty 
            };
        }

        protected override VatTu? UpdateEntityF(VatTu entity, VatTuDTO dto)
        {
            var donViTinh = _donViTinhRepository.GetDonViTinhByMa(dto.MaDonViTinh);
            var nhomVatTu = _nhomVatTuRepository.GetNhomVatTuByMa(dto.MaNhomVatTu);
            entity.Name = dto.TenVatTu;
            entity.MaKyThuat = dto.MaKyThuat;
            entity.DonViTinhId = donViTinh != null ? donViTinh.Id : Guid.Empty;
            entity.NhomVatTuId = nhomVatTu != null ? nhomVatTu.Id : Guid.Empty;
            return entity;
        }

        protected override ResponseDTO? ValidateDTO(VatTuDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.MaKyThuat))
                return new ResponseDTO { Code = 400, Message = "Mã kỹ thuật là null or only whitespace" };

            if (string.IsNullOrWhiteSpace(dto.MaKeToan))
                return new ResponseDTO { Code = 400, Message = "Mã kế toán là null or only whitespace" };

            if (string.IsNullOrWhiteSpace(dto.TenVatTu))
                return new ResponseDTO { Code = 400, Message = "Tên vật tư là null or only whitespace" };

            if (_donViTinhRepository.GetDonViTinhByMa(dto.MaDonViTinh) == null)
                return new ResponseDTO { Code = 400, Message = "Mã đơn vị tính không tồn tại" };

            if (_nhomVatTuRepository.GetNhomVatTuByMa(dto.MaNhomVatTu) == null)
                return new ResponseDTO { Code = 400, Message = "Mã nhóm vật tư không tồn tại" };

            return null;
        }
    }
}
