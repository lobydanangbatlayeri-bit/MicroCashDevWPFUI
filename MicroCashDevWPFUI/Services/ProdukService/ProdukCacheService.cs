using MicroCashDevWPFUI.DTOs;
using MicroCashDevWPFUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MicroCashDevWPFUI.Services.ProdukService
{
    public class ProdukCacheService : IProdukCacheService
    {
        private readonly IProdukService _produkService;
        private readonly IProdukSatuanService _produkSatuanService;

        private Dictionary<string, ProductItem> _lookup = new();
        private Dictionary<int, List<ProdukSatuan>> _satuanCache = new();

        private bool _isLoaded = false;

        public ProdukCacheService(
            IProdukService produkService,
            IProdukSatuanService produkSatuanService)
        {
            _produkService = produkService;
            _produkSatuanService = produkSatuanService;
        }

        public async Task EnsureLoadedAsync()
        {
            if (_isLoaded) return;

            await LoadAsync();
            _isLoaded = true;
        }

        public async Task AddOrUpdateAsync(Produk produk)
        {
            // hapus key lama kalau ada nama berubah
            var existingKey = _lookup
                .FirstOrDefault(x => x.Value.Id == produk.Id).Key;

            if (existingKey != null && existingKey != produk.NamaBarang.ToUpper())
                _lookup.Remove(existingKey);

            _lookup[produk.NamaBarang.ToUpper()] = new ProductItem
            {
                Id = produk.Id,
                NamaBarang = produk.NamaBarang
            };

            _satuanCache.Remove(produk.Id);

            OnCacheUpdated?.Invoke();
        }

        private async Task LoadAsync()
        {
            var data = await _produkService.GetAllAsync();

            _lookup = data.ToDictionary(
                x => x.NamaBarang.ToUpper(),
                x => new ProductItem
                {
                    Id = x.Id,
                    NamaBarang = x.NamaBarang
                });
        }

        public ProductItem? Find(string nama)
        {
            if (string.IsNullOrWhiteSpace(nama))
                return null;

            _lookup.TryGetValue(nama.ToUpper(), out var result);
            return result;
        }

        public IEnumerable<ProductItem> GetAll()
        {
            return _lookup.Values;
        }

        public async Task<List<ProdukSatuan>> GetSatuanAsync(int produkId)
        {
            // ✅ ambil dari cache dulu
            if (_satuanCache.TryGetValue(produkId, out var satuan))
                return satuan;

            // ❗ belum ada → ambil dari service
            var data = await _produkSatuanService.GetByProdukIdAsync(produkId);

            // simpan ke cache
            _satuanCache[produkId] = data;

            return data;
        }

        public event Action? OnCacheUpdated;
    }
}
