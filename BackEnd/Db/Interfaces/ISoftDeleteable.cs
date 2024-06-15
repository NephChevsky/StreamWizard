namespace BackEnd.Db.Interfaces
{
	public interface ISoftDeleteable
	{
		public bool Deleted { get; set; }
	}
}
