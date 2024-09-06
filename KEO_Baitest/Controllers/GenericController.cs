using KEO_Baitest.Data.DTOs;
using KEO_Baitest.Services;
using KiemTraThuViec1.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KiemTraThuViec1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenericController<TDto> : ControllerBase
    {
        protected readonly IGenericService<TDto> _service;

        public GenericController(IGenericService<TDto> service)
        {
            _service = service;
        }


        [HttpGet]
        public virtual ResponseGetDTO<TDto> GetAll(int page = 1, string keyword = "")
        {
            var res = _service.GetAll(page, keyword);
            return res;
        }

        [HttpGet("/[controller]/{id}")]
        public virtual ResponseGetDTO<TDto> GetById(string id)
        {
            var res = _service.GetById(id);
            return res;
        }

        [HttpPost]
        [Authorize]
        public IActionResult Add(TDto dto)
        {
            var res = _service.Add(dto);
            return StatusCode(res.Code, res);
        }

        [HttpPut]
        [Authorize]
        public IActionResult Update(TDto dto)
        {
            var res = _service.Update(dto);
            return StatusCode(res.Code, res);
        }

        [HttpDelete]
        [Authorize]
        public IActionResult Delete(string id)
        {
            var res = _service.Delete(id);
            return StatusCode(res.Code, res);
        }
    }
}
