using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MicroCashDevWPFUI.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDetailPenjualanBatch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DetailPenjualanBatches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DetailPenjualanId = table.Column<int>(type: "INTEGER", nullable: false),
                    ProdukBatchId = table.Column<int>(type: "INTEGER", nullable: false),
                    Jumlah = table.Column<int>(type: "INTEGER", nullable: false),
                    HargaBeli = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetailPenjualanBatches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DetailPenjualanBatches_DetailPenjualans_DetailPenjualanId",
                        column: x => x.DetailPenjualanId,
                        principalTable: "DetailPenjualans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DetailPenjualanBatches_ProdukBatchs_ProdukBatchId",
                        column: x => x.ProdukBatchId,
                        principalTable: "ProdukBatchs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DetailPenjualanBatches_DetailPenjualanId",
                table: "DetailPenjualanBatches",
                column: "DetailPenjualanId");

            migrationBuilder.CreateIndex(
                name: "IX_DetailPenjualanBatches_ProdukBatchId",
                table: "DetailPenjualanBatches",
                column: "ProdukBatchId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DetailPenjualanBatches");
        }
    }
}
