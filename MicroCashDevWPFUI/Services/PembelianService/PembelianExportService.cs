using ClosedXML.Excel;
using MicroCashDevWPFUI.DTOs;
using System.Diagnostics;
using System.IO;

namespace MicroCashDevWPFUI.Services.PembelianService
{
	public class PembelianExportService : IPembelianExportService
	{
		public Task ExportAsync(
	IEnumerable<PembelianItem> data,
	DateTime start,
	DateTime end)
		{
			if (!data.Any())
				return Task.CompletedTask;

			using var wb = new XLWorkbook();
			var ws = wb.Worksheets.Add("Pembelian");

			int row = 1;

			var lastDay = end.AddDays(-1).Date;

			string periodeText =
				start.Date == lastDay
					? start.ToString("dd MMMM yyyy")
					: $"{start:dd MMM yyyy} - {lastDay:dd MMM yyyy}";

			// ================= JUDUL =================
			ws.Cell(row, 1).Value = $"RIWAYAT PEMBELIAN : {periodeText}";
			ws.Range(row, 1, row, 10).Merge();
			ws.Range(row, 1, row, 10).Style.Font.Bold = true;
			ws.Range(row, 1, row, 10).Style.Font.FontSize = 14;
			ws.Range(row, 1, row, 10).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

			row += 2;

			// ================= HEADER =================
			var headers = new[]
			{
		"Tanggal",
		"Nomor Faktur",
		"Supplier",
		"Barang",
		"Satuan",
		"Jumlah",
		"Harga Beli",
		"Subtotal",
		"Batch",
		"Expired"
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
			foreach (var p in data)
			{
				foreach (var d in p.DetailPembelians)
				{
					foreach (var b in d.ProdukBatchs)
					{
						ws.Cell(row, 1).Value = p.Tanggal;
						ws.Cell(row, 2).Value = p.NomorFaktur;
						ws.Cell(row, 3).Value = p.Supplier?.NamaSupplier ?? "";
						ws.Cell(row, 4).Value = d.Produk;
						ws.Cell(row, 5).Value = d.Satuan?.NamaSatuan ?? "";
						ws.Cell(row, 6).Value = d.Jumlah;
						ws.Cell(row, 7).Value = d.HargaBeli;
						ws.Cell(row, 8).Value = d.Subtotal;
						ws.Cell(row, 9).Value = b.BatchNumber;
						ws.Cell(row, 10).Value = b.TanggalKadarluasa;

						grandTotal += d.Subtotal;

						row++;
					}
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
			ws.Cell(row + 1, 7).Value = "TOTAL KESELURUHAN";
			ws.Cell(row + 1, 8).Value = grandTotal;

			ws.Range(row + 1, 7, row + 1, 8).Style.Font.Bold = true;
			ws.Range(row + 1, 7, row + 1, 8).Style.Fill.BackgroundColor = XLColor.LightYellow;

			// ================= FORMAT =================
			ws.Column(1).Style.DateFormat.Format = "dd/MM/yyyy";
			ws.Column(10).Style.DateFormat.Format = "dd/MM/yyyy";
			ws.Column(6).Style.NumberFormat.Format = "#,##0";
			ws.Column(7).Style.NumberFormat.Format = "#,##0";
			ws.Column(8).Style.NumberFormat.Format = "#,##0";

			ws.Columns().AdjustToContents();

			// ================= FILE NAME =================
			var fileName =
				start.Date == lastDay
					? $"Pembelian_{start:yyyyMMdd}.xlsx"
					: $"Pembelian_{start:yyyyMMdd}_sd_{lastDay:yyyyMMdd}.xlsx";

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
