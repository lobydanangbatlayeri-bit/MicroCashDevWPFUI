using MicroCashDevWPFUI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.Services.KeuanganService
{
	public interface ILabaExportService
	{
		Task ExportAsync(IEnumerable<LabaItem> data, DateTime start, DateTime end);
	}
}
