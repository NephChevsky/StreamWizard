namespace BackEnd.DTO
{
	public class UserOAuthResponse(string url)
	{
		public string AuthorizationUrl { get; set; } = url;
	}
}
