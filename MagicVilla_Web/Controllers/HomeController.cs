using AutoMapper;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using Utilities;

namespace MagicVilla_Web.Controllers
{
    public class HomeController : Controller
	{
		private readonly IMapper _mapper;
		private readonly IVillaService _villaService;

		public HomeController(IMapper mapper, IVillaService villaService)
		{
			_mapper = mapper;
			_villaService = villaService;
		}
		public async Task<IActionResult> Index()
		{
			List<VillaDTO> list = new();
			var response = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));
			if (response != null && response.isSuccess)
			{
				list = JsonConvert.DeserializeObject<List<VillaDTO>>(Convert.ToString(response.Result));
			}
			return View(list);
		}

		public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}