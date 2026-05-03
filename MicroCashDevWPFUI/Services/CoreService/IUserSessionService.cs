using MicroCashDevWPFUI.Models;

namespace MicroCashDevWPFUI.Services.CoreService
{
	public interface IUserSessionService
	{
		User? CurrentUser { get; set; }
		bool IsLoggedIn => CurrentUser != null;
		void Logout();
	}
}
