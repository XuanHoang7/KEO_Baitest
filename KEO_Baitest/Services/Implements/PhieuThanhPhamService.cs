using KEO_Baitest.Data.DTOs;
using KEO_Baitest.Data.Entities;
using KEO_Baitest.Repository.Implements;
using KEO_Baitest.Repository.Interfaces;
using KEO_Baitest.Services.Interfaces;
using KiemTraThuViec1.Data;

namespace KEO_Baitest.Services.Implements
{
    public class PhieuThanhPhamService : IPhieuThanhPhamService
    {
        private readonly IUserService _userService;
        private readonly IPhieuThanhPhamDetailRepository _phieuThanhPhamDetailRepository;
        private readonly IPhieuThanhPhamRepository _phieuThanhPhamRepository;
        private readonly INhaCungCapRepository _nhaCungCapRepository;
        private readonly IKhachHangRepository _khachHangRepository;
        private readonly IThanhPhamRepository _ThanhPhamRepository;
        private readonly IKhoThanhPhamRepository _khoThanhPhamRepository;
        private readonly IDonViTinhRepository _donViTinhRepository;
        private readonly IQRThanhPhamService _qRThanhPhamservice;
        private readonly INhomThanhPhamRepository _nhomThanhPhamRepository;
        public PhieuThanhPhamService(IPhieuThanhPhamDetailRepository phieuThanhPhamDetailRepository,
            IUserService userService,
            IPhieuThanhPhamRepository phieuThanhPhamRepository,
            INhaCungCapRepository nhaCungCapRepository,
            IThanhPhamRepository ThanhPhamRepository,
            IKhoThanhPhamRepository khoThanhPhamRepository,
            IDonViTinhRepository donViTinhRepository,
            IQRThanhPhamService qRThanhPhamService,
            IKhachHangRepository khachHangRepository,
            INhomThanhPhamRepository nhomThanhPhamRepository)
        {
            _userService = userService;
            _phieuThanhPhamDetailRepository = phieuThanhPhamDetailRepository;
            _phieuThanhPhamRepository = phieuThanhPhamRepository;
            _nhaCungCapRepository = nhaCungCapRepository;
            _ThanhPhamRepository = ThanhPhamRepository;
            _khoThanhPhamRepository = khoThanhPhamRepository;
            _donViTinhRepository = donViTinhRepository;
            _qRThanhPhamservice = qRThanhPhamService;
            _khachHangRepository = khachHangRepository;
            _nhomThanhPhamRepository = nhomThanhPhamRepository;
        }


        private ThanhPham GetThanhPham(string maThanhPham)
        {
            string maketoan = maThanhPham.Trim().ToUpper().Replace(" ", string.Empty);
            return _ThanhPhamRepository.Find(r => (r.IsDeleted == false) && r.MaKeToan.Equals(maketoan)).FirstOrDefault() ??
                   throw new Exception("Mã kế toán không tồn tại");
        }
        private PhieuThanhPham CreateNewPhieuThanhPham(PhieuVatTuDTO phieuThanhPhamDTO, string userId, bool status)
        {
            DateTime dateTime = DateTime.Now;
            string result = $"NKTP-{dateTime:ddMMyyyy}";
            if (!status)
            {
                result = $"XKTP-{dateTime:ddMMyyyy}";
            }

            var phieuThanhPham = new PhieuThanhPham
            {
                MaPhieu = result,
                KhoThanhPhamId = _khoThanhPhamRepository.GetKhoThanhPhamByMa(phieuThanhPhamDTO.MaKho)?.Id
                    ?? throw new Exception("Kho Thành phẩm not exists"),
                ImportOrExport = phieuThanhPhamDTO.IsNhapKho,
                Status = status,
                CreateBy = userId,
                CreateDate = DateTime.Now
            };

            _phieuThanhPhamRepository.Add(phieuThanhPham);
            if (!_phieuThanhPhamRepository.IsSaveChange())
                throw new Exception("Phieu thành phẩm not save");

            return phieuThanhPham;
        }

