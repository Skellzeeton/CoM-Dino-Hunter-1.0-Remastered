using System;
using System.Collections.Generic;
using System.Timers;
using Sfs2X.Requests;

namespace Sfs2X.Util
{
	public class LagMonitor
	{
		private int lastReqTime;

		private List<int> valueQueue;

		private int interval;

		private int queueSize;

		private Timer pollTimer;

		private SmartFox sfs;

		public bool IsRunning
		{
			get
			{
				return pollTimer.Enabled;
			}
		}

		public int LastPingTime
		{
			get
			{
				if (valueQueue.Count > 0)
				{
					return valueQueue[valueQueue.Count - 1];
				}
				return 0;
			}
		}

		public int AveragePingTime
		{
			get
			{
				if (valueQueue.Count == 0)
				{
					return 0;
				}
				int num = 0;
				foreach (int item in valueQueue)
				{
					num += item;
				}
				return num / valueQueue.Count;
			}
		}

		public LagMonitor(SmartFox sfs)
			: this(sfs, 4, 10)
		{
		}

		public LagMonitor(SmartFox sfs, int interval)
			: this(sfs, interval, 10)
		{
		}

		public LagMonitor(SmartFox sfs, int interval, int queueSize)
		{
			if (interval < 1)
			{
				interval = 1;
			}
			this.sfs = sfs;
			valueQueue = new List<int>();
			this.interval = interval;
			this.queueSize = queueSize;
			pollTimer = new Timer();
			pollTimer.Enabled = false;
			pollTimer.AutoReset = true;
			pollTimer.Elapsed += OnPollEvent;
			pollTimer.Interval = interval * 1000;
		}

		public void Start()
		{
			if (!IsRunning)
			{
				pollTimer.Start();
			}
		}

		public void Stop()
		{
			if (IsRunning)
			{
				pollTimer.Stop();
			}
		}

		public void Destroy()
		{
			Stop();
			pollTimer.Dispose();
			sfs = null;
		}

		public int OnPingPong()
		{
			int item = DateTime.Now.Millisecond - lastReqTime;
			if (valueQueue.Count >= queueSize)
			{
				valueQueue.RemoveAt(0);
			}
			valueQueue.Add(item);
			return AveragePingTime;
		}

		private void OnPollEvent(object source, ElapsedEventArgs e)
		{
			Console.WriteLine("********** Polling!!");
			lastReqTime = DateTime.Now.Millisecond;
			sfs.Send(new PingPongRequest());
		}
	}
}
