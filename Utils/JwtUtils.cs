using Microsoft.IdentityModel.Tokens;
using PRN_ProjectAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PRN_ProjectAPI.Utils
{
	public class JwtUtils
	{
		private readonly IConfiguration _configuration;
		public JwtUtils(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public string GenerateToken(PhoneNumber phone)
		{
			var handler = new JwtSecurityTokenHandler();
			var key = Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]);
			var credentials = new SigningCredentials(
				new SymmetricSecurityKey(key),
				SecurityAlgorithms.HmacSha256Signature);

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = GenerateClaims(phone),
				Expires = DateTime.UtcNow.AddMinutes(15),
				Issuer = _configuration["JwtSettings:Issuer"],
				Audience = _configuration["JwtSettings:Audience"].Trim(),
				SigningCredentials = credentials,
			};

			var token = handler.CreateToken(tokenDescriptor);
			return handler.WriteToken(token);
		}

		private static ClaimsIdentity GenerateClaims(PhoneNumber phone)
		{
			var claims = new ClaimsIdentity();
			claims.AddClaim(new Claim(ClaimTypes.MobilePhone, phone.PhoneNumber1));
			claims.AddClaim(new Claim(ClaimTypes.Role, "User"));

			return claims;
		}
	}
}
