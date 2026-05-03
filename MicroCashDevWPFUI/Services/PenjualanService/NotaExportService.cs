using ClosedXML.Excel;
using MicroCashDevWPFUI.DTOs;
using System.Diagnostics;
using System.IO;

namespace MicroCashDevWPFUI.Services.PenjualanService
{
	public class NotaExportService : INotaExportService
	{
		public Task ExportAsync(IEnumerable<NotaItem> data, DateTime start, DateTime end)
		{
			if (!data.Any())
				return Task.CompletedTask;

			using var wb = new XLWorkbook();
			var ws = wb.Worksheets.Add("Nota");

			int row = 1;

			var lastDay = end.AddDays(-1).Date;

			string periodeText =
				start == DateTime.MinValue && end.Date >= DateTime.Now.Date
					? "Semua Waktu"
					: start.Date == lastDay
						? start.ToString("dd MMMM yyyy")
						: $"{start:dd MMM yyyy} - {lastDay:dd MMM yyyy}";

			// ================= JUDUL =================
			ws.Cell(row, 1).Value = $"RIWAYAT NOTA : {periodeText}";
			ws.Range(row, 1, row, 6).Merge();
			ws.Range(row, 1, row, 6).Style.Font.Bold = true;
			ws.Range(row, 1, row, 6).Style.Font.FontSize = 14;
			ws.Range(row, 1, row, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

			row += 2;

			// ================= HEADER =================
			var headers = new[]
			{
				"Tanggal Deadline",
				"Nomor Faktur",
				"Supplier",
				"Item (Bank)",
				"Total",
				"Status Pembayaran"
			};

			for (int col = 0; col < headers.Length; col++)
			{
				ws.Cell(row, col + 1).Value = headers[col];
			}

			ws.Range(row, 1, row, headers.Length).Style.Font.Bold = true;
			ws.Range(row, 1, row, headers.Length).Style.Fill.BackgroundColor = XLColor.LightGray;

			row++;

			decimal grandTotal = 0;
			int dataStartRow = row;

			// ================= DATA =================
			foreach (var n in data)
			{
				ws.Cell(row, 1).Value = n.TanggalDeadline;
				ws.Cell(row, 2).Value = n.NomorFaktur;
				ws.Cell(row, 3).Value = n.Supplier?.NamaSupplier ?? "";
				ws.Cell(row, 4).Value = n.Item;
				ws.Cell(row, 5).Value = n.Total;
				ws.Cell(row, 6).Value = n.StatusPembayaran ? "Lunas" : "Belum Lunas";

				grandTotal += n.Total;
				row++;
			}

			int dataEndRow = row - 1;

			// ================= BORDER =================
			if (dataEndRow >= dataStartRow)
			{
				ws.Range(dataStartRow - 1, 1, dataEndRow, headers.Length)
				  .Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

				ws.Range(dataStartRow - 1, 1, dataEndRow, headers.Length)
				  .Style.Border.InsideBorder = XLBorderStyleValues.Thin;
			}

			// ================= GRAND TOTAL =================
			ws.Cell(row + 1, 4).Value = "TOTAL KESELURUHAN";
			ws.Cell(row + 1, 5).Value = grandTotal;

			ws.Range(row + 1, 4, row + 1, 5).Style.Font.Bold = true;
			ws.Range(row + 1, 4, row + 1, 5).Style.Fill.BackgroundColor = XLColor.LightYellow;

			// ================= FORMAT =================
			ws.Column(1).Style.DateFormat.Format = "dd/MM/yyyy";
			ws.Column(4).Style.NumberFormat.Format = "#,##0";
			ws.Column(5).Style.NumberFormat.Format = "#,##0";

			ws.Columns().AdjustToContents();

			// ================= FILE NAME =================
			var fileName =
				start.Date == lastDay
					? $"Nota_{start:yyyyMMdd}.xlsx"
					: $"Nota_{start:yyyyMMdd}_sd_{lastDay:yyyyMMdd}.xlsx";

			var path = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
				fileName
			);

			wb.SaveAs(path);

			Process.Start(new ProcessStartInfo
			{
				FileName = path,
				UseShellExecute = true
			});

			return Task.CompletedTask;
		}
	}
}
