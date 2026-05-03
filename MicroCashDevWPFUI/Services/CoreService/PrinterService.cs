using MicroCashDevWPFUI.DTOs;
using MicroCashDevWPFUI.Models;
using System.Drawing;
using System.Drawing.Printing;

namespace MicroCashDevWPFUI.Services.CoreService
{
	public class PrinterService : IPrinterService
	{
		private const string PrinterName = "POS-80";
		private const float PaperWidth = 280f;

		private readonly IProfileTokoService _profileTokoService;

		public PrinterService(IProfileTokoService profileTokoService)
		{
			_profileTokoService = profileTokoService;
		}

		public async Task PrintStrukAsync(StrukDto struk)
		{
			try
			{
				var profile = await _profileTokoService.GetAsync();

				if (!PrinterSettings.InstalledPrinters
						.Cast<string>()
						.Any(p => p.Equals(PrinterName, StringComparison.OrdinalIgnoreCase)))
					return;

				PrintDocument pd = new PrintDocument();
				pd.PrinterSettings.PrinterName = PrinterName;

				if (!pd.PrinterSettings.IsValid)
					return;

				pd.PrintPage += (sender, e) =>
				{
					float y = 10;
					float left = 0;

					using Font font = new Font("Consolas", 9);
					using Font fontBold = new Font("Consolas", 11, System.Drawing.FontStyle.Bold);

					var leftAlign = new StringFormat { Alignment = StringAlignment.Near };
					var rightAlign = new StringFormat { Alignment = StringAlignment.Far };

					// ================= HEADER =================
					if (profile != null)
					{
						y = DrawCentered(e, profile.NamaToko, fontBold, y);
						y = DrawCentered(e, profile.Alamat, font, y);

						if (!string.IsNullOrWhiteSpace(profile.KataSambutan))
							y = DrawCentered(e, profile.KataSambutan, font, y);
					}

					y += 5;
					y = DrawLine(e, font, y);

					// ================= INFO =================
					y = DrawLeft(e, $"Nota : {struk.NomorNota}", font, y);
					y = DrawLeft(e, $"Tanggal : {struk.Tanggal:dd/MM/yyyy HH:mm}", font, y);

					y = DrawLine(e, font, y);

					// ================= DETAIL =================
					foreach (var item in struk.Items)
					{
						var wrappedLines = WrapText(e.Graphics!, item.NamaBarang, font, PaperWidth - 10);

						foreach (var line in wrappedLines)
						{
							e.Graphics!.DrawString(line, font, Brushes.Black,
								new RectangleF(left, y, PaperWidth, 18), leftAlign);
							y += 16;
						}

						e.Graphics!.DrawString($"{item.Jumlah} x {item.Harga:N0}",
							font, Brushes.Black,
							new RectangleF(left, y, 150, 18), leftAlign);

						e.Graphics.DrawString($"{item.Subtotal:N0}",
							font, Brushes.Black,
							new RectangleF(left, y, PaperWidth - 5, 18), rightAlign);

						y += 20;
					}

					y = DrawLine(e, font, y);

					// ================= TOTAL =================
					y = DrawTotalRow(e, "Total", struk.Total, fontBold, y);
					y = DrawTotalRow(e, "Bayar", struk.Dibayar, font, y);
					y = DrawTotalRow(e, "Kembali", struk.Kembalian, font, y);

					y += 15;
					y = DrawCentered(e, "Terima Kasih", font, y);
				};

				pd.Print();
			}
			catch
			{
				// silent fail sesuai keinginan kamu
			}
		}

		// ================= HELPER METHODS =================

		private float DrawLine(PrintPageEventArgs e, Font font, float y)
		{
			e.Graphics!.DrawString(new string('-', 32), font, Brushes.Black, 0, y);
			return y + 18;
		}

		private float DrawCentered(PrintPageEventArgs e, string? text, Font font, float y)
		{
			if (string.IsNullOrWhiteSpace(text))
				return y;

			e.Graphics!.DrawString(text, font, Brushes.Black,
				new RectangleF(0, y, PaperWidth, 20),
				new StringFormat { Alignment = StringAlignment.Center });

			return y + 20;
		}

		private float DrawLeft(PrintPageEventArgs e, string text, Font font, float y)
		{
			e.Graphics!.DrawString(text, font, Brushes.Black, 0, y);
			return y + 18;
		}

		private float DrawTotalRow(PrintPageEventArgs e, string label, decimal value, Font font, float y)
		{
			var leftAlign = new StringFormat { Alignment = StringAlignment.Near };
			var rightAlign = new StringFormat { Alignment = StringAlignment.Far };

			e.Graphics!.DrawString(label,
				font, Brushes.Black,
				new RectangleF(0, y, 150, 18), leftAlign);

			e.Graphics.DrawString($"{value:N0}",
				font, Brushes.Black,
				new RectangleF(0, y, PaperWidth - 5, 18), rightAlign);

			return y + 20;
		}

		private List<string> WrapText(Graphics g, string text, Font font, float maxWidth)
		{
			var words = text.Split(' ');
			var lines = new List<string>();
			string currentLine = "";

			foreach (var word in words)
			{
				var testLine = string.IsNullOrEmpty(currentLine)
					? word
					: currentLine + " " + word;

				var size = g.MeasureString(testLine, font);

				if (size.Width > maxWidth)
				{
					if (!string.IsNullOrEmpty(currentLine))
						lines.Add(currentLine);

					currentLine = word;
				}
				else
				{
					currentLine = testLine;
				}
			}

			if (!string.IsNullOrEmpty(currentLine))
				lines.Add(currentLine);

			return lines;
		}
	}
}