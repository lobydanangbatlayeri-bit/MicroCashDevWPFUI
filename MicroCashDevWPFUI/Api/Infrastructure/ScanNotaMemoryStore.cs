using MicroCashDevWPFUI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.Api.Infrastructure
{
	public class ScanNotaMemoryStore
	{
		private RestokNotaScanDto? _current;

		public void Set(RestokNotaScanDto data)
		{
			_current = data;
		}

		public RestokNotaScanDto? Get()
		{
			return _current;
		}

		public void Clear()
		{
			_current = null;
		}
	}
}
