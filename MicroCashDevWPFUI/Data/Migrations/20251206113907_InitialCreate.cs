using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicroCashDevWPFUI.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Menus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ParentId = table.Column<int>(type: "INTEGER", nullable: true),
                    NamaMenu = table.Column<string>(type: "TEXT", nullable: false),
                    PageKey = table.Column<string>(type: "TEXT", nullable: false),
                    Icon = table.Column<string>(type: "TEXT", nullable: false),
                    Urutan = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Menus_Menus_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Menus",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Produks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NamaBarang = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProfileTokos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NamaToko = table.Column<string>(type: "TEXT", nullable: false),
                    Alamat = table.Column<string>(type: "TEXT", nullable: false),
                    KataSambutan = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileTokos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Satuans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NamaSatuan = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Satuans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    NamaSupplier = table.Column<string>(type: "TEXT", nullable: false),
                    Alamat = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProdukSatuans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProdukId = table.Column<int>(type: "INTEGER", nullable: false),
                    SatuanId = table.Column<int>(type: "INTEGER", nullable: false),
                    JumlahPerSatuan = table.Column<int>(type: "INTEGER", nullable: false),
                    HargaJual = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProdukSatuans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProdukSatuans_Produks_ProdukId",
                        column: x => x.ProdukId,
                        principalTable: "Produks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProdukSatuans_Satuans_SatuanId",
                        column: x => x.SatuanId,
                        principalTable: "Satuans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Banks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SupplierId = table.Column<int>(type: "INTEGER", nullable: false),
                    NamaBank = table.Column<string>(type: "TEXT", nullable: false),
                    NomorRekening = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Banks_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pembelians",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SupplierId = table.Column<int>(type: "INTEGER", nullable: false),
                    NomorFaktur = table.Column<string>(type: "TEXT", nullable: false),
                    Tanggal = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Total = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pembelians", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pembelians_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Penjualans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    NomorNota = table.Column<string>(type: "TEXT", nullable: false),
                    Tanggal = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Total = table.Column<decimal>(type: "TEXT", nullable: false),
                    Dibayar = table.Column<decimal>(type: "TEXT", nullable: false),
                    Kembalian = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Penjualans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Penjualans_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserMenus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    MenuId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMenus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserMenus_Menus_MenuId",
                        column: x => x.MenuId,
                        principalTable: "Menus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserMenus_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetailPembelians",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PembelianId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProdukSatuanId = table.Column<int>(type: "INTEGER", nullable: false),
                    Jumlah = table.Column<int>(type: "INTEGER", nullable: false),
                    HargaBeli = table.Column<decimal>(type: "TEXT", nullable: false),
                    Subtotal = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetailPembelians", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetailPembelians_Pembelians_PembelianId",
                        column: x => x.PembelianId,
                        principalTable: "Pembelians",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetailPembelians_ProdukSatuans_ProdukSatuanId",
                        column: x => x.ProdukSatuanId,
                        principalTable: "ProdukSatuans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BankId = table.Column<int>(type: "INTEGER", nullable: false),
                    PembelianId = table.Column<int>(type: "INTEGER", nullable: false),
                    StatusPembayaran = table.Column<string>(type: "TEXT", nullable: false),
                    TanggalDeadline = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Total = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notas_Banks_BankId",
                        column: x => x.BankId,
                        principalTable: "Banks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Notas_Pembelians_PembelianId",
                        column: x => x.PembelianId,
                        principalTable: "Pembelians",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DetailPenjualans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PenjualanId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProdukSatuanId = table.Column<int>(type: "INTEGER", nullable: false),
                    Jumlah = table.Column<int>(type: "INTEGER", nullable: false),
                    Harga = table.Column<decimal>(type: "TEXT", nullable: false),
                    Subtotal = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetailPenjualans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetailPenjualans_Penjualans_PenjualanId",
                        column: x => x.PenjualanId,
                        principalTable: "Penjualans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetailPenjualans_ProdukSatuans_ProdukSatuanId",
                        column: x => x.ProdukSatuanId,
                        principalTable: "ProdukSatuans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProdukBatchs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProdukSatuanId = table.Column<int>(type: "INTEGER", nullable: false),
                    DetailPembelianId = table.Column<int>(type: "INTEGER", nullable: true),
                    BatchNumber = table.Column<string>(type: "TEXT", nullable: true),
                    TanggalMasuk = table.Column<DateTime>(type: "TEXT", nullable: false),
                    TanggalKadarluasa = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Stok = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProdukBatchs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProdukBatchs_DetailPembelians_DetailPembelianId",
                        column: x => x.DetailPembelianId,
                        principalTable: "DetailPembelians",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProdukBatchs_ProdukSatuans_ProdukSatuanId",
                        column: x => x.ProdukSatuanId,
                        principalTable: "ProdukSatuans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Banks_SupplierId",
                table: "Banks",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_DetailPembelians_PembelianId",
                table: "DetailPembelians",
                column: "PembelianId");

            migrationBuilder.CreateIndex(
                name: "IX_DetailPembelians_ProdukSatuanId",
                table: "DetailPembelians",
                column: "ProdukSatuanId");

            migrationBuilder.CreateIndex(
                name: "IX_DetailPenjualans_PenjualanId",
                table: "DetailPenjualans",
                column: "PenjualanId");

            migrationBuilder.CreateIndex(
                name: "IX_DetailPenjualans_ProdukSatuanId",
                table: "DetailPenjualans",
                column: "ProdukSatuanId");

            migrationBuilder.CreateIndex(
                name: "IX_Menus_ParentId",
                table: "Menus",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Notas_BankId",
                table: "Notas",
                column: "BankId");

            migrationBuilder.CreateIndex(
                name: "IX_Notas_PembelianId",
                table: "Notas",
                column: "PembelianId");

            migrationBuilder.CreateIndex(
                name: "IX_Pembelians_SupplierId",
                table: "Pembelians",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_Penjualans_UserId",
                table: "Penjualans",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProdukBatchs_DetailPembelianId",
                table: "ProdukBatchs",
                column: "DetailPembelianId");

            migrationBuilder.CreateIndex(
                name: "IX_ProdukBatchs_ProdukSatuanId",
                table: "ProdukBatchs",
                column: "ProdukSatuanId");

            migrationBuilder.CreateIndex(
                name: "IX_ProdukSatuans_ProdukId",
                table: "ProdukSatuans",
                column: "ProdukId");

            migrationBuilder.CreateIndex(
                name: "IX_ProdukSatuans_SatuanId",
                table: "ProdukSatuans",
                column: "SatuanId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMenus_MenuId",
                table: "UserMenus",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMenus_UserId",
                table: "UserMenus",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetailPenjualans");

            migrationBuilder.DropTable(
                name: "Notas");

            migrationBuilder.DropTable(
                name: "ProdukBatchs");

            migrationBuilder.DropTable(
                name: "ProfileTokos");

            migrationBuilder.DropTable(
                name: "UserMenus");

            migrationBuilder.DropTable(
                name: "Penjualans");

            migrationBuilder.DropTable(
                name: "Banks");

            migrationBuilder.DropTable(
                name: "DetailPembelians");

            migrationBuilder.DropTable(
                name: "Menus");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Pembelians");

            migrationBuilder.DropTable(
                name: "ProdukSatuans");

            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.DropTable(
                name: "Produks");

            migrationBuilder.DropTable(
                name: "Satuans");
        }
    }
}
