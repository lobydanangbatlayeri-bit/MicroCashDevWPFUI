using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.Models
{
	public class UserMenu
	{
		public int Id { get; set; }

		public int UserId { get; set; }
		public User User { get; set; } = null!;

		public int MenuId { get; set; }
		public Menu Menu { get; set; } = null!;
	}

}
