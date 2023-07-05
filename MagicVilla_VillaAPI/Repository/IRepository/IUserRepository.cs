using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;

namespace MagicVilla_VillaAPI.Repository.IRepository
{
	public interface IUserRepository
	{
		bool isUserNameUnique(string userName);
		Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO);
		Task<LocalUser> RegisterUser(RegistrationRequestDTO registrationRequestDTO);
	}
}
