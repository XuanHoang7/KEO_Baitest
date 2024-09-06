using KEO_Baitest.Data.DTOs;
using KEO_Baitest.Data.Entities;
using KEO_Baitest.Repository.Implements;
using KEO_Baitest.Repository.Interfaces;
using KEO_Baitest.Services.Interfaces;
using KiemTraThuViec1.Data;

namespace KEO_Baitest.Services.Implements
{
    public class ThanhPhamService : GenericService<ThanhPham, ThanhPhamDTO>
    {
        private readonly INhomThanhPhamRepository _nhomThanhPhamRepository;
        private readonly IDonViTinhRepository _donViTinhRepository;
        public ThanhPhamService(IThanhPhamRepository repository, IUserService userService,
            INhomThanhPhamRepository nhomThanhPhamRepository,
            IDonViTinhRepository donViTinhRepository
            ) : base(repository, userService)
        {
            _nhomThanhPhamRepository = nhomThanhPhamRepository;
            _donViTinhRepository = donViTinhRepository;
        }

        protected override ThanhPham? getEntityByDto(ThanhPhamDTO dto)
        {
            var result = _repository.GetById(dto.Id);
            return result.IsDeleted ? null : result;
        }

        protected override ThanhPham? getEntityByMa(string ma)
        {
            var result = _repository.GetById(ma);
            return result.IsDeleted ? null : result;
        }

        protected override ThanhPhamDTO MapToDto(ThanhPham entity)
        {

            var donViTinh = _donViTinhRepository
                    .Find(r => (r.IsDeleted == false) && r.Id.Equals(entity.DonViTinhId))
                    .FirstOrDefault();
            var nhomThanhPham = _nhomThanhPhamRepository
                    .Find(r => (r.IsDeleted == false) && r.Id.Equals(entity.NhomThanhPhamId))
                    .FirstOrDefault();
            return new ThanhPhamDTO()
            {
                Id = entity.Id.ToString(),
                MaKeToan = entity.MaKeToan,
                MaKyThuat = entity.MaThanhPham,
                TenThanhPham = entity.Name,
                MaDonViTinh = donViTinh != null ? donViTinh.MaDonViTinh : "",
                MaNhomThanhPham = nhomThanhPham != null ? nhomThanhPham.MaNhomThanhPham : "",
                NameDonViTinh = donViTinh != null ? donViTinh.Name : "",
                TenNhomThanhPham = nhomThanhPham != null ? nhomThanhPham.Name : ""
            };
        }

        protected override ThanhPham MapToEntity(ThanhPhamDTO dto)
        {
            var donViTinh = _donViTinhRepository.GetDonViTinhByMa(dto.MaDonViTinh);
            var nhomThanhPham = _nhomThanhPhamRepository.GetNhomThanhPhamByMa(dto.MaNhomThanhPham);
            return new ThanhPham()
            {
                MaThanhPham = dto.MaKyThuat,
                MaKeToan = dto.MaKeToan,
                Name = dto.TenThanhPham,
                DonViTinhId = donViTinh != null ? donViTinh.Id : Guid.Empty,
                NhomThanhPhamId = nhomThanhPham != null ? nhomThanhPham.Id : Guid.Empty,
            };
        }

        protected override ThanhPham? UpdateEntityF(ThanhPham entity, ThanhPhamDTO dto)
        {
            var donViTinh = _donViTinhRepository.GetDonViTinhByMa(dto.MaDonViTinh);
            var nhomVatTu = _nhomThanhPhamRepository.GetNhomThanhPhamByMa(dto.MaNhomThanhPham);
            entity.Name = dto.TenThanhPham;
            entity.MaThanhPham = dto.MaKyThuat;
            entity.MaKeToan = dto.MaKeToan.Trim().ToUpper().Replace(" ", string.Empty);
            entity.DonViTinhId = donViTinh != null ? donViTinh.Id : Guid.Empty;
            entity.NhomThanhPhamId = nhomVatTu != null ? nhomVatTu.Id : Guid.Empty;
            return entity;
        }

        protected override ResponseDTO? ValidateDTO(ThanhPhamDTO dto, bool isAdd = true)
        {
            if (string.IsNullOrWhiteSpace(dto.MaKyThuat))
                return new ResponseDTO { Code = 400, Message = "Mã kỹ thuật là null or only whitespace" };

            if (string.IsNullOrWhiteSpace(dto.MaKeToan))
                return new ResponseDTO { Code = 400, Message = "Mã kế toán là null or only whitespace" };

            if (string.IsNullOrWhiteSpace(dto.TenThanhPham))
                return new ResponseDTO { Code = 400, Message = "Tên thành phẩm là null or only whitespace" };

            if (_donViTinhRepository.GetDonViTinhByMa(dto.MaDonViTinh) == null)
                return new ResponseDTO { Code = 400, Message = "Mã đơn vị tính không tồn tại" };

            if (_nhomThanhPhamRepository.GetNhomThanhPhamByMa(dto.MaNhomThanhPham) == null)
                return new ResponseDTO { Code = 400, Message = "Mã nhóm thành phẩm không tồn tại" };
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
                    && r.MaKeToan.Equals(dto.MaKeToan) && !r.Id.Equals(entity.Id));
                    if (entityAnotherMa.Count != 0)
                    {
                        return new ResponseDTO { Code = 400, Message = "Mã này đã tồn tại" };
                    }
                }
            }
            else
            {
                var entity = _repository.Find(r => (r.IsDeleted == false) && r.MaKeToan.Equals(dto.MaKeToan.Trim().ToUpper().Replace(" ", string.Empty)))
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
