using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicroCashDevWPFUI.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateNotaBankSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notas_Banks_BankId",
                table: "Notas");

            migrationBuilder.DropIndex(
                name: "IX_Notas_BankId",
                table: "Notas");

            migrationBuilder.DropColumn(
                name: "BankId",
                table: "Notas");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BankId",
                table: "Notas",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Notas_BankId",
                table: "Notas",
                column: "BankId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notas_Banks_BankId",
                table: "Notas",
                column: "BankId",
                principalTable: "Banks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
