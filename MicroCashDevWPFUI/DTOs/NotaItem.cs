using MicroCashDevWPFUI.DTOs.Interfaces;
using MicroCashDevWPFUI.Models;
using MicroCashDevWPFUI.Services.PenjualanService;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace MicroCashDevWPFUI.DTOs
{
	public partial class NotaItem : ObservableObject, IAutoSaveItem
	{
		private readonly INotaService? _notaService;

		public NotaItem() { }

		public NotaItem(INotaService notaService, int notaId)
		{
			_notaService = notaService;
			Id = notaId;
			Debug.WriteLine($"[DEBUG] NotaItem created: Id={Id}");
		}

		public int Id { get; init; }

		[ObservableProperty] private int nomor;
		[ObservableProperty] private string nomorFaktur = "";
		[ObservableProperty] private DateTime tanggalDeadline;
		[ObservableProperty] private Supplier? supplier;
		[ObservableProperty] private int item;
		[ObservableProperty] private decimal total;
		[ObservableProperty] private bool statusPembayaran;

		public ObservableCollection<BankItem> Bank { get; set; } = new();

		partial void OnStatusPembayaranChanged(bool oldValue, bool newValue)
		{
			_ = Task.Run(async () =>
			{
				try { await SaveAsync(); }
				catch (Exception ex) { Debug.WriteLine($"Error saving NotaItem: {ex.Message}"); }
			});
		}

		public async Task SaveAsync()
		{
			if (_notaService == null)
			{
				Debug.WriteLine("[DEBUG] NotaItem.SaveAsync skipped: _notaService is null");
				return;
			}

			var entity = await _notaService.GetByIdAsync(Id);
			if (entity != null)
			{
				Debug.WriteLine($"[DEBUG] Saving NotaItem Id={Id}: Total={Total}, Supplier={Supplier?.NamaSupplier}, NomorFaktur={NomorFaktur}, StatusPembayaran={StatusPembayaran}");

				entity.Pembelian.NomorFaktur = NomorFaktur;
				entity.TanggalDeadline = TanggalDeadline;
				if (Supplier != null)
					entity.Pembelian.Supplier = Supplier;
				entity.Total = Total;
				entity.StatusPembayaran = StatusPembayaran;

				await _notaService.UpdateAsync(entity);
			}
			else
			{
				Debug.WriteLine($"[DEBUG] NotaItem.SaveAsync: entity Id={Id} not found!");
			}
		}
	}
}
