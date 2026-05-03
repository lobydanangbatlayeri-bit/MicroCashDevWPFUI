using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.Models
{
	public class Menu
	{
		public int Id { get; set; }
		public int? ParentId { get; set; }
		public string NamaMenu { get; set; } = "";
		public string PageKey { get; set; } = "";
		public string Icon { get; set; } = "";
		public int Urutan { get; set; }

		public Menu? Parent { get; set; }
		public ICollection<Menu> Children { get; set; } = new List<Menu>();
		public ICollection<UserMenu> UserMenus { get; set; } = new List<UserMenu>();
	}

}
