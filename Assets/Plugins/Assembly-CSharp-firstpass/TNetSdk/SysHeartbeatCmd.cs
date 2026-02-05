namespace TNetSdk
{
	public class SysHeartbeatCmd : SysCmd
	{
		public Packet MakePacket()
		{
			return MakePacket(CMD.sys_heartbeat);
		}
	}
}
