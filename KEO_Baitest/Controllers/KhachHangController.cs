using KEO_Baitest.Data.DTOs;
using KEO_Baitest.Services.Interfaces;
using KiemTraThuViec1.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace KEO_Baitest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KhachHangController : GenericController<KhachHangDTO>
    {
        private readonly IKhachHangService _khachHangService;

        public KhachHangController(IKhachHangService khachHangService)
            : base(khachHangService) 
        {
            _khachHangService = khachHangService;
        }
    }
}
