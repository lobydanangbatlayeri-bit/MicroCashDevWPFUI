using MicroCashDevWPFUI.OCR.Engine.Interfaces;
using System.IO;
using Tesseract;
using TRect = Tesseract.Rect;

public class TesseractOcrService : IOcrService
{
	private readonly string _tessPath;

	public TesseractOcrService()
	{
		_tessPath = Path.Combine(
			AppContext.BaseDirectory,
			"OCR",
			"tessdata");
	}

	public async Task<string> ReadTextAsync(Stream imageStream)
	{
		var imageBytes = await ReadStreamAsync(imageStream);

		using var engine = CreateEngine();
		using var image = Pix.LoadFromMemory(imageBytes);

		using var page = engine.Process(image);

		return page.GetText();
	}
	private TesseractEngine CreateEngine()
	{
		var engine = new TesseractEngine(
			_tessPath,
			"ind+eng",
			EngineMode.LstmOnly);

		engine.DefaultPageSegMode = PageSegMode.SparseText;

		return engine;
	}

	private async Task<byte[]> ReadStreamAsync(Stream stream)
	{
		using var ms = new MemoryStream();
		await stream.CopyToAsync(ms);
		return ms.ToArray();
	}
}