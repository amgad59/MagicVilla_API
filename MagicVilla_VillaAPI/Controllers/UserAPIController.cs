using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla_VillaAPI.Controllers
{
	[ApiController]
	[Route("api/UserAuthentication")]
	public class UserAPIController : Controller
	{
		private readonly IUserRepository _userRepository;
		protected APIResponse _response;
        public UserAPIController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
			_response = new APIResponse();
        }

		[HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequestDTO)
		{
			LoginResponseDTO loginResponseDTO = await _userRepository.Login(loginRequestDTO);
			if (loginResponseDTO.User == null || string.IsNullOrEmpty(loginResponseDTO.Token))
			{
				_response.isSuccess = false;
				_response.ErrorMessages.Add("username or password incorrect");
				_response.StatusCode = HttpStatusCode.BadRequest;
				return BadRequest(_response);
			}

			_response.Result = loginResponseDTO;
			_response.isSuccess = true;
			_response.StatusCode = HttpStatusCode.OK;
			return Ok(_response);
		}
		[HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO model)
		{
			bool isUserUnique = _userRepository.isUserNameUnique(model.UserName);
			if(!isUserUnique)
			{
				_response.ErrorMessages.Add("username already exists");
				_response.StatusCode = HttpStatusCode.BadRequest;
				_response.isSuccess = false;
				return BadRequest(_response);
			}
			LocalUser user = await _userRepository.RegisterUser(model);
			if (user == null)
			{
				_response.ErrorMessages.Add("error registering ");
				_response.StatusCode = HttpStatusCode.BadRequest;
				_response.isSuccess = false;
				return BadRequest(_response);
			}
			_response.isSuccess = true;
			_response.StatusCode = HttpStatusCode.OK;
			return Ok(_response);
		}
	}
}
