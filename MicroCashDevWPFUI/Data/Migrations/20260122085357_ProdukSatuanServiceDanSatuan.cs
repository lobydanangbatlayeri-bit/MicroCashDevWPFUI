using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicroCashDevWPFUI.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProdukSatuanServiceDanSatuan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProdukSatuans_Produks_ProdukId",
                table: "ProdukSatuans");

            migrationBuilder.DropForeignKey(
                name: "FK_ProdukSatuans_Satuans_SatuanId",
                table: "ProdukSatuans");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Satuans");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ProdukSatuans");

            migrationBuilder.DropColumn(
                name: "IsBaseUnit",
                table: "ProdukSatuans");

            migrationBuilder.AddForeignKey(
                name: "FK_ProdukSatuans_Produks_ProdukId",
                table: "ProdukSatuans",
                column: "ProdukId",
                principalTable: "Produks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProdukSatuans_Satuans_SatuanId",
                table: "ProdukSatuans",
                column: "SatuanId",
                principalTable: "Satuans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProdukSatuans_Produks_ProdukId",
                table: "ProdukSatuans");

            migrationBuilder.DropForeignKey(
                name: "FK_ProdukSatuans_Satuans_SatuanId",
                table: "ProdukSatuans");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Satuans",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ProdukSatuans",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsBaseUnit",
                table: "ProdukSatuans",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_ProdukSatuans_Produks_ProdukId",
                table: "ProdukSatuans",
                column: "ProdukId",
                principalTable: "Produks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProdukSatuans_Satuans_SatuanId",
                table: "ProdukSatuans",
                column: "SatuanId",
                principalTable: "Satuans",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
