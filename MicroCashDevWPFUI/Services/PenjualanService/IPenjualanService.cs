using MicroCashDevWPFUI.DTOs;
using MicroCashDevWPFUI.Models;

namespace MicroCashDevWPFUI.Services.PenjualanService
{
	public interface IPenjualanService
	{
		Task<Penjualan> ProsesPenjualanAsync(
			int userId,
			decimal dibayar,
			IEnumerable<KeranjangItems> keranjang);
		Task<Penjualan?> GetByIdAsync(int id);
		Task UpdateAsync(Penjualan penjualan);
		Task<List<Penjualan>> GetRiwayatAsync(DateTime? start = null, DateTime? end = null);
		Task<StrukDto?> GetStrukAsync(int penjualanId);
	}
}
