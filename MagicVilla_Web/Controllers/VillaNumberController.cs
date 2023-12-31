﻿using AutoMapper;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Models.VM;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using Utilities;

namespace MagicVilla_Web.Controllers
{
	public class VillaNumberController : Controller
	{
		private readonly IMapper _mapper;
		private readonly IVillaNumberService _villaNumberService;
		private readonly IVillaService _villaService;

		public VillaNumberController(IMapper mapper, IVillaNumberService villaNumberService, IVillaService villaService)
		{
			_mapper = mapper;
			_villaNumberService = villaNumberService;
			_villaService = villaService;
		}
		public async Task<IActionResult> IndexVillaNumber()
		{
			List<VillaNumberDTO> list = new();
			var response = await _villaNumberService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
			if(response != null && response.isSuccess)
			{
				list = JsonConvert.DeserializeObject<List<VillaNumberDTO>>(Convert.ToString(response.Result));
            }
			return View(list);
        }
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateVillaNumber()
		{
			VillaNumberCreateVM villaNumberCreateVM = new ();
			var response = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
			if (response != null && response.isSuccess)
			{
				villaNumberCreateVM.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
					(Convert.ToString(response.Result)).Select(i => new SelectListItem
					{
						Text = i.Name,
						Value = i.Id.ToString()
					});
			}
			return View(villaNumberCreateVM);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateVillaNumber(VillaNumberCreateVM villaNumberCreateVM)
		{

			if (ModelState.IsValid)
			{
				var response = await _villaNumberService.CreateAsync<APIResponse>(villaNumberCreateVM.VillaNumber, HttpContext.Session.GetString(SD.SessionToken));
				if (response != null && response.isSuccess)
				{
					return RedirectToAction(nameof(IndexVillaNumber));
				}
				else
				{
					if(response.ErrorMessages.Count > 0)
						ModelState.AddModelError("ErrorMessages",response.ErrorMessages.FirstOrDefault());
				}
			}
			VillaNumberCreateVM villaNumberVM = new();
			var responseFail = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
			if (responseFail != null && responseFail.isSuccess)
			{
				villaNumberVM.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
					(Convert.ToString(responseFail.Result)).Select(i => new SelectListItem
					{
						Text = i.Name,
						Value = i.Id.ToString()
					});
			}
			return View(villaNumberVM);
		}


        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateVillaNumber(int villaNo)
		{
			VillaNumberUpdateVM villaNumberUpdateVM = new();
			var response = await _villaNumberService.GetAsync<APIResponse>(villaNo, HttpContext.Session.GetString(SD.SessionToken));
			if (response != null && response.isSuccess)
			{
				VillaNumberDTO model = JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(response.Result));
				villaNumberUpdateVM.VillaNumber = _mapper.Map<VillaNumberUpdateDTO>(model);
			}
			response = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
			if (response != null && response.isSuccess)
			{
				villaNumberUpdateVM.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
					(Convert.ToString(response.Result)).Select(i => new SelectListItem
					{
						Text = i.Name,
						Value = i.Id.ToString()
					});
				return View(villaNumberUpdateVM);
			}
			return NotFound();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateVillaNumber(VillaNumberUpdateVM model)
		{
			if (ModelState.IsValid)
			{
				var response = await _villaNumberService.UpdateAsync<APIResponse>(model.VillaNumber, HttpContext.Session.GetString(SD.SessionToken));
				if (response != null && response.isSuccess)
				{
					return RedirectToAction(nameof(IndexVillaNumber));
				}
				else
				{
					if (response.ErrorMessages.Count > 0)
						ModelState.AddModelError("ErrorMessages", response.ErrorMessages.FirstOrDefault());
				}
			}
			VillaNumberUpdateVM villaNumberVM = new();
			var responseFail = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
			if (responseFail != null && responseFail.isSuccess)
			{
				villaNumberVM.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
					(Convert.ToString(responseFail.Result)).Select(i => new SelectListItem
					{
						Text = i.Name,
						Value = i.Id.ToString()
					});
			}
			return View(villaNumberVM);
        }
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteVillaNumber(int villaNo)
		{
			VillaNumberDeleteVM villaNumberUpdateVM = new();
			var response = await _villaNumberService.GetAsync<APIResponse>(villaNo, HttpContext.Session.GetString(SD.SessionToken));
			if (response != null && response.isSuccess)
			{
				VillaNumberDTO model = JsonConvert.DeserializeObject<VillaNumberDTO>(Convert.ToString(response.Result));
				villaNumberUpdateVM.VillaNumber = model;
			}
			response = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
			if (response != null && response.isSuccess)
			{
				villaNumberUpdateVM.VillaList = JsonConvert.DeserializeObject<List<VillaDTO>>
					(Convert.ToString(response.Result)).Select(i => new SelectListItem
					{
						Text = i.Name,
						Value = i.Id.ToString()
					});
				return View(villaNumberUpdateVM);
			}
			return NotFound();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteVillaNumber(VillaNumberDeleteVM model)
		{
			var response = await _villaNumberService.DeleteAsync<APIResponse>(model.VillaNumber.VillaNo, HttpContext.Session.GetString(SD.SessionToken));
			if (response != null && response.isSuccess)
			{
				return RedirectToAction(nameof(IndexVillaNumber));
			}
			return View(model);
		}
	}
}