        private ResponseDTO AddPVTDetail(PhieuVatPhamDTO phieuThanhPhamDTO, string userId, Guid phieuThanhPhamId)
        {
            try
            {
                KhachHang? kh = null;
                if (phieuThanhPhamDTO.IsNhapKho == false)
                {
                    kh = _khachHangRepository.Find(kh => kh.MaKhachHang.Equals(phieuThanhPhamDTO.MaKhachHang)).FirstOrDefault();
                    if (kh == null)
                    {
                        return new ResponseDTO { Code = 400, Message = "Mã khách hàng không tồn tại" };
                    }
                }
                QRThanhPham? q = _qRThanhPhamservice.GetByQRCode(phieuThanhPhamDTO.QRCode);

                // Gán giá trị cho các biến
                if (q != null)
                {
                    var p = new PhieuThanhPhamDetail
                    {
                        PhieuThanhPhamId = phieuThanhPhamId,
                        KhachHangId = kh?.Id,
                        ThanhPhamId = q.ThanhPhamId,
                        SoLuong = phieuThanhPhamDTO.SoLuong,
                        SoLot = q.SoLot,
                        ThoiGian = DateTime.Now,
                        CreateBy = userId,
                        CreateDate = DateTime.Now,
                        QRCode = q.QRCode
                    };
                    _phieuThanhPhamDetailRepository.Add(p);
                    if (_phieuThanhPhamDetailRepository.IsSaveChange())
                    {
                        var phieuThanhPham = _phieuThanhPhamRepository
                                .Find(d => !d.IsDeleted && d.Id == p.PhieuThanhPhamId)
                                .FirstOrDefault();
                        if (phieuThanhPham != null && phieuThanhPham.Status == true)
                        {
                            phieuThanhPham.Status = false;
                            _phieuThanhPhamRepository.Update(phieuThanhPham);
                            _phieuThanhPhamRepository.IsSaveChange();
                        }
                        return new ResponseDTO { Code = 200, Message = "Success", Description = p };
                    }
                }
                else
                {
                    return new ResponseDTO { Code = 400, Message = "QRCode is not exists" };
                }

                throw new Exception("Failed to save PhieuThanhPhamDetail");
            }
            catch (Exception ex)
            {
                return new ResponseDTO { Code = 400, Message = ex.Message };
            }
        }
        public ResponseDTO Add(PhieuVatPhamDTO phieuThanhPhamDTO)
        {
            var validationResponse = ValidateDTO(phieuThanhPhamDTO);
            DateTime dateTime = DateTime.Now;
            string result = $"NKTP-{dateTime:ddMMyyyy}";
            if (!phieuThanhPhamDTO.IsNhapKho)
            {
                result = $"XKTP-{dateTime:ddMMyyyy}";
            }
            
            if (validationResponse != null) return validationResponse;

            string? userId = _userService.GetCurrentUser();
            if (userId == null)
                return new ResponseDTO { Code = 400, Message = "User not exists" };
            var phieuThanhPham = _phieuThanhPhamRepository.Find(r => (r.IsDeleted == false) && r.MaPhieu.Equals(result)).FirstOrDefault();
            //var phieuThanhPhamDetail = _phieuThanhPhamDetailRepository.Find(r =>
            //    (r.IsDeleted == false) 
            //    && r.CreateBy.Equals(userId)
            //    && r.
            //).FirstOrDefault();
            
            //var phieuThanhPhamId = _phieuThanhPhamDetailRepository.Find(r => r.CreateBy.Equals(userId) && r.CreateDate.Equals(DateTime.Now));
            if(phieuThanhPhamDTO.IsNhapKho == false)
            {
                var phieuThanhPhamDetails = _phieuThanhPhamDetailRepository
                    .Find(d => d.QRCode.Equals(phieuThanhPhamDTO.QRCode) && d.Status == true)
                    .ToList();
                double sumImport = phieuThanhPhamDetails
                    .Where(d => _phieuThanhPhamRepository
                        .Find(r => r.IsDeleted == false && r.Id == d.PhieuThanhPhamId)
                        .FirstOrDefault()?.ImportOrExport == true)
                    .Sum(d => d.SoLuong);
                double sumExport = phieuThanhPhamDetails
                    .Where(d => _phieuThanhPhamRepository
                        .Find(r => r.IsDeleted == false && r.Id == d.PhieuThanhPhamId)
                        .FirstOrDefault()?.ImportOrExport == false)
                    .Sum(d => d.SoLuong);
                double totalQuantity = Math.Round((double)sumImport - sumExport, 2);
                if (phieuThanhPhamDTO.SoLuong > totalQuantity)
                {
                    return new ResponseDTO()
                    {
                        Code = 400,
                        Message = "Số lượng sản phẩm xuất lớn hơn số hiện tại trong kho",
                        Description = null
                    };
                }
            }

            var phieuThanhPhamId = phieuThanhPham == null ? Guid.Empty : phieuThanhPham.Id;
            return !phieuThanhPhamId.Equals(Guid.Empty)
                ? AddPVTDetail(phieuThanhPhamDTO, userId, phieuThanhPhamId)
                : AddPVTDetail(phieuThanhPhamDTO, userId, CreateNewPhieuThanhPham(phieuThanhPhamDTO, userId, phieuThanhPhamDTO.IsNhapKho).Id);
        }

