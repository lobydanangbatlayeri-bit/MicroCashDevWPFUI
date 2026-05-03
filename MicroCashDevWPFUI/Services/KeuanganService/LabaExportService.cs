using ClosedXML.Excel;
using MicroCashDevWPFUI.DTOs;
using System.Diagnostics;
using System.IO;

namespace MicroCashDevWPFUI.Services.KeuanganService
{
	public class LabaExportService : ILabaExportService
	{
		public Task ExportAsync(
			IEnumerable<LabaItem> data,
			DateTime start,
			DateTime end)
		{
			if (!data.Any())
				return Task.CompletedTask;

			using var wb = new XLWorkbook();
			var ws = wb.Worksheets.Add("Laba");

			int row = 1;

			// ================= JUDUL =================
			var lastDay = end.AddDays(-1).Date;
			string periodeText =
				start.Date == lastDay
					? start.ToString("dd MMMM yyyy")
					: $"{start:dd MMM yyyy} - {lastDay:dd MMM yyyy}";

			ws.Cell(row, 1).Value = $"LAPORAN LABA : {periodeText}";
			ws.Range(row, 1, row, 8).Merge();
			ws.Range(row, 1, row, 8).Style.Font.Bold = true;
			ws.Range(row, 1, row, 8).Style.Font.FontSize = 14;
			ws.Range(row, 1, row, 8).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

			row += 2;

			// ================= HEADER =================
			var headers = new[]
			{
				"Tanggal",
				"Nomor Nota",
				"Produk",
				"Qty",
				"Harga Jual",
				"Subtotal",
				"HPP",
				"Laba"
			};

			for (int col = 0; col < headers.Length; col++)
			{
				ws.Cell(row, col + 1).Value = headers[col];
			}

			ws.Range(row, 1, row, headers.Length).Style.Font.Bold = true;
			ws.Range(row, 1, row, headers.Length).Style.Fill.BackgroundColor = XLColor.LightGray;

			row++;

			decimal totalPenjualan = 0;
			decimal totalHpp = 0;
			decimal totalLaba = 0;
			int dataStartRow = row;

			// ================= DATA =================
			foreach (var p in data)
			{
				totalPenjualan += p.TotalPenjualan;
				totalHpp += p.TotalHpp;
				totalLaba += p.Laba;

				foreach (var d in p.DetailLabas)
				{
					ws.Cell(row, 1).Value = p.Tanggal;
					ws.Cell(row, 2).Value = p.NomorNota;
					ws.Cell(row, 3).Value = d.Produk;
					ws.Cell(row, 4).Value = d.Jumlah;
					ws.Cell(row, 5).Value = d.HargaJual;
					ws.Cell(row, 6).Value = d.Subtotal;
					ws.Cell(row, 7).Value = d.Hpp;
					ws.Cell(row, 8).Value = d.Laba;

					row++;
				}
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
			ws.Cell(row + 1, 6).Value = "TOTAL KESELURUHAN";
			ws.Cell(row + 1, 7).Value = totalHpp;
			ws.Cell(row + 1, 8).Value = totalLaba;

			ws.Range(row + 1, 6, row + 1, 8).Style.Font.Bold = true;
			ws.Range(row + 1, 6, row + 1, 8).Style.Fill.BackgroundColor = XLColor.LightYellow;

			// ================= FORMAT =================
			ws.Column(1).Style.DateFormat.Format = "dd/MM/yyyy";
			ws.Column(4).Style.NumberFormat.Format = "#,##0";
			ws.Column(5).Style.NumberFormat.Format = "#,##0";
			ws.Column(6).Style.NumberFormat.Format = "#,##0";
			ws.Column(7).Style.NumberFormat.Format = "#,##0";
			ws.Column(8).Style.NumberFormat.Format = "#,##0";

			ws.Columns().AdjustToContents();

			// ================= FILE NAME =================
			var fileName =
				start.Date == lastDay
					? $"Laba_{start:yyyyMMdd}.xlsx"
					: $"Laba_{start:yyyyMMdd}_sd_{lastDay:yyyyMMdd}.xlsx";

			var path = Path.Combine(
				Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
				fileName);

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
