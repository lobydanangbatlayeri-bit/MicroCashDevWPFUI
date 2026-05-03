using MicroCashDevWPFUI.DTOs;
using MicroCashDevWPFUI.Matching.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.Matching
{
	public class SupplierMatcher : ISupplierMatcher
	{
		public SupplierItem? FindClosest(
			IEnumerable<SupplierItem> suppliers,
			string? scannedName)
		{
			if (string.IsNullOrWhiteSpace(scannedName))
				return null;

			string cleanedScan = Normalize(scannedName);

			// 1️⃣ Exact match (case insensitive + cleaned)
			var exact = suppliers.FirstOrDefault(x =>
				Normalize(x.NamaSupplier)
				.Equals(cleanedScan, StringComparison.OrdinalIgnoreCase));

			if (exact != null)
				return exact;

			// 2️⃣ Contains match
			return suppliers.FirstOrDefault(x =>
				Normalize(x.NamaSupplier)
				.Contains(cleanedScan)
				|| cleanedScan.Contains(Normalize(x.NamaSupplier)));
		}

		private string Normalize(string input)
		{
			return new string(
				input.ToUpper()
					 .Where(char.IsLetterOrDigit)
					 .ToArray());
		}
	}
}