        public ResponseGetDTO<BC_NhapKhoThanhPhamDTO> BC_NhapKhoThanhPham(DateTime? dateFrom, DateTime? dateTo, int page)
        {
            int pageSize = 20;

            // Lấy dữ liệu từ PhieuVatTu và PhieuVatTuDetail
            var phieuThanhPhams = _phieuThanhPhamRepository.GetAll()
                .Where(r => r.CreateDate >= dateFrom
                && r.CreateDate <= dateTo
                && !r.IsDeleted
                && r.ImportOrExport == true)
            .ToList();

            var phieuThanhPhamDetails = _phieuThanhPhamDetailRepository.GetAll()
                .Where(d => phieuThanhPhams.Select(p => p.Id).Contains(d.PhieuThanhPhamId) 
                && !d.IsDeleted && d.Status == true)
                .ToList();

            var result = phieuThanhPhamDetails.Select(detail => {
                var thanhPham = _ThanhPhamRepository.GetById(detail.ThanhPhamId.ToString());

                return new BC_NhapKhoThanhPhamDTO
                {
                    Date = detail.CreateDate.ToString("MM/dd/yyyy"),
                    MaKyThuat = thanhPham.MaThanhPham,
                    MaKeToan = thanhPham.MaKeToan,
                    TenThanhPham = thanhPham.Name,
                    Dvt = _donViTinhRepository.GetById(thanhPham.DonViTinhId.ToString()).Name,
                    SoLuong = detail.SoLuong,
                    Time = detail.CreateDate.ToString("HH:mm:ss")
                };
            }).ToList();
            var groupedQuery = result
                .GroupBy(p => new { p.MaKyThuat, p.MaKeToan, p.TenThanhPham, p.Date, p.Time })
                .Select(g => new BC_NhapKhoThanhPhamDTO
                {
                    Date = g.Key.Date,
                    MaKyThuat = g.Key.MaKyThuat,
                    MaKeToan = g.Key.MaKeToan,
                    TenThanhPham = g.Key.TenThanhPham,
                    Dvt = g.First().Dvt,
                    SoLuong = g.Sum(x => x.SoLuong),
                    Time = g.First().Time,
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

            return new ResponseGetDTO<BC_NhapKhoThanhPhamDTO>
            {
                TotalRow = totalRow,
                TotalPage = totalPage,
                PageSize = pageSize,
                Datalist = pagedResult
            };
        }


        public ResponseGetDTO<BC_N_X_T_VatTu_ThanhPhamDTO> BC_N_X_SumThanhPham(DateTime? dateFrom, DateTime? dateTo, int page)
        {
            int pageSize = 20;

            // Lấy ngày cuối của tháng trước DateFrom
            DateTime lastDayOfPreviousMonth = dateFrom.Equals(DateTime.MinValue) ? DateTime.MinValue : dateFrom.Value.AddMonths(-1).AddDays(-dateFrom.Value.Day);

            // Lấy dữ liệu PhieuVatTu và PhieuVatTuDetail của tháng hiện tại
            var phieuVatPham = _phieuThanhPhamRepository.GetAll()
                .Where(r => r.CreateDate >= dateFrom
                && r.CreateDate <= dateTo
                && !r.IsDeleted)
                .ToList();

            var phieuThanhPhamDetails = _phieuThanhPhamDetailRepository.GetAll()
                .Where(d => phieuVatPham.Select(p => p.Id).Contains(d.PhieuThanhPhamId) && !d.IsDeleted && d.Status == true)
                .ToList();

            // Lấy dữ liệu tồn cuối của tháng trước (tức là tính đến ngày cuối cùng của tháng trước)
            var phieuVatTuDetailsPreviousMonth = _phieuThanhPhamDetailRepository.GetAll()
                .Where(d => d.CreateDate <= lastDayOfPreviousMonth && !d.IsDeleted)
                .ToList();

            // Nhóm dữ liệu tồn cuối tháng trước theo từng vật tư
            var tonCuoiKyTruoc = phieuVatTuDetailsPreviousMonth
                .GroupBy(d => d.ThanhPhamId)
                .Select(g => new
                {
                    VatTuId = g.Key,
                    TonCuoiKyTruoc = g.Where(x => x.Status).Sum(x => x.SoLuong) - g.Where(x => !x.Status).Sum(x => x.SoLuong)
                }).ToDictionary(x => x.VatTuId, x => x.TonCuoiKyTruoc);

            // Kết quả tính toán vật tư của tháng hiện tại
            var result = phieuThanhPhamDetails.Select(detail => {
                var vatTu = _ThanhPhamRepository.GetById(detail.ThanhPhamId.ToString());
                var nhomVatTu = _nhomThanhPhamRepository.GetById(vatTu.NhomThanhPhamId.ToString()); // Lấy nhóm vật tư
                var phieuThanhPham = _phieuThanhPhamRepository.GetById(detail.PhieuThanhPhamId.ToString());
                // Tính số lượng đầu kỳ
                double dauKy = tonCuoiKyTruoc.ContainsKey(vatTu.Id) ? tonCuoiKyTruoc[vatTu.Id] : 0;

                return new
                {
                    NhomVatTuName = nhomVatTu.Name,
                    vatTu.MaThanhPham,
                    vatTu.MaKeToan,
                    TenVatTu = vatTu.Name,
                    DVT = _donViTinhRepository.GetById(vatTu.DonViTinhId.ToString()).Name,
                    detail.SoLuong,
                    detail.Status,
                    phieuThanhPham.ImportOrExport,
                    DauKy = dauKy // Thêm cột tồn đầu kỳ
                };
            }).ToList();

            // Nhóm theo tên nhóm vật tư và các thông tin vật tư bên dưới nhóm
            var groupedResult = result
                .GroupBy(r => r.NhomVatTuName)
                .Select(g => new BC_N_X_T_VatTu_ThanhPhamDTO
                {
                    Name = g.Key,
                    TVT = g.GroupBy(p => new { p.MaThanhPham, p.MaKeToan, p.TenVatTu, p.DVT})
                            .Select(vtGroup => new BC_N_X_T_VatTu_ThanhPhamDetailDTO
                            {
                                MaKyThuat = vtGroup.Key.MaThanhPham,
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

            return new ResponseGetDTO<BC_N_X_T_VatTu_ThanhPhamDTO>
            {
                TotalRow = totalRow,
                TotalPage = totalPage,
                PageSize = pageSize,
                Datalist = pagedResult
            };
        }

        public ResponseGetDTO<BC_XuatKhoThanhPhamDTO> BC_XuatKhoThanhPham(DateTime? dateFrom, DateTime? dateTo, int page)
        {
            int pageSize = 20;

            // Lấy dữ liệu từ PhieuVatTu và PhieuVatTuDetail
            var phieuThanhPhams = _phieuThanhPhamRepository.GetAll()
                .Where(r => r.CreateDate >= dateFrom
                && r.CreateDate <= dateTo
                && !r.IsDeleted
                && r.ImportOrExport == false)
            .ToList();

            var phieuThanhPhamDetails = _phieuThanhPhamDetailRepository.GetAll()
                .Where(d => phieuThanhPhams.Select(p => p.Id).Contains(d.PhieuThanhPhamId)
                && !d.IsDeleted && d.Status == true)
                .ToList();

            var result = phieuThanhPhamDetails.Select(detail => {
                var thanhPham = _ThanhPhamRepository.GetById(detail.ThanhPhamId.ToString());
                var khachHang = _khachHangRepository.GetById(detail.KhachHangId.ToString());
                return new BC_XuatKhoThanhPhamDTO
                {
                    Date = detail.CreateDate.ToString("MM/dd/yyyy"),
                    MaKyThuat = thanhPham.MaThanhPham,
                    MaKeToan = thanhPham.MaKeToan,
                    TenThanhPham = thanhPham.Name,
                    Dvt = _donViTinhRepository.GetById(thanhPham.DonViTinhId.ToString()).Name,
                    SoLuong = detail.SoLuong,
                    Time = detail.CreateDate.ToString("HH:mm:ss"),
                    DonViGiaoHang = khachHang.Name
                };
            }).ToList();
            var groupedQuery = result
                .GroupBy(p => new { p.MaKyThuat, p.MaKeToan, p.TenThanhPham, p.Date, p.Time, p.DonViGiaoHang })
                .Select(g => new BC_XuatKhoThanhPhamDTO
                {
                    Date = g.Key.Date,
                    MaKyThuat = g.Key.MaKyThuat,
                    MaKeToan = g.Key.MaKeToan,
                    TenThanhPham = g.Key.TenThanhPham,
                    Dvt = g.First().Dvt,
                    SoLuong = g.Sum(x => x.SoLuong),
                    Time = g.First().Time,
                    DonViGiaoHang = g.First().DonViGiaoHang
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

            return new ResponseGetDTO<BC_XuatKhoThanhPhamDTO>
            {
                TotalRow = totalRow,
                TotalPage = totalPage,
                PageSize = pageSize,
                Datalist = pagedResult
            };
        }

        public ResponseDTO GetByMa(string ma)
        {
            var list = _phieuThanhPhamDetailRepository.Find(r => r.PhieuThanhPham.MaPhieu.Equals(ma.Trim().ToUpper().Replace(" ", string.Empty)));
            var nhapXuatKhoThanhPhamDetailDTOs = list
                .Where(r => !r.IsDeleted) // Loại bỏ những khách hàng bị xóa (nếu cần)
                .Select(r => new NhapXuatKhoThanhPhamDetailDTO
                {
                    MaKyThuat = _ThanhPhamRepository.GetById(r.ThanhPhamId.ToString()).MaThanhPham,
                    MaKeToan = _ThanhPhamRepository.GetById(r.ThanhPhamId.ToString()).MaKeToan,
                    Id = r.Id,
                    NguoiNhap = r.CreateBy!,
                    Note = r.Notes,
                    SoLot = r.SoLot,
                    SoLuong = r.SoLuong,
                    TenDvt = _donViTinhRepository.GetById(_ThanhPhamRepository.GetById(r.ThanhPhamId.ToString()).Id.ToString()).Name,
                    TenThanhPham = _ThanhPhamRepository.GetById(r.ThanhPhamId.ToString()).Name,
                    ThoiGian = r.ThoiGian.ToString("HH:mm:ss"),
                    IsDuyet = r.Status,
                    NguoiDuyet = r.NguoiDuyet
                    // Ánh xạ các trường cần thiết khác
                })
                .ToList();
            return new ResponseDTO { Code = 200, Message = "Sucess", Description = nhapXuatKhoThanhPhamDetailDTOs };
        }
        public ResponseGetDTO<PhieuDTO> GetPhieus(DateTime? dateFrom, DateTime? dateTo, int page, bool isNhap)
        {
            int pageSize = 20;

            // Lấy danh sách phiếu thỏa mãn điều kiện ngày và chưa bị xóa
            var phieus = _phieuThanhPhamRepository.GetAll()
                .Where(r => !r.IsDeleted && r.CreateDate >= dateFrom 
                && r.CreateDate <= dateTo
                && r.ImportOrExport == isNhap)
                .Select(r => new PhieuDTO()
                {
                    MaPhieuNhap = r.MaPhieu,
                    Ngay = r.CreateDate.ToString("MM/dd/yyyy"),
                    TinhTrang = r.Status,
                    TenKho = _khoThanhPhamRepository.GetById(r.KhoThanhPhamId.ToString()).Name
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

        protected PhieuThanhPhamDetail? getEntityByMa(string ma)
        {
            if (Guid.TryParse(ma, out Guid parsedGuid))
            {
                return _phieuThanhPhamDetailRepository.Find(r => (r.IsDeleted == false) && r.Id.Equals(parsedGuid))
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
                    if(entityExists.Status == true)
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
                    _phieuThanhPhamDetailRepository.Update(entityExists);
                    _phieuThanhPhamDetailRepository.IsSaveChange();
                    if (_phieuThanhPhamDetailRepository.Find(r => (r.IsDeleted == false) 
                        && r.PhieuThanhPhamId.Equals(entityExists.Id)).FirstOrDefault() == null)
                    {
                        var phieuThanhPham = _phieuThanhPhamRepository.Find(r => (r.IsDeleted == false)
                        && r.Id.Equals(entityExists.PhieuThanhPhamId)).FirstOrDefault();
                        phieuThanhPham!.IsDeleted = true;
                        phieuThanhPham.DeleteBy = userId;
                        phieuThanhPham.DeleteDate = DateTime.Now;
                        _phieuThanhPhamRepository.Update(phieuThanhPham);
                        _phieuThanhPhamRepository.IsSaveChange();
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
                var entityExists = _phieuThanhPhamDetailRepository.Find(r => (r.IsDeleted == false) &&
                                    r.Id.Equals(Guid.Parse(dto.Id))).FirstOrDefault();

                if (entityExists != null)
                {
                    if (entityExists.Status == true)
                    {
                        return new ResponseDTO()
                        {
                            Code = 400,
                            Message = "Phiếu này không được sửa vì đã duyệt",
                            Description = null
                        };
                    }
                    var phieuThanhPham = _phieuThanhPhamRepository.Find(r => (r.IsDeleted == false)
                        && r.Id.Equals(entityExists.PhieuThanhPhamId)).FirstOrDefault();
                    var phieuThanhPhamDetails = _phieuThanhPhamDetailRepository
                    .Find(d => d.QRCode.Equals(entityExists.QRCode) && d.Status == true)
                    .ToList();
                    var query = from d in phieuThanhPhamDetails
                                join r in _phieuThanhPhamRepository.Find(r => r.IsDeleted == false)
                                on d.PhieuThanhPhamId equals r.Id
                                select new { d.SoLuong, r.ImportOrExport };
                    double sumImport = query
                        .Where(x => x.ImportOrExport == true)
                        .Sum(x => x.SoLuong);

                    double sumExport = query
                        .Where(x => x.ImportOrExport == false)
                        .Sum(x => x.SoLuong);
                    double totalQuantity = Math.Round((double)sumImport - sumExport, 2);
                    //double totalQuantity = _phieuThanhPhamDetailRepository
                    //        .Find(d => d.QRCode.Equals(entityExists.QRCode) 
                    //        && _phieuThanhPhamRepository.Find(r => (r.IsDeleted == false)
                    //    && r.Id.Equals(d.PhieuThanhPhamId)).FirstOrDefault()!.ImportOrExport == true)
                    //        .Sum(d => d.SoLuong);
                    bool isExportWithValidQuantity = phieuThanhPham?.ImportOrExport == false
                        && ((dto.SoLuong == null && entityExists.SoLuong <= totalQuantity)
                        || dto.SoLuong <= totalQuantity);
                    bool isImport = phieuThanhPham?.ImportOrExport == true;
                    if (isExportWithValidQuantity || isImport)
                    {
                        entityExists.UpdateBy = userId;
                        entityExists.UpdateDate = DateTime.Now;
                        entityExists.SoLuong = dto.SoLuong ?? entityExists.SoLuong;
                        entityExists.Notes = dto.Notes;
                        entityExists.Status = dto.IsDuyet;
                        entityExists.NguoiDuyet = entityExists.Status ? userId : null;
                        _phieuThanhPhamDetailRepository.Update(entityExists);
                        if (_phieuThanhPhamDetailRepository.IsSaveChange())
                        {
                            if (dto.IsDuyet == true)
                            {
                                // Giả sử phieuVatTuDetails là danh sách các đối tượng PhieuVatTuDetail
                                phieuThanhPhamDetails = _phieuThanhPhamDetailRepository
                                    .Find(d => (d.IsDeleted == false) && d.PhieuThanhPhamId == phieuThanhPham.Id)
                                    .ToList();

                                // Kiểm tra xem tất cả các đối tượng PhieuVatTuDetail có Status == true không
                                bool areAllDetailsTrue = phieuThanhPhamDetails.All(d => d.Status == true);

                                // Cập nhật Status của PhieuVatTu nếu tất cả Status trong danh sách đều là true
                                if (areAllDetailsTrue != phieuThanhPham.Status)
                                {
                                    phieuThanhPham.Status = areAllDetailsTrue;
                                    _phieuThanhPhamRepository.Update(phieuThanhPham);
                                    _phieuThanhPhamRepository.IsSaveChange();
                                }

                                // Sau đó bạn có thể cập nhật lại đối tượng PhieuVatTu vào cơ sở dữ liệu
                            }
                            return new ResponseDTO()
                            {
                                Code = 200,
                                Message = "Update successed!!!",
                                Description = null
                            };
                        }
                        return new ResponseDTO()
                        {
                            Code = 200,
                            Message = "Success",
                            Description = null
                        };
                    }
                    else return new ResponseDTO()
                    {
                        Code = 400,
                        Message = "Số lượng sản phẩm xuất lớn hơn số hiện tại trong kho",
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
            catch (Exception)
            {
                return new ResponseDTO()
                {
                    Code = 400,
                    Message = "ID không hợp lệ",
                    Description = null
                };
            }

        }

        private ResponseDTO? ValidateDTO(PhieuVatPhamDTO dto)
        {
            if(dto.IsNhapKho == false && string.IsNullOrEmpty(dto.MaKhachHang))
                return new ResponseDTO { Code = 400, Message = "Mã khách hàng là null or empty" };
            if (string.IsNullOrWhiteSpace(dto.MaKho))
                return new ResponseDTO { Code = 400, Message = "Mã kho thành phẩm là null or empty" };

            if (string.IsNullOrWhiteSpace(dto.QRCode.Split('&')[0]))
                return new ResponseDTO { Code = 400, Message = "QR Code là null or empty" };
            if (_qRThanhPhamservice.GetByQRCode(dto.QRCode) == null)
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
