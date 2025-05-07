using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLNT.Migrations
{
    /// <inheritdoc />
    public partial class OptimizeInvoiceRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_Contracts_ContractId1",
                table: "Invoices");

            migrationBuilder.DropIndex(
                name: "IX_Invoices_ContractId1",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "ContractId1",
                table: "Invoices");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContractId1",
                table: "Invoices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Invoices_ContractId1",
                table: "Invoices",
                column: "ContractId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_Contracts_ContractId1",
                table: "Invoices",
                column: "ContractId1",
                principalTable: "Contracts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
