using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory_Management_System.Migrations
{
    public partial class removeforeignkey : Migration
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SupplierID",
                table: "Product_Model",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Product_Model_SupplierID",
                table: "Product_Model",
                column: "SupplierID");

            migrationBuilder.AddForeignKey(
                name: "FK_Product_Model_Supplier_Model_SupplierID",
                table: "Product_Model",
                column: "SupplierID",
                principalTable: "Supplier_Model",
                principalColumn: "SupplierID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
