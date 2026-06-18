using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Optivio.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddProductFaceShapes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductFaceShapes",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    FaceShape = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductFaceShapes", x => new { x.ProductId, x.FaceShape });
                    table.ForeignKey(
                        name: "FK_ProductFaceShapes_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductFaceShapes");
        }
    }
}
