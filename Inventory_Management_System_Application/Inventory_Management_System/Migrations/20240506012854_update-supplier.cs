using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory_Management_System.Migrations
{
    public partial class updatesupplier : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Product_Model_Supplier_Model_SupplierID",
                table: "Product_Model");

            migrationBuilder.DropIndex(
                name: "IX_Product_Model_SupplierID",
                table: "Product_Model");

            migrationBuilder.DropColumn(
                name: "SupplierID",
                table: "Product_Model");

            migrationBuilder.AddColumn<string>(
                name: "Products",
                table: "Supplier_Model",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Products",
                table: "Supplier_Model");

            migrationBuilder.AddColumn<int>(
                name: "SupplierID",
                table: "Product_Model",
                type: "int",
                nullable: true);

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
    }
}
