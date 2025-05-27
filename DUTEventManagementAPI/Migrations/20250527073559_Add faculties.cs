using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace DUTEventManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class Addfaculties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Faculties",
                columns: table => new
                {
                    FacultyId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FacultyName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Faculties", x => x.FacultyId);
                });

            migrationBuilder.InsertData(
                table: "Faculties",
                columns: new[] { "FacultyId", "FacultyName" },
                values: new object[,]
                {
                    { "101", "K. Cơ khí" },
                    { "102", "K. Công nghệ Thông tin" },
                    { "103", "K. Cơ khí Giao thông" },
                    { "104", "K. Công nghệ Nhiệt - Điện lạnh" },
                    { "105", "K. Điện" },
                    { "106", "K. Điện tử Viễn thông" },
                    { "107", "K. Hóa" },
                    { "109", "K. Xây dựng Cầu - Đường" },
                    { "110", "K. Xây dựng Dân dụng - Công nghiệp" },
                    { "111", "K. Xây dựng công trình thủy" },
                    { "117", "K. Môi trường" },
                    { "118", "K. Quản lý dự án" },
                    { "121", "K. Kiến trúc" },
                    { "123", "K. Khoa học Công nghệ tiên tiến" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Faculties");
        }
    }
}
