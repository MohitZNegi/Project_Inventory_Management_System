using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inventory_Management_System.Migrations
{
    public partial class productmodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Product_Model",
                columns: table => new
                {
                    ProductID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProductDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ProductPrice = table.Column<float>(type: "real", nullable: false),
                    ProductQuantity = table.Column<int>(type: "int", nullable: false),
                    ProductImg = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product_Model", x => x.ProductID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Product_Model");
        }
    }
}
