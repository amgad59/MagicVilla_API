using AutoMapper;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.DTO;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
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
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private string secretKey;
		public UserRepository(ApplicationDbContext db,IMapper mapper,IConfiguration configuration
			,UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager) 
		{
			_db = db;
			_mapper = mapper;
			_userManager = userManager;
			_roleManager = roleManager;
			secretKey = configuration.GetValue<string>("ApiSettings:Secret");
		}

		public bool isUserNameUnique(string userName)
		{
			if(_db.applicationUsers.FirstOrDefault(u=>u.UserName == userName) == null)
				return true;
			return false;
		}

		public async Task<LoginResponseDTO> Login(LoginRequestDTO loginRequestDTO)
		{
			var user = await _db.applicationUsers.FirstOrDefaultAsync(u => u.UserName.ToLower() == loginRequestDTO.UserName.ToLower());
			
			bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);

			if (user == null || !isValid)
				return new LoginResponseDTO { User = null,Token=""};

			//else generate JWT token

			var roles = await _userManager.GetRolesAsync(user);
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.ASCII.GetBytes(secretKey);

			var TokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new Claim[]
				{
					new Claim(ClaimTypes.Name, user.UserName.ToString()),
					new Claim(ClaimTypes.Role, roles.FirstOrDefault())
				}),
				Expires = DateTime.UtcNow.AddDays(7),
				SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
			};
			var token = tokenHandler.CreateToken(TokenDescriptor);

			LoginResponseDTO loginResponseDTO = new LoginResponseDTO
			{
				Token = tokenHandler.WriteToken(token),
				User = _mapper.Map<UserDTO>(user)
			};
			return loginResponseDTO;
		}

		public async Task<UserDTO> RegisterUser(RegistrationRequestDTO registrationRequestDTO)
		{
			ApplicationUser user = new()
			{
				UserName = registrationRequestDTO.UserName,
				Email = registrationRequestDTO.UserName,
				NormalizedEmail = registrationRequestDTO.UserName.ToUpper(),
				name = registrationRequestDTO.Name
			};

			try
			{
				var result = await _userManager.CreateAsync(user, registrationRequestDTO.Password);
				if (result.Succeeded)
				{
					if(!await _roleManager.RoleExistsAsync("admin"))
					{
						await _roleManager.CreateAsync(new IdentityRole("admin"));
						await _roleManager.CreateAsync(new IdentityRole("customer"));
					}
					await _userManager.AddToRoleAsync(user, "admin");
					var userToReturn = _db.applicationUsers
						.FirstOrDefault(u => u.UserName == registrationRequestDTO.UserName);
					return _mapper.Map<UserDTO> (userToReturn);
				}
			}
			catch(Exception ex)
			{

			}
			return new UserDTO();
		}
	}
}
