using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThesisDB.Migrations
{
    /// <inheritdoc />
    public partial class AddProgrammeModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProgrammeId",
                table: "Theses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Programmes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Programmes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Theses_ProgrammeId",
                table: "Theses",
                column: "ProgrammeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Theses_Programmes_ProgrammeId",
                table: "Theses",
                column: "ProgrammeId",
                principalTable: "Programmes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Theses_Programmes_ProgrammeId",
                table: "Theses");

            migrationBuilder.DropTable(
                name: "Programmes");

            migrationBuilder.DropIndex(
                name: "IX_Theses_ProgrammeId",
                table: "Theses");

            migrationBuilder.DropColumn(
                name: "ProgrammeId",
                table: "Theses");
        }
    }
}
