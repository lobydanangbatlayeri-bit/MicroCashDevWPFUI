using System.IO;

namespace MicroCashDevWPFUI.OCR.Engine.Interfaces
{
	public interface IOcrService
	{
		Task<string> ReadTextAsync(Stream imageStream);
	}
}
