using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookshelf.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Add_IsShelfPublic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_shelf_public",
                table: "users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_shelf_public",
                table: "users");
        }
    }
}
