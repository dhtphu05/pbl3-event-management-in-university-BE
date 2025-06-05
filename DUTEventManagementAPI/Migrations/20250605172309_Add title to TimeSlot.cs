using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DUTEventManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddtitletoTimeSlot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "TimeSlots",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "TimeSlots");
        }
    }
}
