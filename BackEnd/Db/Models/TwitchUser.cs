using TwitchLib.Api.Helix.Models.Users.GetUsers;

namespace BackEnd.Db.Models
{
    public class TwitchUser
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string TwitchOwner { get; set; }
        public DateTime CreationDateTime { get; set; }
        public DateTime? LastModificationDateTime { get; set; }
        public bool Deleted { get; set; } = false;

        public TwitchUser()
        {
            Id = Guid.NewGuid();
            Name = string.Empty;
            DisplayName = string.Empty;
            TwitchOwner = string.Empty;
            CreationDateTime = DateTime.Now;
        }

        public TwitchUser(User user)
        {
            Id = Guid.NewGuid();
            Name = user.Login;
            DisplayName = user.DisplayName;
            TwitchOwner = user.Id;
            CreationDateTime = DateTime.Now;
        }
    }
}
