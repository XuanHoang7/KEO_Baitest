using KEO_Baitest.Data.DTOs;
using KEO_Baitest.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KEO_Baitest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QRThanhPhamController : ControllerBase
    {
        private readonly IQRThanhPhamService _qRThanhPhamService;
        public QRThanhPhamController(IQRThanhPhamService qRThanhPhamService)
        {
            _qRThanhPhamService = qRThanhPhamService;
        }

        [HttpGet]
        public ResponseGetDTO<QRThanhPhamO> GetSanPhamByMa(int page = 1, string keyword = "")
        {
            var res = _qRThanhPhamService.GetAll(page, keyword);
            return res;
        }

        [HttpPost]
        [Authorize]
        public IActionResult Add(QRThanhPhamE dto)
        {
            var res = _qRThanhPhamService.Add(dto);
            return StatusCode(res.Code, res);
        }
        [HttpDelete]
        [Authorize]
        public IActionResult DeletePVTDetails(string ma)
        {
            var res = _qRThanhPhamService.Delete(ma);
            return StatusCode(res.Code, res);
        }
    }
}
