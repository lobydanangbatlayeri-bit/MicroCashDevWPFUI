using MicroCashDevWPFUI.Api.Infrastructure;
using MicroCashDevWPFUI.DTOs;
using MicroCashDevWPFUI.OCR.Engine.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.Api.Endpoints
{
	public static class ScanNotaEndpoint
	{
		public static void MapScanNotaEndpoints(this WebApplication app)
		{
			app.MapPost("/api/scan-nota", async (
				HttpRequest request,
				IOcrService ocrService) =>
			{
				if (!request.HasFormContentType)
					return Results.BadRequest("Invalid form");

				var form = await request.ReadFormAsync();
				var file = form.Files["file"];

				if (file == null || file.Length == 0)
					return Results.BadRequest("File kosong");

				using var stream = file.OpenReadStream();
				var text = await ocrService.ReadTextAsync(stream);

				return Results.Ok(new
				{
					success = true,
					text = text
				});
			});

			app.MapPost("/api/scan-nota/submit", async (
	HttpContext context,
	ScanNotaMemoryStore store) =>
			{
				try
				{
					context.Request.EnableBuffering();

					using var reader = new StreamReader(
						context.Request.Body,
						Encoding.UTF8,
						leaveOpen: true);

					var rawBody = await reader.ReadToEndAsync();
					context.Request.Body.Position = 0;

					Console.WriteLine("===== RAW JSON MASUK =====");
					Console.WriteLine(rawBody);

					var dto = await context.Request.ReadFromJsonAsync<RestokNotaScanDto>();

					if (dto == null)
						return Results.BadRequest("DTO NULL");

					// 🔥🔥🔥 INI YANG KURANG
					store.Set(dto);

					Console.WriteLine("DATA DISIMPAN KE MEMORY");

					return Results.Ok(new { success = true });
				}
				catch (Exception ex)
				{
					Console.WriteLine("===== ERROR DESERIALIZE =====");
					Console.WriteLine(ex.ToString());

					return Results.BadRequest(ex.ToString());
				}
			});
		}
	}
}
