using KEO_Baitest.Data.DTOs;
using KEO_Baitest.Data.Entities;
using KEO_Baitest.Repository.Implements;
using KEO_Baitest.Repository.Interfaces;
using KEO_Baitest.Services.Interfaces;
using KiemTraThuViec1.Data;

namespace KEO_Baitest.Services.Implements
{
    public class QRThanhPhamService : IQRThanhPhamService
    {
        private readonly IQRThanhPhamRepository _repository;
        private readonly IUserService _userService;
        private readonly IThanhPhamRepository _thanhPhamRepository;

        public QRThanhPhamService(IQRThanhPhamRepository repository,
            IUserService userService,
            IThanhPhamRepository thanhPhamRepository)
        {
            _repository = repository;
            _userService = userService;
            _thanhPhamRepository = thanhPhamRepository;
        }
        public ResponseGetDTO<QRThanhPhamO> GetAll(int page, string keyword)
        {
            int pageSize = 20;
            var entities = _repository.Find(r => (r.IsDeleted == false))
                .Select(e => MapToDto(e))
                .ToList();
            int totalRow = entities.Count();
            int totalPage = (int)Math.Ceiling((double)totalRow / pageSize);

            if(entities != null)
            {
                entities = entities
                .Where(r => r.TenThanhPham.Contains(keyword))
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            }
            return new ResponseGetDTO<QRThanhPhamO>
            {
                TotalRow = totalRow,
                TotalPage = totalPage,
                PageSize = pageSize,
                Datalist = entities
            };
        }

        private QRThanhPhamO? MapToDto(QRThanhPham entity)
        {
            ThanhPham? v = _thanhPhamRepository.Find(r => (r.IsDeleted == false) && r.Id.Equals(entity.ThanhPhamId)).FirstOrDefault();
            if(v!= null)
            {
                return new QRThanhPhamO()
                {
                    Id = entity.Id,
                    MaKeToan = v.MaKeToan,
                    TenThanhPham = v.Name,
                    MaQR = entity.QRCode,
                };
            }
            return null;
        }

        public ResponseDTO Update(UpdateQRThanhPhamDTO dto)
        {
            string? userId = _userService.GetCurrentUser();
            if (userId == null)
                return new ResponseDTO { Code = 400, Message = "User not exists" };

            //var entityExists = _repository.Find(r => ((dynamic)r).Ma.Equals(((dynamic)dto).Ma.Trim().ToUpper().Replace(" ", string.Empty)))
            //    .FirstOrDefault();

            if (Guid.TryParse(dto.Id, out Guid guid))
            {
                var entityExists = _repository.Find(r => r.Id.Equals(guid)).FirstOrDefault();
                if (entityExists != null)
                {
                    entityExists.UpdateBy = userId;
                    entityExists.UpdateDate = DateTime.Now;
                    entityExists.SoLot = dto.Solot;
                    _repository.Update(entityExists);
                    if (_repository.IsSaveChange())
                        return new ResponseDTO()
                        {
                            Code = 200,
                            Message = "Success",
                            Description = null
                        };

                }
            }
            return new ResponseDTO()
            {
                Code = 400,
                Message = "Entity not found",
                Description = null
            };
        }

        public ResponseDTO Add(QRThanhPhamE dto)
        {
            string qRCode = dto.MaKeToan;
            if (dto.SoLot != null)
            {
                qRCode = qRCode + "&" + dto.SoLot;
            }
            var errorResponse = ValidateDTO(dto);
            if (errorResponse != null) return errorResponse;
            string? userId = _userService.GetCurrentUser();
            if (userId == null)
                return new ResponseDTO { Code = 400, Message = "User not exists" };
            var entityExists = _repository
                .Find(r => (r.IsDeleted == false) && r.QRCode.Equals(qRCode)
                ).FirstOrDefault();
            if (entityExists == null)
            {
                QRThanhPham? entity = MapToEntity(dto, qRCode);
                if (entity != null)
                {
                    entity.CreateBy = userId;
                    entity.CreateDate = DateTime.Now;

                    _repository.Add(entity);
                    if (_repository.IsSaveChange())
                        return new ResponseDTO()
                        {
                            Code = 200,
                            Message = "Success",
                            Description = null
                        };
                }
                else
                {
                    return new ResponseDTO()
                    {
                        Code = 400,
                        Message = "Mã kế toán không tồn tại",
                        Description = null
                    };
                }
            }
            return new ResponseDTO()
            {
                Code = 400,
                Message = "Entity already exists",
                Description = null
            };
        }

        public ResponseDTO Delete(string Id)
        {
            Guid guidId;
            if (Guid.TryParse(Id, out guidId))
            {
                string? userId = _userService.GetCurrentUser();
                if (userId == null)
                    return new ResponseDTO { Code = 400, Message = "User not exists" };

                var entityExists = _repository.Find(r => (r.IsDeleted == false) && r.Id.Equals(guidId)).FirstOrDefault();

                if (entityExists != null)
                {
                    entityExists.IsDeleted = true;
                    entityExists.DeleteBy = userId;
                    entityExists.DeleteDate = DateTime.Now;
                    _repository.Update(entityExists);
                    if (_repository.IsSaveChange())
                        return new ResponseDTO()
                        {
                            Code = 200,
                            Message = "Success",
                            Description = null
                        };
                }
                return new ResponseDTO()
                {
                    Code = 400,
                    Message = "Entity not found",
                    Description = null
                };
            }
            return new ResponseDTO()
            {
                Code = 400,
                Message = "Invalid identifier",
                Description = null
            };
        }

        private QRThanhPham? MapToEntity(QRThanhPhamE? q, string qRCode)
        {
            ThanhPham? vt = _thanhPhamRepository.Find(r => r.MaKeToan.Equals(q.MaKeToan)).FirstOrDefault();
            if (vt != null)
            {
                return new QRThanhPham()
                {
                    ThanhPhamId = vt.Id,
                    SoLot = q == null ? null : q.SoLot,
                    QRCode = qRCode
                };
            }
            return null;
        }
        private ResponseDTO? ValidateDTO(QRThanhPhamE dto)
        {

            if (string.IsNullOrWhiteSpace(dto.MaKeToan))
                return new ResponseDTO { Code = 400, Message = "Mã vật tư là null or only whitespace" };

            //if (_nhaCungCapRepository.Find(r => r.MaNhaCungCap.Equals(dto))?.Id ?? Guid.Empty;)
            return null;
        }

        public QRThanhPham? GetByQRCode(string qr)
        {
            return _repository.Find(r => (r.IsDeleted == false) && r.QRCode.Equals(qr)).FirstOrDefault();
        }
    }
}
