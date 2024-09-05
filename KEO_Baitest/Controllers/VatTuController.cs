using KEO_Baitest.Data.DTOs;
using KEO_Baitest.Services;
using KiemTraThuViec1.Controllers;

namespace KEO_Baitest.Controllers
{
    public class VatTuController : GenericController<VatTuDTO>
    {
        public VatTuController(IGenericService<VatTuDTO> service) : base(service)
        {
        }
    }
}
