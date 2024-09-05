using KEO_Baitest.Data.DTOs;
using KEO_Baitest.Services;
using KiemTraThuViec1.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KEO_Baitest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThanhPhamController : GenericController<ThanhPhamDTO>
    {
        public ThanhPhamController(IGenericService<ThanhPhamDTO> service) : base(service)
        {
        }
    }
}
