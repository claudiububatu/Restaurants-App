using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IPSMDB.Migrations
{
    /// <inheritdoc />
    public partial class AddImageUrlToFoodPlace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "FoodPlace",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "FoodPlace");
        }
    }
}
