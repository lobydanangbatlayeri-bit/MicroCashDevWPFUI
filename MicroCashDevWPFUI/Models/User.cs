using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.Models
{
	public class User
	{
		public int Id { get; set; }
		public string Username { get; set; } = "";
		public string Password { get; set; } = "";

		public ICollection<UserMenu> UserMenus { get; set; } = new List<UserMenu>();
		public ICollection<Penjualan> Penjualans { get; set; } = new List<Penjualan>();
	}

}
