using MicroCashDevWPFUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.Services.CoreService
{
	public interface IProfileTokoService
	{
		Task<ProfileToko?> GetAsync();
		Task<ProfileToko> SaveAsync(ProfileToko profile);
	}
}
