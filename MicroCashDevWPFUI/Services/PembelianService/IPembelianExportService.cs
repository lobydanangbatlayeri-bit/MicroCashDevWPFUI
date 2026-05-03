using MicroCashDevWPFUI.DTOs;
using System.Collections.ObjectModel;

namespace MicroCashDevWPFUI.Services.PembelianService
{
	public interface IPembelianExportService
	{
		Task ExportAsync(IEnumerable<PembelianItem> data, DateTime start, DateTime end);
	}

}
