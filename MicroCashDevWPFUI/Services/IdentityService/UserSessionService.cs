using MicroCashDevWPFUI.Models;
using MicroCashDevWPFUI.Services.CoreService;

namespace MicroCashDevWPFUI.Services.IdentityService
{
	public class UserSessionService : IUserSessionService
	{
		public User? CurrentUser { get; set; }

		public bool IsLoggedIn => CurrentUser != null;

		public void Logout()
		{
			CurrentUser = null;
		}
	}
}
