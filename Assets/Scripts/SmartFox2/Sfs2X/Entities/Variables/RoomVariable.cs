namespace Sfs2X.Entities.Variables
{
	public interface RoomVariable : UserVariable
	{
		bool IsPrivate { get; set; }

		bool IsPersistent { get; set; }
	}
}
