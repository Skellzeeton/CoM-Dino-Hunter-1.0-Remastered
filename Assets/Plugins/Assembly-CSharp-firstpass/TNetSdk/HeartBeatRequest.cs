namespace TNetSdk
{
	public class HeartBeatRequest : TNetRequest
	{
		public HeartBeatRequest()
			: base(RequestType.HeartBeat)
		{
			Init();
		}

		private void Init()
		{
			packer = new SysHeartbeatCmd();
			packet = ((SysHeartbeatCmd)packer).MakePacket();
		}
	}
}
