using MicroCashDevWPFUI.DTOs;
using MicroCashDevWPFUI.Matching.Interfaces;

namespace MicroCashDevWPFUI.Matching
{
	public class ProductMatcher : IProductMatcher
	{
		public ProductItem? FindClosest(
			IEnumerable<ProductItem> products,
			string? scannedName)
		{
			if (string.IsNullOrWhiteSpace(scannedName))
				return null;

			string cleanedScan = Normalize(scannedName);

			// 1️⃣ Exact Match
			var exact = products.FirstOrDefault(x =>
				Normalize(x.NamaBarang)
				.Equals(cleanedScan, StringComparison.OrdinalIgnoreCase));

			if (exact != null)
				return exact;

			// 2️⃣ Contains Match
			var contains = products.FirstOrDefault(x =>
				Normalize(x.NamaBarang).Contains(cleanedScan)
				|| cleanedScan.Contains(Normalize(x.NamaBarang)));

			if (contains != null)
				return contains;

			// 3️⃣ Fuzzy Match (Levenshtein Score)
			return products
				.Select(p => new
				{
					Product = p,
					Score = Similarity(Normalize(p.NamaBarang), cleanedScan)
				})
				.OrderByDescending(x => x.Score)
				.Where(x => x.Score > 0.6) // threshold 60%
				.Select(x => x.Product)
				.FirstOrDefault();
		}

		private string Normalize(string input)
		{
			return new string(
				input.ToUpper()
					 .Where(char.IsLetterOrDigit)
					 .ToArray());
		}

		private double Similarity(string source, string target)
		{
			int distance = LevenshteinDistance(source, target);
			int maxLength = Math.Max(source.Length, target.Length);

			if (maxLength == 0)
				return 1.0;

			return 1.0 - (double)distance / maxLength;
		}

		private int LevenshteinDistance(string s, string t)
		{
			int[,] d = new int[s.Length + 1, t.Length + 1];

			for (int i = 0; i <= s.Length; i++)
				d[i, 0] = i;

			for (int j = 0; j <= t.Length; j++)
				d[0, j] = j;

			for (int i = 1; i <= s.Length; i++)
			{
				for (int j = 1; j <= t.Length; j++)
				{
					int cost = (s[i - 1] == t[j - 1]) ? 0 : 1;

					d[i, j] = Math.Min(
						Math.Min(d[i - 1, j] + 1,
								 d[i, j - 1] + 1),
						d[i - 1, j - 1] + cost);
				}
			}

			return d[s.Length, t.Length];
		}
	}
}
