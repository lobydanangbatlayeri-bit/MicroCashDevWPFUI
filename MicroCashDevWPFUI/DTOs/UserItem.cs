using CommunityToolkit.Mvvm.ComponentModel;
using MicroCashDevWPFUI.DTOs.Interfaces;
using MicroCashDevWPFUI.Services.CoreService;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.DTOs
{
	public partial class UserItem : ObservableObject, IAutoSaveItem
	{
		private readonly IUserService _userService;
		private bool _isSaving;

		public UserItem(IUserService userService, int id)
		{
			_userService = userService;
			Id = id;
			Password = string.Empty;
		}

		public int Id { get; init; }

		[ObservableProperty]
		private string? username;

		[ObservableProperty]
		private string? password;

		partial void OnUsernameChanged(string? value)
		{
			_ = SaveAsync();
		}

		partial void OnPasswordChanged(string? value)
		{
			_ = SaveAsync();
		}

		public async Task SaveAsync()
		{
			if (_isSaving) return;
			_isSaving = true;

			try
			{
				var entity = await _userService.GetByIdAsync(Id);
				if (entity == null) return;

				Debug.WriteLine($"[DEBUG] Auto saving User Id={Id}");

				// Update username
				entity.Username = Username ?? entity.Username;
				await _userService.UpdateAsync(entity);

				// Update password jika diisi
				if (!string.IsNullOrWhiteSpace(Password))
				{
					await _userService.ChangePasswordAsync(Id, Password);

					// reset agar tidak tampil hash
					Password = string.Empty;
				}
			}
			finally
			{
				_isSaving = false;
			}
		}
	}
}