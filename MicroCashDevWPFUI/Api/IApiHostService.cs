using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.Api
{
	public interface IApiHostService
	{
		Task StartAsync();
		Task StopAsync();
		bool IsRunning { get; }
		string? CurrentIp { get; }
		int Port { get; }
	}
}
