using CommunityToolkit.Mvvm.Messaging.Messages;

namespace MicroCashDevWPFUI.Messages
{
	public class LogoutMessage : ValueChangedMessage<bool>
	{
		public LogoutMessage() : base(true) { }
	}
}