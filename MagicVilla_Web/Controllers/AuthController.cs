using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;
using Utilities;

namespace MagicVilla_Web.Controllers
{
	public class AuthController : Controller
	{
		private readonly IAuthService _authService;
		public AuthController(IAuthService authService)
		{
			_authService = authService;
		}

		[HttpGet]
		public IActionResult Login()
		{
			LoginRequestDTO obj = new();
			return View(obj);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginRequestDTO obj)
		{
			APIResponse ApiResponse = await _authService.LoginAsync<APIResponse>(obj);
			if(ApiResponse.isSuccess && ApiResponse != null)
			{
				LoginResponseDTO model = JsonConvert.DeserializeObject<LoginResponseDTO>(Convert.ToString(ApiResponse.Result));
				HttpContext.Session.SetString(SD.SessionToken, model.Token);

				ClaimsIdentity claimsIdentity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
				claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, model.User.UserName));
				claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, model.User.Role));

				ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
				await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,claimsPrincipal);

				return RedirectToAction("Index", "Home");
			}
			else
			{
				ModelState.AddModelError("CustomError", ApiResponse.ErrorMessages.FirstOrDefault());
				return View(obj);
			}
		}
		[HttpGet]
		public IActionResult Register()
		{
			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegistrationRequestDTO obj)
		{
			APIResponse result = await _authService.RegisterAsync<APIResponse>(obj);
			if(result.isSuccess&& result != null)
			{
				RedirectToAction(nameof(Login));
			}
			return View(result);
		}
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync();
			HttpContext.Session.SetString(SD.SessionToken, "");
			return RedirectToAction("Index","Home");
		}
		public IActionResult AccessDenied()
		{
			return View();
		}
	}
}
