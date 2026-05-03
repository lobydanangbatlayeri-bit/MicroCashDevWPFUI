using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicroCashDevWPFUI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AwalSetHardDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Suppliers");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Satuans");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Produks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Suppliers",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Satuans",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Produks",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
