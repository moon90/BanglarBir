using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BanglarBir.Migrations
{
    /// <inheritdoc />
    public partial class updateVictimModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VolunteerId",
                table: "Victims",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VolunteerId",
                table: "Victims");
        }
    }
}
