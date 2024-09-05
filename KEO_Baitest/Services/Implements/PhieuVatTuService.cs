using KEO_Baitest.Data.DTOs;
using KEO_Baitest.Data.Entities;
using KEO_Baitest.Repository.Implements;
using KEO_Baitest.Repository.Interfaces;
using KEO_Baitest.Services.Interfaces;
using KiemTraThuViec1.Data;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Eventing.Reader;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace KEO_Baitest.Services.Implements
{
    public class PhieuVatTuService : IPhieuVatTuService
    {
        private readonly IUserService _userService;
        private readonly IPhieuVatTuDetailRepository _phieuVatTuDetailRepository;
        private readonly IPhieuVatTuRepository _phieuVatTuRepository;
        private readonly INhaCungCapRepository _nhaCungCapRepository;
        private readonly IKhachHangRepository _khachHangRepository;
        private readonly IVatTuRepository _VatTuRepository;
        private readonly IKhoVatTuRepository _khoVatTuRepository;
        private readonly IDonViTinhRepository _donViTinhRepository;
        private readonly IQRVatTuService _qRVatTuservice;
        private readonly INhomVatTuRepository _nhomVatTuRepository;
        public PhieuVatTuService(IPhieuVatTuDetailRepository phieuVatTuDetailRepository,
            IUserService userService,
            IPhieuVatTuRepository phieuVatTuRepository,
            INhaCungCapRepository nhaCungCapRepository,
            IVatTuRepository VatTuRepository,
            IKhoVatTuRepository khoVatTuRepository,
            IDonViTinhRepository donViTinhRepository,
            IQRVatTuService qRVatTuService,
            IKhachHangRepository khachHangRepository,
            INhomVatTuRepository nhomVatTuRepository)
        {
            _userService = userService;
            _phieuVatTuDetailRepository = phieuVatTuDetailRepository;
            _phieuVatTuRepository = phieuVatTuRepository;
            _nhaCungCapRepository = nhaCungCapRepository;
            _VatTuRepository = VatTuRepository;
            _khoVatTuRepository = khoVatTuRepository;
            _donViTinhRepository = donViTinhRepository;
            _qRVatTuservice = qRVatTuService;
            _khachHangRepository = khachHangRepository;
            _nhomVatTuRepository = nhomVatTuRepository;
        }


        private VatTu GetVatTu(string maVatTu)
        {
            string maketoan = maVatTu.Trim().ToUpper().Replace(" ", string.Empty);
            return _VatTuRepository.Find(r => (r.IsDeleted == false) && r.MaKeToan.Equals(maketoan)).FirstOrDefault() ??
                   throw new Exception("Mã kế toán không tồn tại");
        }
        private PhieuVatTu CreateNewPhieuVatTu(PhieuVatTuDTO phieuVatTuDTO, string userId, bool status)
        {
            DateTime dateTime = DateTime.Now;
            string result = $"NKVT-{dateTime:ddMMyyyy}";
            if (!status)
            {
                result = $"XKVT-{dateTime:ddMMyyyy}";
            }

            var phieuVatTu = new PhieuVatTu
            {
                MaPhieu = result,
                KhoVatTuId = _khoVatTuRepository.GetKhoVatTuByMa(phieuVatTuDTO.MaKho)?.Id
                    ?? throw new Exception("Kho Thành phẩm not exists"),
                ImportOrExport = phieuVatTuDTO.IsNhapKho,
                Status = status,
                CreateBy = userId,
                CreateDate = DateTime.Now
            };

            _phieuVatTuRepository.Add(phieuVatTu);
            if (!_phieuVatTuRepository.IsSaveChange())
                throw new Exception("Phieu thành phẩm not save");

            return phieuVatTu;
        }

        private ResponseDTO AddPVTDetail(PhieuVatTuDTO phieuVatTuDTO, string userId, Guid phieuVatTuId)
        {
            try
            {
                QRVatTu? q = _qRVatTuservice.GetByQRCode(phieuVatTuDTO.QRCode);

                // Gán giá trị cho các biến
                if (q != null)
                {
                    var p = new PhieuVatTuDetail
                    {
                        PhieuVatTuId = phieuVatTuId,
                        VatTuId = q.VatTuId,
                        SoLuong = phieuVatTuDTO.SoLuong,
                        SoLot = q.SoLot,
                        NhaCungCapID = q.NhaCungCapId,
                        ThoiGian = DateTime.Now,
                        CreateBy = userId,
                        CreateDate = DateTime.Now,
                        QRCode = q.QRCode
                    };
                    _phieuVatTuDetailRepository.Add(p);
                    if (_phieuVatTuDetailRepository.IsSaveChange())
                    {
                        var phieuVatTu = _phieuVatTuRepository
                                .Find(d => !d.IsDeleted && d.Id == p.PhieuVatTuId)
                                .FirstOrDefault();

                        if (phieuVatTu != null && phieuVatTu.Status == true)
                        {
                            phieuVatTu.Status = false;
                            _phieuVatTuRepository.Update(phieuVatTu);
                            _phieuVatTuRepository.IsSaveChange();
                        }
                        return new ResponseDTO { Code = 200, Message = "Success", Description = p };
                    }
                }
                else
                {
                    return new ResponseDTO { Code = 400, Message = "QRCode is not exists" };
                }

                throw new Exception("Failed to save PhieuVatTuDetail");
            }
            catch (Exception ex)
            {
                return new ResponseDTO { Code = 400, Message = ex.Message };
            }
        }
        public ResponseDTO Add(PhieuVatTuDTO phieuVatTuDTO)
        {
            var validationResponse = ValidateDTO(phieuVatTuDTO);
            DateTime dateTime = DateTime.Now;
            string result = $"NKVT-{dateTime:ddMMyyyy}";
            if (!phieuVatTuDTO.IsNhapKho)
            {
                result = $"XKVT-{dateTime:ddMMyyyy}";
            }

            if (validationResponse != null) return validationResponse;

            string? userId = _userService.GetCurrentUser();
            if (userId == null)
                return new ResponseDTO { Code = 400, Message = "User not exists" };
            var phieuVatTu = _phieuVatTuRepository.Find(r => (r.IsDeleted == false) && r.MaPhieu.Equals(result)).FirstOrDefault();
            //var phieuVatTuDetail = _phieuVatTuDetailRepository.Find(r =>
            //    (r.IsDeleted == false) 
            //    && r.CreateBy.Equals(userId)
            //    && r.
            //).FirstOrDefault();

            //var phieuVatTuId = _phieuVatTuDetailRepository.Find(r => r.CreateBy.Equals(userId) && r.CreateDate.Equals(DateTime.Now));
            if (phieuVatTuDTO.IsNhapKho == false)
            {
                var phieuVatTuDetails = _phieuVatTuDetailRepository
                    .Find(d => d.QRCode.Equals(phieuVatTuDTO.QRCode) && d.Status == true);

                var query = from d in phieuVatTuDetails
                            join r in _phieuVatTuRepository.Find(r => r.IsDeleted == false)
                            on d.PhieuVatTuId equals r.Id
                            select new { d.SoLuong, r.ImportOrExport };
                double sumImport = query
                    .Where(x => x.ImportOrExport == true)
                    .Sum(x => x.SoLuong);

                double sumExport = query
                    .Where(x => x.ImportOrExport == false)
                    .Sum(x => x.SoLuong);
                double totalQuantity = Math.Round((double)sumImport - sumExport, 2);
                if (phieuVatTuDTO.SoLuong > totalQuantity)
                {
                    return new ResponseDTO()
                    {
                        Code = 400,
                        Message = "Số lượng sản phẩm xuất lớn hơn số hiện tại trong kho",
                        Description = null
                    };
                }
            }

            var phieuVatTuId = phieuVatTu == null ? Guid.Empty : phieuVatTu.Id;
            return !phieuVatTuId.Equals(Guid.Empty)
                ? AddPVTDetail(phieuVatTuDTO, userId, phieuVatTuId)
                : AddPVTDetail(phieuVatTuDTO, userId, CreateNewPhieuVatTu(phieuVatTuDTO, userId, phieuVatTuDTO.IsNhapKho).Id);
        }

        public ResponseDTO GetByMa(string ma)
        {
            var list = _phieuVatTuDetailRepository.Find(r => r.PhieuVatTu.MaPhieu.Equals(ma.Trim().ToUpper().Replace(" ", string.Empty)));
            var nhapXuatKhoVatTuDetailDTOs = list
                .Where(r => !r.IsDeleted) // Loại bỏ những khách hàng bị xóa (nếu cần)
                .Select(r => new NhapXuatKhoVatTuDetailDTO
                {
                    MaKyThuat = _VatTuRepository.GetById(r.VatTuId).MaKyThuat,
                    MaKeToan = _VatTuRepository.GetById(r.VatTuId).MaKeToan,
                    Id = r.Id,
                    NguoiNhap = r.CreateBy!,
                    Note = r.Notes,
                    SoLot = r.SoLot,
                    SoLuong = r.SoLuong,
                    TenDvt = _donViTinhRepository.GetById(_VatTuRepository.GetById(r.VatTuId).DonViTinhId).Name,
                    TenVatTu = _VatTuRepository.GetById(r.VatTuId).Name,
                    ThoiGian = r.ThoiGian.ToString("HH:mm:ss"),
                    IsDuyet = r.Status,
                    NguoiDuyet = r.NguoiDuyet
                    // Ánh xạ các trường cần thiết khác
                })
                .ToList();
            return new ResponseDTO { Code = 200, Message = "Sucess", Description = nhapXuatKhoVatTuDetailDTOs };
        }
        public ResponseGetDTO<PhieuDTO> GetPhieus(DateTime? dateFrom, DateTime? dateTo, int page, bool isNhap)
        {
            int pageSize = 20;

            // Lấy danh sách phiếu thỏa mãn điều kiện ngày và chưa bị xóa
            var phieus = _phieuVatTuRepository.GetAll()
                .Where(r => !r.IsDeleted && r.CreateDate >= dateFrom 
                && r.CreateDate <= dateTo
                && r.ImportOrExport == isNhap)
                .Select(r => new PhieuDTO()
                {
                    MaPhieuNhap = r.MaPhieu,
                    Ngay = r.CreateDate.ToString("MM/dd/yyyy"),
                    TinhTrang = r.Status,
                    TenKho = _khoVatTuRepository.GetById(r.KhoVatTuId).Name
                })
                .ToList();

            // Đếm tổng số bản ghi
            int totalRow = phieus.Count();

            // Tính tổng số trang
            int totalPage = (int)Math.Ceiling((double)totalRow / pageSize);

            // Áp dụng phân trang
            phieus = phieus
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new ResponseGetDTO<PhieuDTO>()
            {
                TotalRow = totalRow,
                TotalPage = totalPage,
                PageSize = pageSize,
                Datalist = phieus
            };
        }

        public ResponseGetDTO<BC_XuatNhapKhoVatTu> BC_N_X_KhoVatTu(DateTime? dateFrom, DateTime? dateTo, int page, bool status)
        {
            int pageSize = 20;

            // Lấy dữ liệu từ PhieuVatTu và PhieuVatTuDetail
            var phieuVatTus = _phieuVatTuRepository.GetAll()
                .Where(r => r.CreateDate >= dateFrom 
                && r.CreateDate <= dateTo 
                && !r.IsDeleted 
                && r.ImportOrExport == status)
                .ToList();

            var phieuVatTuDetails = _phieuVatTuDetailRepository.GetAll()
                .Where(d => phieuVatTus.Select(p => p.Id).Contains(d.PhieuVatTuId) && !d.IsDeleted && d.Status == true)
                .ToList();

            var result = phieuVatTuDetails.Select(detail => {
                var vatTu = _VatTuRepository.GetById(detail.VatTuId);

                return new BC_XuatNhapKhoVatTu
                {
                    Date = detail.CreateDate.ToString("MM/dd/yyyy"),
                    MaKyThuat = vatTu.MaKyThuat,
                    MaKeToan = vatTu.MaKeToan,
                    TenVatTu = vatTu.Name,
                    Dvt = _donViTinhRepository.GetById((Guid)(vatTu.DonViTinhId)).Name,
                    SoLuong = detail.SoLuong,
                    Time = detail.CreateDate.ToString("HH:mm:ss"),
                    SoLot = detail.SoLot,
                    TenNCC = detail.NhaCungCapID == null ? null : _nhaCungCapRepository.GetById((Guid)detail.NhaCungCapID!).Name
                };
            }).ToList();
            var groupedQuery = result
                .GroupBy(p => new { p.MaKyThuat, p.MaKeToan, p.TenVatTu, p.Date, p.Time, p.TenNCC })
                .Select(g => new BC_XuatNhapKhoVatTu
                {
                    Date = g.Key.Date,
                    MaKyThuat = g.Key.MaKyThuat,
                    MaKeToan = g.Key.MaKeToan,
                    TenVatTu = g.Key.TenVatTu,
                    Dvt = g.First().Dvt,
                    SoLuong = g.Sum(x => x.SoLuong),
                    Time = g.First().Time,
                    SoLot = g.First().SoLot,
                    TenNCC = g.Key.TenNCC
                })
                .OrderBy(p => p.Date)
                .ThenBy(p => p.MaKyThuat)
                .ThenBy(p => p.MaKeToan).ToList();
            // Phân trang
            int totalRow = groupedQuery.Count;
            int totalPage = (int)Math.Ceiling((double)totalRow / pageSize);

            var pagedResult = result
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new ResponseGetDTO<BC_XuatNhapKhoVatTu>
            {
                TotalRow = totalRow,
                TotalPage = totalPage,
                PageSize = pageSize,
                Datalist = pagedResult
            };
        }

        public ResponseGetDTO<BC_N_X_T_VatTu_ThanhPham> BC_N_X_SumKhoVatTu(DateTime? dateFrom, DateTime? dateTo, int page)
        {
            int pageSize = 20;

            // Lấy ngày cuối của tháng trước DateFrom
            DateTime lastDayOfPreviousMonth = dateFrom.Equals(DateTime.MinValue) ? DateTime.MinValue : dateFrom.Value.AddMonths(-1).AddDays(-dateFrom.Value.Day);

            // Lấy dữ liệu PhieuVatTu và PhieuVatTuDetail của tháng hiện tại
            var phieuVatTus = _phieuVatTuRepository.GetAll()
                .Where(r => r.CreateDate >= dateFrom
                && r.CreateDate <= dateTo
                && !r.IsDeleted)
                .ToList();

            var phieuVatTuDetails = _phieuVatTuDetailRepository.GetAll()
                .Where(d => phieuVatTus.Select(p => p.Id).Contains(d.PhieuVatTuId) && !d.IsDeleted && d.Status == true)
                .ToList();

            // Lấy dữ liệu tồn cuối của tháng trước (tức là tính đến ngày cuối cùng của tháng trước)
            var phieuVatTuDetailsPreviousMonth = _phieuVatTuDetailRepository.GetAll()
                .Where(d => d.CreateDate <= lastDayOfPreviousMonth && !d.IsDeleted)
                .ToList();

            // Nhóm dữ liệu tồn cuối tháng trước theo từng vật tư
            var tonCuoiKyTruoc = phieuVatTuDetailsPreviousMonth
                .GroupBy(d => d.VatTuId)
                .Select(g => new
                {
                    VatTuId = g.Key,
                    TonCuoiKyTruoc = g.Where(x => x.Status).Sum(x => x.SoLuong) - g.Where(x => !x.Status).Sum(x => x.SoLuong)
                }).ToDictionary(x => x.VatTuId, x => x.TonCuoiKyTruoc);

            // Kết quả tính toán vật tư của tháng hiện tại
            var result = phieuVatTuDetails.Select(detail => {
                var vatTu = _VatTuRepository.GetById(detail.VatTuId);
                var nhomVatTu = _nhomVatTuRepository.GetById(vatTu.NhomVatTuId); // Lấy nhóm vật tư
                var phieuVatTu = _phieuVatTuRepository.GetById(detail.PhieuVatTuId);
                // Tính số lượng đầu kỳ
                double dauKy = tonCuoiKyTruoc.ContainsKey(vatTu.Id) ? tonCuoiKyTruoc[vatTu.Id] : 0;

                return new
                {
                    NhomVatTuName = nhomVatTu.Name,
                    vatTu.MaKyThuat,
                    vatTu.MaKeToan,
                    TenVatTu = vatTu.Name,
                    DVT = _donViTinhRepository.GetById((Guid)(vatTu.DonViTinhId)).Name,
                    detail.SoLuong,
                    phieuVatTu.ImportOrExport,
                    detail.Status,
                    DauKy = dauKy // Thêm cột tồn đầu kỳ
                };
            }).ToList();

            // Nhóm theo tên nhóm vật tư và các thông tin vật tư bên dưới nhóm
            var groupedResult = result
                .GroupBy(r => r.NhomVatTuName)
                .Select(g => new BC_N_X_T_VatTu_ThanhPham
                {
                    Name = g.Key,
                    TVT = g.GroupBy(p => new { p.MaKyThuat, p.MaKeToan, p.TenVatTu, p.DVT })
                            .Select(vtGroup => new BC_N_X_T_VatTu_ThanhPhamDetail
                            {
                                MaKyThuat = vtGroup.Key.MaKyThuat,
                                MaKeToan = vtGroup.Key.MaKeToan,
                                TenVatTu = vtGroup.Key.TenVatTu,
                                DVT = vtGroup.Key.DVT,
                                DauKy = vtGroup.First().DauKy, // Tồn đầu kỳ
                                Nhap = vtGroup.Where(x => x.ImportOrExport).Sum(x => x.SoLuong),
                                Xuat = vtGroup.Where(x => !x.ImportOrExport).Sum(x => x.SoLuong),
                                Ton = vtGroup.First().DauKy + vtGroup.Where(x => x.ImportOrExport).Sum(x => x.SoLuong) - vtGroup.Where(x => !x.ImportOrExport).Sum(x => x.SoLuong), // Tồn hiện tại
                            }).ToList()
                }).ToList();

            // Phân trang
            int totalRow = groupedResult.Count;
            int totalPage = (int)Math.Ceiling((double)totalRow / pageSize);

            var pagedResult = groupedResult
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new ResponseGetDTO<BC_N_X_T_VatTu_ThanhPham>
            {
                TotalRow = totalRow,
                TotalPage = totalPage,
                PageSize = pageSize,
                Datalist = pagedResult
            };
        }


        protected PhieuVatTuDetail? getEntityByMa(string ma)
        {
            if (Guid.TryParse(ma, out Guid parsedGuid))
            {
                return _phieuVatTuDetailRepository.Find(r => (r.IsDeleted == false) && r.Id.Equals(parsedGuid))
                .FirstOrDefault();
            }
            return null;
        }

        public ResponseDTO Delete(string ma)
        {
            if (!string.IsNullOrWhiteSpace(ma))
            {
                string? userId = _userService.GetCurrentUser();
                if (userId == null)
                    return new ResponseDTO { Code = 400, Message = "User not exists" };

                var entityExists = getEntityByMa(ma);

                if (entityExists != null)
                {
                    if (entityExists.Status == true)
                    {
                        return new ResponseDTO()
                        {
                            Code = 400,
                            Message = "Phiếu này không được xóa vì đã duyệt",
                            Description = null
                        };
                    }
                    entityExists.IsDeleted = true;
                    entityExists.DeleteBy = userId;
                    entityExists.DeleteDate = DateTime.Now;
                    _phieuVatTuDetailRepository.Update(entityExists);
                    _phieuVatTuDetailRepository.IsSaveChange();
                    if (_phieuVatTuDetailRepository.Find(r => (r.IsDeleted == false)
                        && r.PhieuVatTuId.Equals(entityExists.Id)).FirstOrDefault() == null)
                    {
                        var phieuVatTu = _phieuVatTuRepository.Find(r => (r.IsDeleted == false)
                        && r.Id.Equals(entityExists.PhieuVatTuId)).FirstOrDefault();
                        phieuVatTu!.IsDeleted = true;
                        phieuVatTu.DeleteBy = userId;
                        phieuVatTu.DeleteDate = DateTime.Now;
                        _phieuVatTuRepository.Update(phieuVatTu);
                        _phieuVatTuRepository.IsSaveChange();
                    }
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

        public ResponseDTO Update(PhieuVatTuUpdateDTO dto)
        {
            var errorResponse = ValidateXNKDTO(dto);
            if (errorResponse != null) return errorResponse;

            string? userId = _userService.GetCurrentUser();
            if (userId == null)
                return new ResponseDTO { Code = 400, Message = "User not exists" };

            try
            {
                var entityExists = _phieuVatTuDetailRepository.Find(r => !r.IsDeleted && r.Id.Equals(Guid.Parse(dto.Id))).FirstOrDefault();

                if (entityExists == null)
                {
                    return new ResponseDTO()
                    {
                        Code = 400,
                        Message = "Entity not found",
                        Description = null
                    };
                }

                if (entityExists.Status == true)
                {
                    return new ResponseDTO()
                    {
                        Code = 400,
                        Message = "Phiếu này không được sửa vì đã duyệt",
                        Description = null
                    };
                }

                var phieuVatTu = _phieuVatTuRepository.Find(r => !r.IsDeleted && r.Id.Equals(entityExists.PhieuVatTuId)).FirstOrDefault();

                if (phieuVatTu == null)
                {
                    return new ResponseDTO()
                    {
                        Code = 400,
                        Message = "Phiếu vật tư không tồn tại",
                        Description = null
                    };
                }

                var phieuVatTuDetails = _phieuVatTuDetailRepository
                    .Find(d => d.QRCode.Equals(entityExists.QRCode) && d.Status == true)
                    .ToList();

                var query = from d in phieuVatTuDetails
                            join r in _phieuVatTuRepository.Find(r => r.IsDeleted == false)
                            on d.PhieuVatTuId equals r.Id
                            select new { d.SoLuong, r.ImportOrExport };

                double sumImport = query
                    .Where(x => x.ImportOrExport == true)
                    .Sum(x => x.SoLuong);

                double sumExport = query
                    .Where(x => x.ImportOrExport == false)
                    .Sum(x => x.SoLuong);

                double totalQuantity = Math.Round(sumImport - sumExport, 2);
                bool isExportWithValidQuantity = phieuVatTu.ImportOrExport == false 
                    && ((dto.SoLuong == null && entityExists.SoLuong <= totalQuantity) 
                    || dto.SoLuong <= totalQuantity);
                bool isImport = phieuVatTu.ImportOrExport == true;
                if (isExportWithValidQuantity || isImport)
                {
                    entityExists.UpdateBy = userId;
                    entityExists.UpdateDate = DateTime.Now;
                    entityExists.SoLuong = dto.SoLuong ?? entityExists.SoLuong;
                    entityExists.Notes = dto.Notes;
                    entityExists.Status = dto.IsDuyet;
                    entityExists.NguoiDuyet = dto.IsDuyet ? userId : null;
                    _phieuVatTuDetailRepository.Update(entityExists);

                    if (_phieuVatTuDetailRepository.IsSaveChange())
                    {
                        if (dto.IsDuyet == true)
                        {
                            phieuVatTuDetails = _phieuVatTuDetailRepository
                                .Find(d => !d.IsDeleted && d.PhieuVatTuId == phieuVatTu.Id)
                                .ToList();

                            bool areAllDetailsTrue = phieuVatTuDetails.All(d => d.Status == true);

                            if (areAllDetailsTrue != phieuVatTu.Status)
                            {
                                phieuVatTu.Status = areAllDetailsTrue;
                                _phieuVatTuRepository.Update(phieuVatTu);
                                _phieuVatTuRepository.IsSaveChange();
                            }
                        }
                        return new ResponseDTO()
                        {
                            Code = 200,
                            Message = "Update successed!!!",
                            Description = null
                        };
                    }
                    else
                    {
                        return new ResponseDTO()
                        {
                            Code = 400,
                            Message = "Save failed",
                            Description = null
                        };
                    }
                    
                }

                return new ResponseDTO()
                {
                    Code = 400,
                    Message = "Số lượng sản phẩm xuất lớn hơn số hiện tại trong kho",
                    Description = null
                };
            }
            catch (Exception ex)
            {
                // Log the exception for further analysis
                return new ResponseDTO()
                {
                    Code = 400,
                    Message = $"An error occurred: {ex.Message}",
                    Description = null
                };
            }
        }


        private ResponseDTO? ValidateDTO(PhieuVatTuDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.MaKho))
                return new ResponseDTO { Code = 400, Message = "Mã kho thành phẩm là null or empty" };

            if (string.IsNullOrWhiteSpace(dto.QRCode.Split('&')[0]))
                return new ResponseDTO { Code = 400, Message = "QR Code là null or empty" };
            if (_qRVatTuservice.GetByQRCode(dto.QRCode) == null)
                return new ResponseDTO { Code = 400, Message = "QR Code not extists" };

            if (dto.SoLuong <= 0)
                return new ResponseDTO { Code = 400, Message = "Số lượng thành phẩm <= 0" };

            return null;
        }
        private ResponseDTO? ValidateXNKDTO(PhieuVatTuUpdateDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Id))
                return new ResponseDTO { Code = 400, Message = "Id là null or empty" };


            if (dto.SoLuong != null && dto.SoLuong <= 0)
                return new ResponseDTO { Code = 400, Message = "Số lượng thành phẩm <= 0" };

            return null;
        }
    }
}
