using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BanglarBir.Migrations
{
    /// <inheritdoc />
    public partial class updateVolunteerModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NIdOrStudentId",
                table: "Volunteers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NIdOrStudentId",
                table: "Volunteers");
        }
    }
}
