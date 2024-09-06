using KEO_Baitest.Data.DTOs;
using KiemTraThuViec1.Data;

namespace KEO_Baitest.Services
{
    public interface IGenericService<TDto>
    {
        ResponseDTO GetByMa(string ma);
        ResponseGetDTO<TDto> GetAll(int page, string keyword);
        ResponseDTO Add(TDto dto);
        ResponseDTO Update(TDto entity);
        ResponseDTO Delete(string ma);
        ResponseGetDTO<TDto> GetById(string id);
    }

    
}
