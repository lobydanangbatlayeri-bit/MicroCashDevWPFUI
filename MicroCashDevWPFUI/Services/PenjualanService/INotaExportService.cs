using MicroCashDevWPFUI.DTOs;

namespace MicroCashDevWPFUI.Services.PenjualanService
{
	public interface INotaExportService
	{
		Task ExportAsync(IEnumerable<NotaItem> data, DateTime start, DateTime end);
	}
}
