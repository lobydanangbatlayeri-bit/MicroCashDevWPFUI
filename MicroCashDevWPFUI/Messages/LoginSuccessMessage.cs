using CommunityToolkit.Mvvm.Messaging.Messages;
using MicroCashDevWPFUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.Messages
{
	public class LoginSuccessMessage : ValueChangedMessage<User>
	{
		public LoginSuccessMessage(User user) : base(user) { }
	}
}
