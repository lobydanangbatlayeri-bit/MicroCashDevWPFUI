using MicroCashDevWPFUI.DTOs;
using MicroCashDevWPFUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.Services.CoreService
{
	public interface IPrinterService
	{
		Task PrintStrukAsync(StrukDto struk);
	}
}
