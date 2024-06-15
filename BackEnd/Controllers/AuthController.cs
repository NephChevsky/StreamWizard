using BackEnd.Db;
using BackEnd.Db.Models;
using BackEnd.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TwitchLib.Api.Helix.Models.Users.GetUsers;

namespace BackEnd.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class AuthController(IConfiguration configuration, StreamWizardDbContext db) : ControllerBase
	{
		private readonly IConfiguration _configuration = configuration;
		private readonly StreamWizardDbContext _db = db;

		[AllowAnonymous]
		[HttpPost("SignIn")]
		public async Task<ActionResult<UserSignInResponse>> SignIn([FromBody] UserOAuthQuery query)
		{
			TwitchApi.TwitchApi api = new(_configuration);
			if (!string.IsNullOrEmpty(query.Code))
			{
				(string accessToken, string refreshToken) = await api.GetOAuthTokens(query.Code);
				User twitchUser = await api.GetUser(accessToken);
				TwitchStreamer dbUser = _db.TwitchStreamers.Where(x => x.TwitchOwner == twitchUser.Id).FirstOrDefault();
				if (dbUser == null)
				{
					dbUser = new(twitchUser, accessToken, refreshToken);
					_db.TwitchStreamers.Add(dbUser);
				}
				dbUser.LastLoginDateTime = DateTime.Now;
				await _db.SaveChangesAsync();
				return Ok(new UserSignInResponse(dbUser, GetJwtToken(dbUser)));
			}
			else
			{
				return BadRequest();
			}

			/*TwitchStreamer dbUser = null;
			if (Guid.TryParse(user.Id, out Guid id))
			{
				_db.TwitchStreamers.Where(x => x.Id == Guid.Parse(user.Id)).FirstOrDefault();
			}
			if (dbUser == null)
			{
				dbUser = new();
			}
			
			(dbUser.AccessToken, dbUser.RefreshToken) = await api.CheckAndUpdateTokenStatus(dbUser.AccessToken, dbUser.RefreshToken);
			*/
		}

		[AllowAnonymous]
		[HttpGet("GetAuthorizationUrl")]
		public UserOAuthResponse GetAuthorizationUrl()
		{
            TwitchApi.TwitchApi api = new(_configuration);
			return new UserOAuthResponse(api.GetAuthorizationUrl());
		}

		private string GetJwtToken(TwitchStreamer user)
		{
			JwtSecurityTokenHandler tokenHandler = new();
			var key = Encoding.UTF8.GetBytes(_configuration["JwtKey"]);
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new Claim[]
				{
					new(ClaimTypes.Name, user.Id.ToString())
				}),
				Expires = DateTime.UtcNow.AddHours(4),
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
			};
			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}
	}
}
