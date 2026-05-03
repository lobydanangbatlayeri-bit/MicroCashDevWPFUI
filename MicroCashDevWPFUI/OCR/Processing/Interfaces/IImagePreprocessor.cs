using System.IO;

namespace MicroCashDevWPFUI.OCR.Processing.Interfaces
{
	public interface IImagePreprocessor
	{
		Task<byte[]> ProcessAsync(Stream imageStream);
	}
}
