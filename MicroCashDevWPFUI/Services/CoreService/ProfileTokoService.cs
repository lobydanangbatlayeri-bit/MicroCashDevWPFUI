using MicroCashDevWPFUI.Data;
using MicroCashDevWPFUI.Models;
using Microsoft.EntityFrameworkCore;

namespace MicroCashDevWPFUI.Services.CoreService
{
	public class ProfileTokoService : IProfileTokoService
	{
		private readonly IAppDbContext _context;

		public ProfileTokoService(IAppDbContext context)
		{
			_context = (AppDbContext)context;
		}

		public async Task<ProfileToko?> GetAsync()
		{
			return await _context.Set<ProfileToko>()
				.AsNoTracking()
				.FirstOrDefaultAsync();
		}

		public async Task<ProfileToko> SaveAsync(ProfileToko profile)
		{
			var existing = await _context.Set<ProfileToko>()
				.FirstOrDefaultAsync();

			if (existing == null)
			{
				_context.Set<ProfileToko>().Add(profile);
			}
			else
			{
				existing.NamaToko = profile.NamaToko;
				existing.Alamat = profile.Alamat;
				existing.KataSambutan = profile.KataSambutan;
			}

			await _context.SaveChangesAsync();

			return existing ?? profile;
		}
	}
}
