using TwitchLib.Api.Helix.Models.Users.GetUsers;

namespace BackEnd.Db.Models
{
    public class TwitchStreamer : TwitchUser
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime LastLoginDateTime { get; set; }

        public TwitchStreamer() : base()
        {
        }

        public TwitchStreamer(User user, string accessToken, string refreshToken) : base(user)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
        }
    }
}
