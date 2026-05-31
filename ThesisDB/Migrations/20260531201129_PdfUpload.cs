using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThesisDB.Migrations
{
    /// <inheritdoc />
    public partial class PdfUpload : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PdfFileName",
                table: "Theses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PdfFileName",
                table: "Theses");
        }
    }
}
