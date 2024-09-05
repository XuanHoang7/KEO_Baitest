using KEO_Baitest.Data.DTOs;
using KEO_Baitest.Services;
using KiemTraThuViec1.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace KEO_Baitest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KhoThanhPhamController : GenericController<KhoThanhPhamDTO>
    {
        public KhoThanhPhamController(IGenericService<KhoThanhPhamDTO> service) : base(service)
        {
        }
    }
}
