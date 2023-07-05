using MagicVilla_Web.Models;
using MagicVilla_Web.Models.DTO;
using MagicVilla_Web.Services.IServices;
using static Utilities.SD;

namespace MagicVilla_Web.Services
{
	public class AuthService : BaseService, IAuthService
	{
		private readonly IHttpClientFactory _clientFactory;
		private string villaUrl;
		public AuthService(IHttpClientFactory httpClient, IConfiguration configuration) : base(httpClient)
		{
			_clientFactory = httpClient;
			villaUrl = configuration.GetValue<string>("ServiceUrls:VillaApi");
		}

		public Task<T> LoginAsync<T>(LoginRequestDTO loginRequestDTO)
		{
			return SendAsync<T>(new APIRequest()
			{
				Url = villaUrl + "api/UserAuthentication/Login",
				ApiType = ApiType.POST,
				data = loginRequestDTO
			});
		}

		public Task<T> RegisterAsync<T>(RegistrationRequestDTO registrationRequestDTO)
		{
			return SendAsync<T>(new APIRequest()
			{
				Url = villaUrl + "api/UserAuthentication/Register",
				ApiType = ApiType.POST,
				data = registrationRequestDTO
			});
		}
	}
}
