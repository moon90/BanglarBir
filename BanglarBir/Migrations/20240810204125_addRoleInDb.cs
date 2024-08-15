using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BanglarBir.Migrations
{
    /// <inheritdoc />
    public partial class addRoleInDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Volunteers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Volunteers");
        }
    }
}
