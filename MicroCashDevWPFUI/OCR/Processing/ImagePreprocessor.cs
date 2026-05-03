using MicroCashDevWPFUI.OCR.Processing.Interfaces;
using OpenCvSharp;
using System.IO;
using Point = OpenCvSharp.Point;
using Size = OpenCvSharp.Size;

namespace MicroCashDevWPFUI.OCR.Processing
{
	public class ImagePreprocessor : IImagePreprocessor
	{
		public async Task<byte[]> ProcessAsync(Stream imageStream)
		{
			using var ms = new MemoryStream();
			await imageStream.CopyToAsync(ms);

			var src = Cv2.ImDecode(ms.ToArray(), ImreadModes.Color);

			// 🔥 Auto rotate jika landscape
			if (src.Width > src.Height)
			{
				Cv2.Rotate(src, src, RotateFlags.Rotate90Clockwise);
			}

			// Resize
			double ratio = 1200.0 / src.Height;
			Cv2.Resize(src, src, new Size(), ratio, ratio);

			// Grayscale
			using var gray = new Mat();
			Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);

			// Adaptive threshold
			using var binary = new Mat();
			Cv2.AdaptiveThreshold(
				gray,
				binary,
				255,
				AdaptiveThresholdTypes.GaussianC,
				ThresholdTypes.Binary,
				15,
				5
			);

			return binary.ToBytes(".png");
		}
	}
}