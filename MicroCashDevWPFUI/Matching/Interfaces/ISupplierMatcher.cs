using MicroCashDevWPFUI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.Matching.Interfaces
{
	public interface ISupplierMatcher
	{
		SupplierItem? FindClosest(
			IEnumerable<SupplierItem> suppliers,
			string? scannedName);
	}
}
