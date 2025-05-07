using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLNT.Migrations
{
    /// <inheritdoc />
    public partial class AddMeterLogFieldsToInvoiceDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MeterLogId",
                table: "InvoiceDetails",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MeterName",
                table: "InvoiceDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Month",
                table: "InvoiceDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "NewReading",
                table: "InvoiceDetails",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OldReading",
                table: "InvoiceDetails",
                type: "decimal(18,2)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MeterLogId",
                table: "InvoiceDetails");

            migrationBuilder.DropColumn(
                name: "MeterName",
                table: "InvoiceDetails");

            migrationBuilder.DropColumn(
                name: "Month",
                table: "InvoiceDetails");

            migrationBuilder.DropColumn(
                name: "NewReading",
                table: "InvoiceDetails");

            migrationBuilder.DropColumn(
                name: "OldReading",
                table: "InvoiceDetails");
        }
    }
}
