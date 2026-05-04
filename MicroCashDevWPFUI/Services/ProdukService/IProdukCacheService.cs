using MicroCashDevWPFUI.DTOs;
using MicroCashDevWPFUI.Models;

namespace MicroCashDevWPFUI.Services.ProdukService
{
    public interface IProdukCacheService
    {
        Task EnsureLoadedAsync();
        ProductItem? Find(string nama);
        Task AddOrUpdateAsync(Produk produk);
        IEnumerable<ProductItem> GetAll();
        Task<List<ProdukSatuan>> GetSatuanAsync(int produkId);
        event Action? OnCacheUpdated;
    }
}
