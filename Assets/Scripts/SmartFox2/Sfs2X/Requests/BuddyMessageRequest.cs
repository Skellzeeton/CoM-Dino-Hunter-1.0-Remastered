using Sfs2X.Entities;
using Sfs2X.Entities.Data;

namespace Sfs2X.Requests
{
	public class BuddyMessageRequest : GenericMessageRequest
	{
		public BuddyMessageRequest(string message, Buddy targetBuddy)
			: this(message, targetBuddy, null)
		{
		}

		public BuddyMessageRequest(string message, Buddy targetBuddy, ISFSObject parameters)
		{
			type = 5;
			base.message = message;
			recipient = ((targetBuddy == null) ? (-1) : targetBuddy.Id);
			base.parameters = parameters;
		}
	}
}
