namespace BackEnd.Db.Interfaces
{
	public interface IDateTimeTrackable
	{
		public DateTime CreationDateTime { get; set; }
		public DateTime? LastModificationDateTime { get; set; }
	}
}
