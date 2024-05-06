using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory_Management_System.Migrations
{
    public partial class supplierModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Supplier",
                table: "Product_Model",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SupplierID",
                table: "Product_Model",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Supplier_Model",
                columns: table => new
                {
                    SupplierID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ContactDetails = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supplier_Model", x => x.SupplierID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Product_Model_SupplierID",
                table: "Product_Model",
                column: "SupplierID");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Model_Supplier_Model_SupplierID",
                table: "Product_Model",
                column: "SupplierID",
                principalTable: "Supplier_Model",
                principalColumn: "SupplierID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_Model_Supplier_Model_SupplierID",
                table: "Product_Model");

            migrationBuilder.DropTable(
                name: "Supplier_Model");

            migrationBuilder.DropIndex(
                name: "IX_Product_Model_SupplierID",
                table: "Product_Model");

            migrationBuilder.DropColumn(
                name: "Supplier",
                table: "Product_Model");

            migrationBuilder.DropColumn(
                name: "SupplierID",
                table: "Product_Model");
        }
    }
}
