using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory_Management_System.Migrations
{
    public partial class ProductSupplier_Model_added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductSupplier_Model",
                columns: table => new
                {
                    ProductSupplierId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductID = table.Column<int>(type: "int", nullable: false),
                    SupplierID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductSupplier_Model", x => x.ProductSupplierId);
                    table.ForeignKey(
                        name: "FK_ProductSupplier_Model_Product_Model_ProductID",
                        column: x => x.ProductID,
                        principalTable: "Product_Model",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductSupplier_Model_Supplier_Model_SupplierID",
                        column: x => x.SupplierID,
                        principalTable: "Supplier_Model",
                        principalColumn: "SupplierID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductSupplier_Model_ProductID",
                table: "ProductSupplier_Model",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductSupplier_Model_SupplierID",
                table: "ProductSupplier_Model",
                column: "SupplierID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductSupplier_Model");
        }
    }
}
