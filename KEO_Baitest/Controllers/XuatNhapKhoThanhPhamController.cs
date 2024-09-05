using KEO_Baitest.Data.DTOs;
using KEO_Baitest.Data.Entities;
using KEO_Baitest.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace KEO_Baitest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class XuatNhapKhoThanhPhamController : ControllerBase
    {
        private readonly IPhieuThanhPhamService _phieuThanhPhamService;
        public XuatNhapKhoThanhPhamController(IPhieuThanhPhamService phieuThanhPhamService)
        {
            _phieuThanhPhamService = phieuThanhPhamService;
        }
        [HttpPost]
        [Authorize]
        public IActionResult Add(PhieuVatPhamDTO dto)
        {
            var res = _phieuThanhPhamService.Add(dto);
            return StatusCode(res.Code, res);
        }

        [HttpGet]
        public IActionResult GetByMa(string ma)
        {
            var res = _phieuThanhPhamService.GetByMa(ma);
            return StatusCode(res.Code, res);
        }

        [HttpGet("/phieu")]
        public ResponseGetDTO<PhieuDTO> GetAllPhieu([FromQuery] DateTime? dateFrom, 
            [FromQuery] DateTime? dateTo, 
            [FromQuery] int page = 1,
            [FromQuery] bool isNhap = true)
        {
            dateFrom = dateFrom ?? DateTime.MinValue;
            dateTo = dateTo ?? DateTime.MaxValue;
            var res = _phieuThanhPhamService.GetPhieus(dateFrom, dateTo, page, isNhap);
            return res;
        }

        [HttpGet("/BC_Nhap_NhapKhoThanhPham")]
        public ResponseGetDTO<BC_NhapKhoThanhPhamDTO> GetBC_KhoThanhPham([FromQuery] DateTime? dateFrom, 
            [FromQuery] DateTime? dateTo, [FromQuery] int page = 1)
        {
            dateFrom = dateFrom ?? DateTime.MinValue;
            dateTo = dateTo ?? DateTime.MaxValue;
            return _phieuThanhPhamService.BC_NhapKhoThanhPham(dateFrom, dateTo, page);
        }
        [HttpGet("/BC_Xuat_KhoThanhPham")]
        public ResponseGetDTO<BC_XuatKhoThanhPhamDTO> GetBC_XuatKhoThanhPham([FromQuery] DateTime? dateFrom,
            [FromQuery] DateTime? dateTo, [FromQuery] int page = 1)
        {
            dateFrom = dateFrom ?? DateTime.MinValue;
            dateTo = dateTo ?? DateTime.MaxValue;
            return _phieuThanhPhamService.BC_XuatKhoThanhPham(dateFrom, dateTo, page);

        }

        [HttpGet("/BC_Xuat_Kho_Tong_Ket_ThanhPham")]
        public ResponseGetDTO<BC_N_X_T_VatTu_ThanhPhamDTO> BC_Xuat_Kho_Tong_Ket_ThanhPham([FromQuery] DateTime? dateFrom,
            [FromQuery] DateTime? dateTo, [FromQuery] int page = 1)
        {
            dateFrom = dateFrom ?? DateTime.MinValue;
            dateTo = dateTo ?? DateTime.MaxValue;
            return _phieuThanhPhamService.BC_N_X_SumThanhPham(dateFrom, dateTo, page);

        }
        [HttpGet("Export_Xuat_Nhap_Thanh_Pham")]
        public IActionResult ExportPhieuThanhPham([FromQuery] DateTime? dateFrom,
                    [FromQuery] DateTime? dateTo,
                    [FromQuery] bool isNhap = true, [FromQuery] int page = 1)
        {
            dynamic phieus = isNhap ? GetBC_KhoThanhPham(dateFrom, dateTo, page).Datalist
            : GetBC_XuatKhoThanhPham(dateFrom, dateTo, page).Datalist;

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add(isNhap ? "PhieuThanhPhamNhap" : "PhieuThanhPhamXuat");
                worksheet.Cells[1, 1].Value = "STT";
                worksheet.Cells[1, 2, 1, 4].Merge = true; // Hợp nhất các cột 2, 3, 4 cho "Mã thành phẩm"
                worksheet.Cells[1, 2].Value = "Mã thành phẩm";
                worksheet.Cells[1, 5].Value = "Tên Thành Phẩm";
                worksheet.Cells[1, 6].Value = "Đvt";
                worksheet.Cells[1, 7].Value = "Số lượng";
                worksheet.Cells[1, 8].Value = "Thời gian";
                // Tạo header
                worksheet.Cells[2, 2].Value = "Ngày nhập kho";
                worksheet.Cells[2, 3].Value = "Kỹ thuật";
                worksheet.Cells[2, 4].Value = "Kế toán";

                // Tạo viền cho header
                using (var range = worksheet.Cells[1, 1, 2, 8])
                {
                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }
                // Điền dữ liệu vào các hàng bắt đầu từ hàng 3
                for (int i = 0; i < phieus.Count; i++)
                {
                    int rowIndex = i + 3; // Dữ liệu bắt đầu từ hàng 3

                    worksheet.Cells[rowIndex, 1].Value = i + 1; // STT (Số thứ tự)
                    worksheet.Cells[rowIndex, 2].Value = phieus[i].Date; // Ngày nhập kho
                    worksheet.Cells[rowIndex, 3].Value = phieus[i].MaKyThuat; // Kỹ thuật
                    worksheet.Cells[rowIndex, 4].Value = phieus[i].MaKeToan; // Kế toán
                    worksheet.Cells[rowIndex, 5].Value = phieus[i].TenThanhPham; // Kế toán
                    worksheet.Cells[rowIndex, 6].Value = phieus[i].Dvt; // Đvt
                    worksheet.Cells[rowIndex, 7].Value = phieus[i].SoLuong; // Số lượng
                    worksheet.Cells[rowIndex, 8].Value = phieus[i].Time; // Thời gian
                    //worksheet.Cells[rowIndex, 8].Value = phieus[i].SoLot; // Số Lot
                    //worksheet.Cells[rowIndex, 9].Value = phieus[i].TenNCC; // Nhà Cung Cấp
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);
                var content = stream.ToArray();

                //Trả về file Excel
                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", isNhap ? "PhieuThanhPhamNhap.xlsx" : "PhieuThanhPhamXuat.xlsx"); //PhieuVatTu.xlsx

            }
            //return null;
        }

        [HttpGet("Export_BC_Xuat_Kho_Tong_Ket_ThanhPham")]
        public IActionResult Export_BC_Xuat_Kho_Tong_Ket_ThanhPham([FromQuery] DateTime? dateFrom,
                    [FromQuery] DateTime? dateTo,[FromQuery] int page = 1)
        {
            var phieus = BC_Xuat_Kho_Tong_Ket_ThanhPham(dateFrom, dateTo, page).Datalist;

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("N_X_T_ThanhPham");
                worksheet.Cells[1, 1].Value = "STT";
                worksheet.Cells[1, 2, 1, 3].Merge = true; // Hợp nhất các cột 2, 3, 4 cho "Mã thành phẩm"
                worksheet.Cells[1, 2].Value = "Mã thành phẩm";
                worksheet.Cells[1, 4].Value = "Tên Thành Phẩm";
                worksheet.Cells[1, 5].Value = "Đvt";
                worksheet.Cells[1, 6, 1, 9].Merge = true;
                worksheet.Cells[1, 6].Value = "Số Lượng";
                // Tạo header
                worksheet.Cells[2, 2].Value = "Kỹ thuật";
                worksheet.Cells[2, 3].Value = "Kế toán";

                worksheet.Cells[2, 6].Value = "Đầu kỳ";
                worksheet.Cells[2, 7].Value = "Nhập"; 
                worksheet.Cells[2, 8].Value = "Xuất";
                worksheet.Cells[2, 9].Value = "Tồn";

                // Tạo viền cho header
                using (var range = worksheet.Cells[1, 1, 2, 9])
                {
                    range.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    range.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                }
                // Điền dữ liệu vào các hàng bắt đầu từ hàng 3
                int rowIndex = 3; // Bắt đầu từ hàng 3
                for (int i = 0; i < phieus.Count; i++)
                {
                    var nhom = phieus[i]; // Nhóm thành phẩm
                                          // Thêm row cho tên nhóm thành phẩm
                    worksheet.Cells[rowIndex, 1].Value = nhom.Name;
                    worksheet.Cells[rowIndex, 1, rowIndex, 8].Merge = true; // Gộp các cột từ 1 đến 8 để hiển thị tên nhóm
                    worksheet.Cells[rowIndex, 1].Style.Font.Bold = true; // In đậm
                    rowIndex++; // Tăng index cho hàng mới

                    // Điền chi tiết từng vật tư trong nhóm
                    var tvtDetails = nhom.TVT;
                    for (int j = 0; j < tvtDetails.Count; j++)
                    {
                        worksheet.Cells[rowIndex, 1].Value = (j + 1).ToString(); // Số thứ tự
                        worksheet.Cells[rowIndex, 2].Value = tvtDetails[j].MaKyThuat; // Mã kỹ thuật
                        worksheet.Cells[rowIndex, 3].Value = tvtDetails[j].MaKeToan; // Mã kế toán
                        worksheet.Cells[rowIndex, 4].Value = tvtDetails[j].TenVatTu; // Tên vật tư
                        worksheet.Cells[rowIndex, 5].Value = tvtDetails[j].DVT; // Đvt
                        worksheet.Cells[rowIndex, 6].Value = tvtDetails[j].DauKy; // Đvt
                        worksheet.Cells[rowIndex, 7].Value = tvtDetails[j].Nhap; // Số lượng nhập
                        worksheet.Cells[rowIndex, 8].Value = tvtDetails[j].Xuat; // Số lượng xuất
                        worksheet.Cells[rowIndex, 9].Value = tvtDetails[j].Ton; // Số lượng tồn
                        rowIndex++; // Tăng index cho hàng mới
                    }
                }

                var stream = new MemoryStream();
                package.SaveAs(stream);
                var content = stream.ToArray();

                //Trả về file Excel
                return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "N_X_T_ThanhPham.xlsx");

            }
            //return null;
        }

        [HttpDelete]
        [Authorize]
        public IActionResult Delete(Guid ma)
        {
            var res = _phieuThanhPhamService.Delete(ma.ToString());
            return StatusCode(res.Code, res);
        }

        [HttpPut]
        [Authorize]
        public IActionResult Update(PhieuVatTuUpdateDTO update)
        {
            var res = _phieuThanhPhamService.Update(update);
            return StatusCode(res.Code, res);
        }
    }
}
