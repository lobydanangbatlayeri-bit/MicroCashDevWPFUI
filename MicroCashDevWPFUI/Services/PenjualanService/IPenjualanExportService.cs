using MicroCashDevWPFUI.DTOs;

namespace MicroCashDevWPFUI.Services.PenjualanService
{
	public interface IPenjualanExportService
	{
		Task ExportAsync(
			IEnumerable<PenjualanItem> data,
			DateTime start,
			DateTime end);
	}
}