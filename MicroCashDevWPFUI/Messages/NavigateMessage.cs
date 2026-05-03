using CommunityToolkit.Mvvm.Messaging.Messages;
using System.Windows.Controls;

namespace MicroCashDevWPFUI.Messages
{
	public class NavigateMessage : ValueChangedMessage<Page>
	{
		public NavigateMessage(Page value) : base(value) { }
	}
}
