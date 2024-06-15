using BackEnd.Db.Models;

namespace BackEnd.DTO
{
	public class UserSignInResponse(TwitchStreamer user, string token)
	{
		public Guid Id { get; set; } = user.Id;
		public string Token { get; set; } = token;
	}
}
