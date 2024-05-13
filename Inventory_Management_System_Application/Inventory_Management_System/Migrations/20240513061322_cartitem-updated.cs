using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory_Management_System.Migrations
{
    public partial class cartitemupdated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "CartItem_Model");

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "Supplier_Model",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "ContactDetails",
                table: "Supplier_Model",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "ProductDescription",
                table: "Product_Model",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.CreateIndex(
                name: "IX_CartItem_Model_ProductId",
                table: "CartItem_Model",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItem_Model_Product_Model_ProductId",
                table: "CartItem_Model",
                column: "ProductId",
                principalTable: "Product_Model",
                principalColumn: "ProductID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItem_Model_Product_Model_ProductId",
                table: "CartItem_Model");

            migrationBuilder.DropIndex(
                name: "IX_CartItem_Model_ProductId",
                table: "CartItem_Model");

            migrationBuilder.AlterColumn<string>(
                name: "Location",
                table: "Supplier_Model",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ContactDetails",
                table: "Supplier_Model",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ProductDescription",
                table: "Product_Model",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "CartItem_Model",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
