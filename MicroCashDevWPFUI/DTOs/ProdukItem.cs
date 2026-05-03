using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using MicroCashDevWPFUI.Models;

namespace MicroCashDevWPFUI.DTOs
{
	public partial class ProdukItem : ObservableObject
	{
		#region Fields

		private bool _isInternalUpdate;

		private string _originalNamaBarang = string.Empty;
		private string _originalSatuan = string.Empty;
		private decimal _originalHargaJual;

		#endregion

		#region Identity

		[ObservableProperty]
		private int produkId;

		#endregion

		#region Editable Properties

		[ObservableProperty]
		private string namaBarang = string.Empty;

		[ObservableProperty]
		private string satuan = string.Empty;

		[ObservableProperty]
		private decimal hargaJual;

		#endregion

		#region Stock (READ ONLY)

		[ObservableProperty]
		private int currentStok;

		#endregion

		#region Collections

		[ObservableProperty]
		private ObservableCollection<string> satuanList = new();

		[ObservableProperty]
		private ObservableCollection<ProdukSatuan> produkSatuans = new();

		public string Varian => SatuanList?.Count.ToString() ?? "0";

		#endregion

		#region Change Hooks

		partial void OnSatuanChanged(string value)
		{
			if (_isInternalUpdate) return;
			Recalculate();
		}

		partial void OnSatuanListChanged(ObservableCollection<string> value)
		{
			OnPropertyChanged(nameof(Varian));
		}

		#endregion

		#region Public API

		public void Initialize()
		{
			Recalculate();
			CaptureOriginalState();
		}

		public void CaptureOriginalState()
		{
			_originalNamaBarang = NamaBarang;
			_originalSatuan = Satuan;
			_originalHargaJual = HargaJual;
		}

		public void Restore()
		{
			_isInternalUpdate = true;

			NamaBarang = _originalNamaBarang;
			Satuan = _originalSatuan;
			HargaJual = _originalHargaJual;

			_isInternalUpdate = false;
		}

		public bool IsDirty =>
			NamaBarang != _originalNamaBarang ||
			Satuan != _originalSatuan ||
			HargaJual != _originalHargaJual;

		#endregion

		#region Core Logic (READ ONLY)

		private void Recalculate()
		{
			var basePs = ProdukSatuans.FirstOrDefault(ps => ps.JumlahPerSatuan == 1);
			if (basePs == null) return;

			var totalSmallest = basePs.ProdukBatchs?.Sum(b => b.Stok) ?? 0;

			var selectedPs = ProdukSatuans
				.FirstOrDefault(ps => ps.Satuan?.NamaSatuan == Satuan);

			if (selectedPs == null) return;

			_isInternalUpdate = true;

			CurrentStok = totalSmallest / selectedPs.JumlahPerSatuan;
			HargaJual = selectedPs.HargaJual;

			_isInternalUpdate = false;
		}

		public void RefreshAfterSave()
		{
			Recalculate();
		}

		#endregion
	}
}
