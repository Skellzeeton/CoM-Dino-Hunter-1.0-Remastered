using TNetSdk.BinaryProtocol;
using UnityEngine;

namespace TNetSdk
{
	public class RoomCreaterChangeNotifyCmd : UnPacker
	{
		public ushort m_user_id;

		public override bool ParserPacket(Packet packet)
		{
			if (!base.ParserPacket(packet))
			{
				return false;
			}
			if (!PopUInt16(ref m_user_id))
			{
				return false;
			}
			return true;
		}

		public override void ToTNetEventData(Packet packet, ref TNetEventData event_data, TNetObject target)
		{
			ParserPacket(packet);
			Debug.Log("master change id:" + m_user_id);
			if (target != null && target.CurRoom != null)
			{
				TNetUser userById = target.CurRoom.GetUserById(m_user_id);
				Debug.Log("111111111");
				if (userById != null)
				{
					Debug.Log("2222222222222s");
					target.CurRoom.RoomMasterID = m_user_id;
					target.CurRoom.RoomMaster = userById;
					event_data.data.Add("user", userById);
				}
			}
		}
	}
}
