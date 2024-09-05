using KEO_Baitest.Data.DTOs;
using KEO_Baitest.Data.Entities;
using KEO_Baitest.Repository.Interfaces;
using KEO_Baitest.Services.Interfaces;
using KiemTraThuViec1.Data;
using Microsoft.IdentityModel.Tokens;
using System;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace KEO_Baitest.Services.Implements
{
    public class QRVatTuService : IQRVatTuService
    {
        private readonly IQRVatTuRepository _repository;
        private readonly IUserService _userService;
        private readonly INhaCungCapRepository _nhaCungCapRepository;
        private readonly IVatTuRepository _vatTuRepository;

        public QRVatTuService(IQRVatTuRepository repository,
            IUserService userService,
            INhaCungCapRepository nhaCungCapRepository,
            IVatTuRepository vatTuRepository)
        {
            _repository = repository;
            _userService = userService;
            _nhaCungCapRepository = nhaCungCapRepository;
            _vatTuRepository = vatTuRepository;
        }
        public ResponseGetDTO<QRVatTuO> GetAll(int page, string keyword)
        {
            int pageSize = 20;
            var entities = _repository.Find(r => (r.IsDeleted == false))
                .Select(e => MapToDto(e))
                .ToList();
            int totalRow = entities.Count();
            int totalPage = (int)Math.Ceiling((double)totalRow / pageSize);

            if (entities != null)
            {
                entities = entities
                .Where(r => r.TenVatTu.Contains(keyword))
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            }
            return new ResponseGetDTO<QRVatTuO>
            {
                TotalRow = totalRow,
                TotalPage = totalPage,
                PageSize = pageSize,
                Datalist = entities
            };
        }

        private QRVatTuO MapToDto(QRVatTu entity)
        {
            VatTu? v = _vatTuRepository.Find(r => (r.IsDeleted == false) && r.Id.Equals(entity.VatTuId)).FirstOrDefault();
            NhaCungCap? n = _nhaCungCapRepository
                    .Find(r => (r.IsDeleted == false) && r.Id
                    .Equals(entity.NhaCungCapId)).FirstOrDefault();
            return new QRVatTuO()
            {
                Id = entity.Id,
                MaKeToan = v.MaKeToan,
                TenVatTu = v.Name,
                MaQR = entity.QRCode,
                MaNhaCungCap = n?.MaNhaCungCap
            };
        }

        public ResponseDTO Update(UpdateQRVatTuDTO dto)
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
                    var nhaCungCap =  _nhaCungCapRepository.Find(r => r.MaNhaCungCap.Equals(dto.MaNCC))?.FirstOrDefault();
                    if(!string.IsNullOrEmpty(dto.MaNCC) && nhaCungCap == null)
                        return new ResponseDTO()
                        {
                            Code = 400,
                            Message = "Mã nhà cung cấp không tồn tại",
                            Description = null
                        };
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

            public ResponseDTO Add(QRVatTuE dto)
            {
                string qRCode = dto.MaKeToan;
                if (dto.SoLot != null)
                {
                    qRCode = qRCode + "&" + dto.SoLot;
                }
                if (dto.MaNhaCungCap != null)
                {
                    qRCode = qRCode + "&" + dto.MaNhaCungCap;
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
                    QRVatTu? entity = MapToEntity(dto, qRCode);
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

            private QRVatTu? MapToEntity(QRVatTuE q, string qRCode)
            {
                VatTu? vt = _vatTuRepository.Find(r => r.MaKeToan.Equals(q.MaKeToan)).FirstOrDefault();
                NhaCungCap? ncc = _nhaCungCapRepository
                        .Find(r => (r.IsDeleted == false) && r.MaNhaCungCap
                        .Equals(q.MaNhaCungCap)).FirstOrDefault();
                if (vt != null)
                {
                    return new QRVatTu()
                    {
                        VatTuId = vt.Id,
                        NhaCungCapId = ncc?.Id,
                        SoLot = q.SoLot,
                        QRCode = qRCode
                    };
                }
                return null;
            }
            private ResponseDTO? ValidateDTO(QRVatTuE dto)
            {

                if (string.IsNullOrWhiteSpace(dto.MaKeToan))
                    return new ResponseDTO { Code = 400, Message = "Mã vật tư là null or only whitespace" };

                //if (_nhaCungCapRepository.Find(r => r.MaNhaCungCap.Equals(dto))?.Id ?? Guid.Empty;)
                return null;
            }

            public QRVatTu? GetByQRCode(string qr)
            {
                return _repository.Find(r => (r.IsDeleted == false) && r.QRCode.Equals(qr)).FirstOrDefault();
            }
        }
    }
