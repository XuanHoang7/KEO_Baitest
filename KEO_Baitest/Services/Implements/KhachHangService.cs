using KEO_Baitest.Data.DTOs;
using KEO_Baitest.Data.Entities;
using KEO_Baitest.Repository.Implements;
using KEO_Baitest.Repository.Interfaces;
using KEO_Baitest.Services.Interfaces;
using KiemTraThuViec1.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace KEO_Baitest.Services.Implements
{
    public class KhachHangService : IKhachHangService
    {
        private readonly IKhachHangRepository _khachHangRepository;
        private readonly IUserService _userService;
        public KhachHangService(IKhachHangRepository khachHangRepository, IUserService userService)
        {
            _khachHangRepository = khachHangRepository;
            _userService = userService;
        }
        public  bool IsValidVietnamesePhoneNumber(string phoneNumber)
        {
            // Loại bỏ khoảng trắng hoặc dấu "-" trong số điện thoại
            phoneNumber = phoneNumber.Replace(" ", "").Replace("-", "");

            // Biểu thức chính quy để kiểm tra số điện thoại Việt Nam
            string pattern = @"^(03|05|07|08|09)\d{8}$";

            Regex regex = new Regex(pattern);
            return regex.IsMatch(phoneNumber);
        }
        private ResponseDTO? ValidateDTO(KhachHangDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.MaKhachHang))
                return new ResponseDTO { Code = 400, Message = "Mã khách hàng là null or only whitespace" };

            if (string.IsNullOrWhiteSpace(dto.TenKhachHang))
                return new ResponseDTO { Code = 400, Message = "Tên khách hàng là null or only whitespace" };

            if (string.IsNullOrWhiteSpace(dto.DiaChi))
                return new ResponseDTO { Code = 400, Message = "Địa chỉ là null or only whitespace" };
            if (dto.SoDienThoai == null || !IsValidVietnamesePhoneNumber(dto.SoDienThoai))
                return new ResponseDTO { Code = 400, Message = "Số điện thoại là null or không hợp lệ tại Việt Nam" };

            return null; // No errors
        }


        public ResponseGetDTO<KhachHangDTO> GetAll(int page, string keyword)
        {
            var khachHangDTOs = _khachHangRepository.Find(r => r.IsDeleted != true)
                .Select(kh => new KhachHangDTO
                {
                    MaKhachHang = kh.MaKhachHang,
                    TenKhachHang = kh.Name,
                    DiaChi = kh.DiaChi,
                    SoDienThoai = kh.SoDienThoai,
                })
                .ToList();
            int pageSize = 20;

            // Đảm bảo query là IQueryable<TEntity>
            var query = _khachHangRepository.Find(r => !r.IsDeleted);


            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(r => r.Name.Contains(keyword)).ToList();
            }

            int totalRow = query.Count();
            int totalPage = (int)Math.Ceiling((double)totalRow / pageSize);

            var entities = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(kh => new KhachHangDTO
                {
                    MaKhachHang = kh.MaKhachHang,
                    TenKhachHang = kh.Name,
                    DiaChi = kh.DiaChi,
                    SoDienThoai = kh.SoDienThoai,
                })
                .ToList();

            return new ResponseGetDTO<KhachHangDTO>
            {
                TotalRow = totalRow,
                TotalPage = totalPage,
                PageSize = pageSize,
                Datalist = entities
            };
        }

        public ResponseDTO GetByMa(string ma)
        {
            throw new NotImplementedException();
        }

        public ResponseDTO Update(KhachHangDTO dto)
        {
            if (dto.SoDienThoai != null && IsValidVietnamesePhoneNumber(dto.SoDienThoai))
                return new ResponseDTO { Code = 400, Message = "Số điện thoại không hợp lệ tại Việt Nam" };

            string? userId = _userService.GetCurrentUser();
            if (userId == null)
                return new ResponseDTO { Code = 400, Message = "User không tồn tại" };
            var khachHangExists = _khachHangRepository
                .Find(r => (r.IsDeleted == false) && r.MaKhachHang
                .Equals(dto.MaKhachHang.Trim().ToUpper().Replace(" ", string.Empty)))
                .FirstOrDefault();

            if (khachHangExists != null)
            {
                khachHangExists.Name = dto.TenKhachHang ?? khachHangExists.Name;
                khachHangExists.DiaChi = dto.DiaChi ?? khachHangExists.DiaChi;
                khachHangExists.SoDienThoai = dto.SoDienThoai ?? khachHangExists.DiaChi;
                khachHangExists.UpdateBy = userId;
                khachHangExists.UpdateDate = DateTime.Now;
                _khachHangRepository.Update(khachHangExists);
                if (_khachHangRepository.IsSaveChange())
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
                Message = "Mã khách hàng không tồn tại trong database",
                Description = null
            };
        }

        public ResponseDTO Delete(string ma)
        {
            if (!string.IsNullOrWhiteSpace(ma))
            {
                string? userId = _userService.GetCurrentUser();
                if (userId == null)
                    return new ResponseDTO { Code = 400, Message = "User not exists" };
                var khachHangExists = _khachHangRepository
                    .Find(r => (r.IsDeleted == false) && r.MaKhachHang
                    .Equals(ma.Trim().ToUpper().Replace(" ", string.Empty)))
                    .FirstOrDefault();

                if (khachHangExists != null)
                {
                    khachHangExists.IsDeleted = true;
                    khachHangExists.DeleteBy = userId;
                    khachHangExists.DeleteDate = DateTime.Now;
                    _khachHangRepository.Update(khachHangExists);
                    if (_khachHangRepository.IsSaveChange())
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
                    Message = "Mã khách hàng không tồn tại trong database",
                    Description = null
                };
            }
            return new ResponseDTO()
            {
                Code = 400,
                Message = "Mã là null or empty",
                Description = null
            };
        }

        public ResponseDTO Add(KhachHangDTO dto)
        {
            var errorResponse = ValidateDTO(dto);
            if (errorResponse != null) return errorResponse;
            var khachHangExists = _khachHangRepository
                .Find(r => (r.IsDeleted == false) && r.MaKhachHang
                .Equals(dto.MaKhachHang.Trim().ToUpper().Replace(" ", string.Empty)))
                .FirstOrDefault();
            string? userId = _userService.GetCurrentUser();
            if (userId == null)
                return new ResponseDTO { Code = 400, Message = "User not exists" };
            KhachHang k = new KhachHang
            {
                MaKhachHang = dto.MaKhachHang,
                Name = dto.TenKhachHang,
                DiaChi = dto.DiaChi,
                SoDienThoai = dto.SoDienThoai,
                CreateBy = userId,
                CreateDate = DateTime.Now
            };
            if (khachHangExists == null)
            {
                
                _khachHangRepository.Add(k);
                if (_khachHangRepository.IsSaveChange())
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
                Message = "Trùng mã khách hàng",
                Description = null
            };
        }
    }
}
