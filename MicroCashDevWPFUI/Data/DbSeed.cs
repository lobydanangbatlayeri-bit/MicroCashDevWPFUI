using MicroCashDevWPFUI.Data;
using MicroCashDevWPFUI.Models;
using MicroCashDevWPFUI.Helpers;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MicroCashDevWPFUI.Data
{
	public static class DbSeed
	{
		public static void Initialize(AppDbContext? context)
		{
			if (context == null) return;

			bool changed = false;

			// ========== User Admin ==========
			var admin = context.Users.FirstOrDefault(u => u.Username == "admin");
			if (admin == null)
			{
				admin = new User
				{
					Username = "admin",
					Password = PasswordHelper.HashPassword("admin")
				};
				context.Users.Add(admin);
				changed = true;
			}

			// ========== User Karyawan ==========
			var karyawan = context.Users.FirstOrDefault(u => u.Username == "karyawan");
			if (karyawan == null)
			{
				karyawan = new User
				{
					Username = "karyawan",
					Password = PasswordHelper.HashPassword("karyawan")
				};
				context.Users.Add(karyawan);
				changed = true;
			}

			if (changed) context.SaveChanges();

			// ========== Menu Default ==========
			if (!context.Menus.Any())
			{
				var dashboard = new Menu { NamaMenu = "Dashboard", PageKey = "DashboardPage", Icon = "ChartMultiple20", Urutan = 1 };
				var tambahProduk = new Menu { NamaMenu = "Inisialisasi Produk", PageKey = "TambahProdukPage", Icon = "ClipboardBulletListLtr20", Urutan = 2 };

				var restok = new Menu { NamaMenu = "Restok", Icon = "ArchiveArrowBack20", Urutan = 3 };
				restok.Children.Add(new Menu { NamaMenu = "Restok Manual", PageKey = "RestokManualPage", Icon = "BookLetter20", Urutan = 1 });
				restok.Children.Add(new Menu { NamaMenu = "Restok Nota", PageKey = "RestokNotaPage", Icon = "BookPulse20", Urutan = 2 });

				var riwayatPembelian = new Menu { NamaMenu = "Riwayat Pembelian", Icon = "CartArrowDown20", Urutan = 3 };
				riwayatPembelian.Children.Add(new Menu { NamaMenu = "Perhari", PageKey = "PembelianPerhariPage", Icon = "CalendarWeekNumber20", Urutan = 1 });
				riwayatPembelian.Children.Add(new Menu { NamaMenu = "Perbulan", PageKey = "PembelianPerbulanPage", Icon = "CalendarMonth20", Urutan = 2 });
				riwayatPembelian.Children.Add(new Menu { NamaMenu = "Pertahun", PageKey = "PembelianPertahunPage", Icon = "CalendarArrowCounterclockwise20", Urutan = 3 });

				var produk = new Menu { NamaMenu = "Produk", PageKey = "ProdukPage", Icon = "Box20", Urutan = 4 };

				var penjualan = new Menu { NamaMenu = "Penjualan", PageKey = "PenjualanPage", Icon = "Money20", Urutan = 5 };

				var riwayatPenjualan = new Menu { NamaMenu = "Riwayat Penjualan", Icon = "History20", Urutan = 6 };
				riwayatPenjualan.Children.Add(new Menu { NamaMenu = "Perhari", PageKey = "PenjualanPerhariPage", Icon = "CalendarWeekNumber20", Urutan = 1 });
				riwayatPenjualan.Children.Add(new Menu { NamaMenu = "Perbulan", PageKey = "PenjualanPerbulanPage", Icon = "CalendarMonth20", Urutan = 2 });
				riwayatPenjualan.Children.Add(new Menu { NamaMenu = "Pertahun", PageKey = "PenjualanPertahunPage", Icon = "CalendraArrowCounterclockwise20", Urutan = 3 });

				var nota = new Menu { NamaMenu = "Nota", Icon = "Check20", Urutan = 7 };
				nota.Children.Add(new Menu { NamaMenu = "Tambah Nota", PageKey = "TambahNotaPage", Icon = "ClipboardTextEdit20", Urutan = 1 });
				nota.Children.Add(new Menu { NamaMenu = "Daftar Nota Belum Dibayar", PageKey = "NotaBelumBayarPage", Icon = "ClipboardPulse20", Urutan = 2 });
				nota.Children.Add(new Menu { NamaMenu = "Daftar Nota Sudah Dibayar", PageKey = "NotaSudahBayarPage", Icon = "ClipboardTask20", Urutan = 3 });

				var laba = new Menu { NamaMenu = "Laba", Icon = "ColorLine20", Urutan = 8 };
				laba.Children.Add(new Menu { NamaMenu = "Perhari", PageKey = "LabaPerhariPage", Icon = "ClipboardTextEdit20", Urutan = 1 });
				laba.Children.Add(new Menu { NamaMenu = "Perbulan", PageKey = "LabaPerbulanPage", Icon = "ClipboardPulse20", Urutan = 2 });
				laba.Children.Add(new Menu { NamaMenu = "Pertahun", PageKey = "LabaPertahunPage", Icon = "ClipboardTask20", Urutan = 3 });

				var setting = new Menu { NamaMenu = "Setting", Icon = "Settings20", Urutan = 9 };
				setting.Children.Add(new Menu { NamaMenu = "Data User", PageKey = "DataUserPage", Icon = "PersonAccounts20", Urutan = 1 });
				setting.Children.Add(new Menu { NamaMenu = "Hak Akses", PageKey = "HakAksesPage", Icon = "Accessibility20", Urutan = 2 });
				setting.Children.Add(new Menu { NamaMenu = "Profile Toko", PageKey = "ProfileTokoPage", Icon = "BuildingRetailMoney20", Urutan = 3 });

				context.Menus.AddRange(new[]
				{
					dashboard, tambahProduk, restok, riwayatPembelian, produk, penjualan,
					riwayatPenjualan, nota, laba, setting
				});

				changed = true;
			}

			// ========== Tambah Menu Scan Nota Jika Belum Ada ==========
			var scanNota = context.Menus.FirstOrDefault(m => m.PageKey == "ScanNotaPage");

			if (scanNota == null)
			{
				scanNota = new Menu
				{
					NamaMenu = "Scan Nota",
					PageKey = "ScanNotaPage",
					Icon = "QrCode20",
					Urutan = 10
				};

				context.Menus.Add(scanNota);
				changed = true;
			}

			if (changed) context.SaveChanges();

			// ========== Hubungkan Menu ke Admin ==========
			if (!context.UserMenus.Any(um => um.UserId == admin!.Id))
			{
				var allMenus = context.Menus.ToList();

				context.UserMenus.AddRange(
					allMenus.Select(m => new UserMenu
					{
						UserId = admin.Id,
						MenuId = m.Id
					})
				);

				changed = true;
			}

			// ========== Hubungkan Menu ke Karyawan ==========
			if (!context.UserMenus.Any(um => um.UserId == karyawan!.Id))
			{
				var allowedPageKeys = new[]
				{
					// Dashboard & Produk
					"DashboardPage",
					"TambahProdukPage",
					"ProdukPage",

					// Restok
					"RestokManualPage",
					"RestokNotaPage",

					// Penjualan
					"PenjualanPage",

					// Nota
					"TambahNotaPage",
					"NotaBelumBayarPage",
					"NotaSudahBayarPage"
				};


				var allowedMenus = context.Menus
					.Where(m => allowedPageKeys.Contains(m.PageKey))
					.ToList();

				// Include Parent jika punya hierarki
				var menusToAdd = new List<Menu>();
				foreach (var menu in allowedMenus)
				{
					var current = menu;
					while (current != null)
					{
						if (!menusToAdd.Contains(current))
							menusToAdd.Add(current);
						current = current.Parent;
					}
				}

				context.UserMenus.AddRange(
					menusToAdd.Select(m => new UserMenu
					{
						UserId = karyawan.Id,
						MenuId = m.Id
					})
				);

				changed = true;
			}

			// ========== Dummy Supplier untuk Restok Manual ==========
			var dummySupplier = context.Suppliers.FirstOrDefault(s => s.NamaSupplier == "Restok Manual");
			if (dummySupplier == null)
			{
				dummySupplier = new Supplier
				{
					NamaSupplier = "Restok Manual",
					Alamat = "Default"
				};
				context.Suppliers.Add(dummySupplier);
				changed = true;
			}

			if (changed) context.SaveChanges();
		}
	}
}
