using KEO_Baitest.Data;
using KEO_Baitest.Data.DTOs;
using KEO_Baitest.Data.Entities;
using KEO_Baitest.Repository.Implements;
using KEO_Baitest.Repository.Interfaces;
using KEO_Baitest.Services.Interfaces;
using KiemTraThuViec1.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace KEO_Baitest.Services.Implements
{
    public abstract class GenericService<TEntity, TDto> : IGenericService<TDto>
    where TEntity : Entities
    {
        protected readonly IBaseRepository<TEntity> _repository;
        protected readonly IUserService _userService;

        public GenericService(IBaseRepository<TEntity> repository, IUserService userService)
        {
            _repository = repository;
            _userService = userService;
        }
        //private bool IsColumnExists(string columnName)
        //{
        //    // Cách tiếp cận này phụ thuộc vào cách bạn truy cập cơ sở dữ liệu.
        //    // Đây là một ví dụ giả định kiểm tra sự tồn tại của cột.
        //    return _repository.GetColumnNames().Contains(columnName);
        //}
        public ResponseGetDTO<TDto> GetAll(int page, string keyword)
        {
            int pageSize = 20;
            var tableName = typeof(TEntity).Name; // Lấy tên của TEntity

            var sqlQuery = @"
                IF EXISTS (
                    SELECT * 
                    FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_NAME = @TableName 
                    AND COLUMN_NAME = 'Name'
                )
                BEGIN
                    DECLARE @SQL NVARCHAR(MAX)
                    SET @SQL = 'SELECT * FROM ' + QUOTENAME(@TableName) + ' WHERE Name LIKE @Keyword'

                    EXEC sp_executesql @SQL, N'@Keyword NVARCHAR(128)', @Keyword
                END
                ELSE
                BEGIN
                    SELECT * FROM (SELECT 1 AS NoResults) AS EmptyResult
                END";

            var entities = _repository.ExcuteSQL(sqlQuery, keyword)
                .AsEnumerable()
                .Select(e => MapToDto(e))
                .ToList();

            int totalRow = entities.Count();
            int totalPage = (int)Math.Ceiling((double)totalRow / pageSize);

            entities = entities
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new ResponseGetDTO<TDto>
            {
                TotalRow = totalRow,
                TotalPage = totalPage,
                PageSize = pageSize,
                Datalist = entities
            };
        }

        ResponseGetDTO<TDto> IGenericService<TDto>.GetById(string id)
        {
            TEntity? entity = null;
            if (Guid.TryParse(id, out Guid parsedGuid))
            {
                entity = _repository.Find(r => (r.IsDeleted == false) && r.Id.Equals(parsedGuid))
                .FirstOrDefault();
            }
            return new ResponseGetDTO<TDto>()
            {
                TotalRow = entity != null ? 1 : 0,
                TotalPage = entity != null ? 1 : 0,
                Datalist = entity != null ? new List<TDto>() { MapToDto(entity) } : new List<TDto>(),
                PageSize = 20
            };
        }

        public virtual ResponseDTO GetByMa(string ma)
        {
            // Implement the method or make it abstract for specific classes
            throw new NotImplementedException();
        }

        public virtual ResponseDTO Add(TDto dto)
        {
            var errorResponse = ValidateDTO(dto);
            if (errorResponse != null) return errorResponse;

            var entityExists = getEntityByDto(dto);

            string? userId = _userService.GetCurrentUser();
            if (userId == null)
                return new ResponseDTO { Code = 400, Message = "User not exists" };

            if (entityExists == null)
            {
                TEntity entity = MapToEntity(dto);
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
            return new ResponseDTO()
            {
                Code = 400,
                Message = "Entity already exists",
                Description = null
            };
        }

        public ResponseDTO Update(TDto dto)
        {
            var errorResponse = ValidateDTO(dto);
            if (errorResponse != null) return errorResponse;

            string? userId = _userService.GetCurrentUser();
            if (userId == null)
                return new ResponseDTO { Code = 400, Message = "User not exists" };

            //var entityExists = _repository.Find(r => ((dynamic)r).Ma.Equals(((dynamic)dto).Ma.Trim().ToUpper().Replace(" ", string.Empty)))
            //    .FirstOrDefault();
            var entityExists = getEntityByDto(dto);

            if (entityExists != null)
            {
                entityExists.UpdateBy = userId;
                entityExists.UpdateDate = DateTime.Now;
                var e = UpdateEntityF(entityExists, dto);
                if (e != null)
                {
                    _repository.Update(e);
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


        public virtual ResponseDTO Delete(string ma)
        {
            if (!string.IsNullOrWhiteSpace(ma))
            {
                string? userId = _userService.GetCurrentUser();
                if (userId == null)
                    return new ResponseDTO { Code = 400, Message = "User not exists" };

                var entityExists = getEntityByMa(ma);

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

        // Abstract or virtual methods to be overridden in derived classes if necessary
        protected abstract TEntity? getEntityByMa(string ma);
        protected abstract TEntity? UpdateEntityF(TEntity entity, TDto dto);
        protected abstract TEntity? getEntityByDto(TDto dto);
        protected abstract TDto MapToDto(TEntity entity);

        protected abstract TEntity MapToEntity(TDto dto);

        protected abstract ResponseDTO? ValidateDTO(TDto dto);

    }

}
