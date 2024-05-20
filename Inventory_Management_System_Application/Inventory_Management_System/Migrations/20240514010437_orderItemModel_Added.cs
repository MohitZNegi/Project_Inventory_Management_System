using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory_Management_System.Migrations
{
    public partial class orderItemModel_Added : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrderItem_Model",
                columns: table => new
                {
                    OrderItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItem_Model", x => x.OrderItemId);
                    table.ForeignKey(
                        name: "FK_OrderItem_Model_Order_Model_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Order_Model",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItem_Model_Product_Model_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product_Model",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_Model_OrderId",
                table: "OrderItem_Model",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItem_Model_ProductId",
                table: "OrderItem_Model",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItem_Model");
        }
    }
}
