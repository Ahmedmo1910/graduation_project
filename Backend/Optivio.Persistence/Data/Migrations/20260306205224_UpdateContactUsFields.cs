using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Optivio.Persistence.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateContactUsFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "ContactUs");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "ContactUs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "ContactUs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "ContactUs",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OrderNumber",
                table: "ContactUs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "ContactUs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "ContactUs",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                table: "ContactUs");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "ContactUs");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "ContactUs");

            migrationBuilder.DropColumn(
                name: "OrderNumber",
                table: "ContactUs");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "ContactUs");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "ContactUs");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ContactUs",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }
    }
}
