using Microsoft.Extensions.Configuration;
using TwitchLib.Api;
using TwitchLib.Api.Auth;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Helix.Models.Users.GetUsers;

namespace TwitchApi
{
	public class TwitchApi
	{
		private readonly IConfiguration _configuration;
		private readonly TwitchAPI _api;

		public TwitchApi(IConfiguration configuration)
		{
			_configuration = configuration;
			_api = new TwitchAPI();
			_api.Settings.ClientId = _configuration["Twitch:ClientId"];
			_api.Settings.Secret = _configuration["Twitch:ClientSecret"];
		}

		public string GetAuthorizationUrl()
		{
			string redirectUrl = _configuration["Twitch:RedirectUrl"];
			List<string> scopes = _configuration.GetSection("Twitch:Scopes").Get<List<string>>();
			List<AuthScopes> authScopes = [];
			foreach (string scope in scopes)
			{
				authScopes.Add((AuthScopes)Enum.Parse(typeof(AuthScopes), scope));
			}
			return _api.Auth.GetAuthorizationCodeUrl(redirectUrl, authScopes);
		}

		public async Task<(string, string)> GetOAuthTokens(string code)
		{
			string redirectUrl = _configuration["Twitch:RedirectUrl"];
			AuthCodeResponse resp = await _api.Auth.GetAccessTokenFromCodeAsync(code, _api.Settings.Secret, redirectUrl);
			return (resp.AccessToken, resp.RefreshToken);
		}

		public async Task<User> GetUser(string accessToken)
		{
			return (await _api.Helix.Users.GetUsersAsync(null, null, accessToken)).Users[0];
		}
	}
}
