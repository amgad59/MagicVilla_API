using AutoMapper;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Utilities;

namespace MagicVilla_Web.Controllers
{
    public class VillaController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IVillaService _villaService;

        public VillaController(IMapper mapper, IVillaService villaService)
        {
            _mapper = mapper;
            _villaService = villaService;
        }
        public async Task<IActionResult> IndexVilla()
        {
            List<VillaDTO> list = new();
            var response = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
            if(response != null && response.isSuccess)
            {
                list = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result));
            }
            return View(list);
        }
        [Authorize(Roles ="admin")]
        public async Task<IActionResult> CreateVilla()
        {
            return View();
        }
        [HttpPost]
		[ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateVilla(VillaCreateDTO villaCreateDTO)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaService.CreateAsync<APIResponse>(villaCreateDTO,HttpContext.Session.GetString(SD.SessionToken));
                if (response != null && response.isSuccess)
                {
                    return RedirectToAction(nameof(IndexVilla));
                }
            }
            return View(villaCreateDTO);
        }
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateVilla(int villaId)
		{
			var response = await _villaService.GetAsync<APIResponse>(villaId, HttpContext.Session.GetString(SD.SessionToken));
			if (response != null && response.isSuccess)
			{
                VillaDTO villaDTO = JsonConvert.DeserializeObject<VillaDTO>(Convert.ToString(response.Result));
				return View(_mapper.Map<VillaUpdateDTO>(villaDTO));
			}
			return NotFound();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateVilla(VillaUpdateDTO villaUpdateDTO)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaService.UpdateAsync<APIResponse>(villaUpdateDTO, HttpContext.Session.GetString(SD.SessionToken));
                if (response != null && response.isSuccess)
                {
                    return RedirectToAction(nameof(IndexVilla));
                }
            }
            return View(villaUpdateDTO);
        }
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteVilla(int villaId)
		{
			var response = await _villaService.GetAsync<APIResponse>(villaId, HttpContext.Session.GetString(SD.SessionToken));
			if (response != null && response.isSuccess)
			{
                VillaDTO villaDTO = JsonConvert.DeserializeObject<VillaDTO>(Convert.ToString(response.Result));
				return View(villaDTO);
			}
			return NotFound();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteVilla(VillaDTO villaDTO)
        {
                var response = await _villaService.DeleteAsync<APIResponse>(villaDTO.Id, HttpContext.Session.GetString(SD.SessionToken));
                if (response != null && response.isSuccess)
                {
                    return RedirectToAction(nameof(IndexVilla));
                }
            return View(villaDTO);
        }
    }
}
