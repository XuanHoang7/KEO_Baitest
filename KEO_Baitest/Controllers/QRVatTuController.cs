﻿using KEO_Baitest.Data.DTOs;
using KEO_Baitest.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KEO_Baitest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QRVatTuController : ControllerBase
    {
        private readonly IQRVatTuService _qRVatTuService;
        public QRVatTuController(IQRVatTuService qRVatTuService)
        {
            _qRVatTuService = qRVatTuService;
        }

        [HttpGet]
        public ResponseGetDTO<QRVatTuO> GetSanPhamByMa(int page = 1, string keyword = "")
        {
            var res = _qRVatTuService.GetAll(page, keyword);
            return res;
        }

        [HttpPost]
        [Authorize]
        public IActionResult Add(QRVatTuE dto)
        {
            var res = _qRVatTuService.Add(dto);
            return StatusCode(res.Code, res);
        }
        [HttpDelete]
        [Authorize]
        public IActionResult DeletePVTDetails(string id)
        {
            var res = _qRVatTuService.Delete(id);
            return StatusCode(res.Code, res);
        }

        [HttpPut]
        [Authorize]
        public IActionResult Update(UpdateQRVatTuDTO update)
        {
            var res = _qRVatTuService.Update(update);
            return StatusCode(res.Code, res);
        }
    }
}
