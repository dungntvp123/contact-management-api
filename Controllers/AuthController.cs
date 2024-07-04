using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PRN_ProjectAPI.Models;
using PRN_ProjectAPI.Utils;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace PRN_ProjectAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[AllowAnonymous]
	public class AuthController : ControllerBase
	{
		private readonly PhoneBookDbContext _context;
		private readonly JwtUtils _jwtUtils;

		public AuthController(PhoneBookDbContext context, JwtUtils jwtUtils)
		{
			_context = context;
			_jwtUtils = jwtUtils;
		}

		[HttpPost("Authenticate")]
		public async Task<IActionResult> Authenticate([FromForm] UsernamePasswordAuthenticateRequest request)
		{
			if (request == null)
			{
				return BadRequest();
			}
			
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			if (!_context.PhoneNumbers.Any(p => p.PhoneNumber1.Replace("-", "") == request.PhoneNunber.Replace("-", "")))
			{
				var p = new PhoneNumber
				{
					PhoneNumber1 = (request.PhoneNunber.Length == 10 ? request.PhoneNunber.Substring(0, 3) + "-" + request.PhoneNunber.Substring(3, 3) + "-" + request.PhoneNunber.Substring(6) : request.PhoneNunber),
					Password = request.Password,
				};
				_context.Add(p);
				await _context.SaveChangesAsync();
				var jwt = _jwtUtils.GenerateToken(p);
				return Ok(new {token = jwt});
			}
			var phoneNumber = _context.PhoneNumbers.SingleOrDefault(p => p.PhoneNumber1.Replace("-", "") == request.PhoneNunber.Replace("-", "") && p.Password == request.Password);
			if (phoneNumber == null)
			{
				return Unauthorized();
			}
			var token = _jwtUtils.GenerateToken(phoneNumber);
			return Ok(new {token = token});
		}
		[Authorize]
		[HttpGet("GetRefreshToken")]
		public async Task<IActionResult> GetRefreshToken()
		{
			string phone = null;
			await Task.Run(() =>
			{
				var currentUser = HttpContext.User;
				if (currentUser.HasClaim(c => c.Type == ClaimTypes.MobilePhone))
				{
					phone = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.MobilePhone).Value;
				}
			});
			if (phone == null)
			{
				return Unauthorized();
			}
			var token = _jwtUtils.GenerateToken(new PhoneNumber() { PhoneNumber1 = phone });
			return Ok(new { token = token });
		}
	}

	public class UsernamePasswordAuthenticateRequest
	{
		[Required]
		[RegularExpression("^([0-9]{10}|([0-9]{3}-[0-9]{3}-[0-9]{4}))$")]
		public string? PhoneNunber { get; set; }
		[Required]
		public string? Password { get; set; }
	}
}
