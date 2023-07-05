using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MagicVilla_VillaAPI.Repository
{
	public class UserRepository : IUserRepository
	{
		private readonly ApplicationDbContext _db;
		private readonly IMapper _mapper;
		private string secretKey;
		public UserRepository(ApplicationDbContext db,IMapper mapper,IConfiguration configuration) 
		{
			_db = db;
			_mapper = mapper;
			secretKey = configuration.GetValue<string>("ApiSettings:Secret");
		}

		public bool isUserNameUnique(string userName)
		{
			if(_db.Users.FirstOrDefault(u=>u.UserName == userName) == null)
				return true;
			return false;
		}

		public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
		{
			var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName.ToLower() == loginRequestDTO.UserName.ToLower()
			&& u.Password ==loginRequestDTO.Password);
			if (user == null)
				return new LoginResponseDTO { User = null,Token=""};

			//else generate JWT token

			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(secretKey);

			var TokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new Claim[]
				{
					new Claim(ClaimTypes.Name, user.Id.ToString()),
					new Claim(ClaimTypes.Role, user.Role)
				}),
				Expires = DateTime.UtcNow.AddDays(7),
				SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
			};
			var token = tokenHandler.CreateToken(TokenDescriptor);

			LoginResponseDTO loginResponseDTO = new LoginResponseDTO
			{
				Token = tokenHandler.WriteToken(token),
				User = user
			};
			return loginResponseDTO;
		}

		public async Task<LocalUser> RegisterUser(RegistrationRequestDTO registrationRequestDTO)
		{
			LocalUser user = _mapper.Map<LocalUser>(registrationRequestDTO);
			await _db.Users.AddAsync(user);
			await _db.SaveChangesAsync();
			user.Password = "";
			return user;
		}
	}
}
