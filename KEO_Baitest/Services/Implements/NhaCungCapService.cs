﻿using KEO_Baitest.Data.DTOs;
using KEO_Baitest.Data.Entities;
using KEO_Baitest.Repository.Interfaces;
using KEO_Baitest.Services.Interfaces;
using KiemTraThuViec1.Data;
using System.Text.RegularExpressions;

namespace KEO_Baitest.Services.Implements
{
    public class NhaCungCapService : GenericService<NhaCungCap, NhaCungCapDTO>
    {
        public NhaCungCapService(INhaCungCapRepository repository, IUserService userService) : base(repository, userService)
        {
        }

        protected override NhaCungCap? getEntityByDto(NhaCungCapDTO dto)
        {
            var result = _repository.GetById(dto.Id);
            return result.IsDeleted ? null : result;
        }

        protected override NhaCungCap? getEntityByMa(string ma)
        {
            var result = _repository.GetById(ma);
            return result.IsDeleted ? null : result;
        }

        protected override NhaCungCapDTO MapToDto(NhaCungCap entity)
        {
            return new NhaCungCapDTO
            {
                Id = entity.Id.ToString(),
                MaNhaCungCap = entity.MaNhaCungCap,
                TenNhaCungCap = entity.Name,
                DiaChi = entity.DiaChi,
                SoDienThoai = entity.SoDienThoai
            };
        }

        protected override NhaCungCap UpdateEntityF(NhaCungCap nhaCungCap, NhaCungCapDTO dto)
        {
            nhaCungCap.MaNhaCungCap = dto.MaNhaCungCap.Trim().ToUpper().Replace(" ", string.Empty);
            nhaCungCap.Name = dto.TenNhaCungCap ?? nhaCungCap.Name;
            nhaCungCap.DiaChi = dto.DiaChi ?? nhaCungCap.DiaChi;
            nhaCungCap.SoDienThoai = dto.SoDienThoai ?? nhaCungCap.Name;
            return nhaCungCap;
        }

        protected override NhaCungCap MapToEntity(NhaCungCapDTO dto)
        {
            return new NhaCungCap
            {
                MaNhaCungCap = dto.MaNhaCungCap,
                Name = dto.TenNhaCungCap,
                DiaChi = dto.DiaChi,
                SoDienThoai = dto.SoDienThoai
            };
        }
        public bool IsValidVietnamesePhoneNumber(string phoneNumber)
        {
            // Loại bỏ khoảng trắng hoặc dấu "-" trong số điện thoại
            phoneNumber = phoneNumber.Replace(" ", "").Replace("-", "");

            // Biểu thức chính quy để kiểm tra số điện thoại Việt Nam
            string pattern = @"^(03|05|07|08|09)\d{8}$";

            Regex regex = new Regex(pattern);
            return regex.IsMatch(phoneNumber);
        }
        protected override ResponseDTO? ValidateDTO(NhaCungCapDTO dto, bool isAdd = true)
        {
            if (string.IsNullOrWhiteSpace(dto.MaNhaCungCap))
                return new ResponseDTO { Code = 400, Message = "Mã Nhà cung cấp là null or only whitespace" };

            if (string.IsNullOrWhiteSpace(dto.TenNhaCungCap))
                return new ResponseDTO { Code = 400, Message = "Tên nhà cung cấp là null or only whitespace" };

            if (string.IsNullOrWhiteSpace(dto.DiaChi))
                return new ResponseDTO { Code = 400, Message = "Địa chỉ là null or only whitespace" };
            if (dto.SoDienThoai == null || !IsValidVietnamesePhoneNumber(dto.SoDienThoai))
                return new ResponseDTO { Code = 400, Message = "Số điện thoại là null or không hợp lệ tại Việt Nam" };
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
                    && r.MaNhaCungCap.Equals(dto.MaNhaCungCap) && !r.Id.Equals(entity.Id));
                    if (entityAnotherMa.Count != 0)
                    {
                        return new ResponseDTO { Code = 400, Message = "Mã này đã tồn tại" };
                    }
                }
            }
            else
            {
                var entity = _repository.Find(r => (r.IsDeleted == false) && r.MaNhaCungCap.Equals(dto.MaNhaCungCap.Trim().ToUpper().Replace(" ", string.Empty)))
                .FirstOrDefault();
                if (entity != null)
                {
                    return new ResponseDTO { Code = 400, Message = "Mã này đã tồn tại" };
                }
            }
            return null; // No errors
        }
    }
}
