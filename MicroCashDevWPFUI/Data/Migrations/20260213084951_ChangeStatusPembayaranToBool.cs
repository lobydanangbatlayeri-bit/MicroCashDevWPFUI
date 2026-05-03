using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicroCashDevWPFUI.Data.Migrations
{
    /// <inheritdoc />
    public partial class ChangeStatusPembayaranToBool : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "StatusPembayaran",
                table: "Notas",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "StatusPembayaran",
                table: "Notas",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "INTEGER");
        }
    }
}
