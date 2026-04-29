using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ThesisDB.Migrations
{
    /// <inheritdoc />
    public partial class MigrationMitStudentSupervisorReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StudentId",
                table: "Theses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SupervisorId",
                table: "Theses",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Reviews",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Summary = table.Column<string>(type: "TEXT", nullable: false),
                    Strengths = table.Column<string>(type: "TEXT", nullable: false),
                    Weaknesses = table.Column<string>(type: "TEXT", nullable: false),
                    Evaluation = table.Column<string>(type: "TEXT", nullable: false),
                    ContentVal = table.Column<int>(type: "INTEGER", nullable: false),
                    LayoutVal = table.Column<int>(type: "INTEGER", nullable: false),
                    StructureVal = table.Column<int>(type: "INTEGER", nullable: false),
                    StyleVal = table.Column<int>(type: "INTEGER", nullable: false),
                    LiteratureVal = table.Column<int>(type: "INTEGER", nullable: false),
                    DifficultyVal = table.Column<int>(type: "INTEGER", nullable: false),
                    NoveltyVal = table.Column<int>(type: "INTEGER", nullable: false),
                    RichnessVal = table.Column<int>(type: "INTEGER", nullable: false),
                    ContentWt = table.Column<int>(type: "INTEGER", nullable: false),
                    LayoutWt = table.Column<int>(type: "INTEGER", nullable: false),
                    StructureWt = table.Column<int>(type: "INTEGER", nullable: false),
                    StyleWt = table.Column<int>(type: "INTEGER", nullable: false),
                    LiteratureWt = table.Column<int>(type: "INTEGER", nullable: false),
                    DifficultyWt = table.Column<int>(type: "INTEGER", nullable: false),
                    NoveltyWt = table.Column<int>(type: "INTEGER", nullable: false),
                    RichnessWt = table.Column<int>(type: "INTEGER", nullable: false),
                    Grade = table.Column<double>(type: "REAL", nullable: false),
                    ThesisId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reviews_Theses_ThesisId",
                        column: x => x.ThesisId,
                        principalTable: "Theses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    MatriculationNumber = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Students", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Supervisors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Active = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supervisors", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Theses_StudentId",
                table: "Theses",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Theses_SupervisorId",
                table: "Theses",
                column: "SupervisorId");

            migrationBuilder.CreateIndex(
                name: "IX_Reviews_ThesisId",
                table: "Reviews",
                column: "ThesisId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Theses_Students_StudentId",
                table: "Theses",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Theses_Supervisors_SupervisorId",
                table: "Theses",
                column: "SupervisorId",
                principalTable: "Supervisors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Theses_Students_StudentId",
                table: "Theses");

            migrationBuilder.DropForeignKey(
                name: "FK_Theses_Supervisors_SupervisorId",
                table: "Theses");

            migrationBuilder.DropTable(
                name: "Reviews");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Supervisors");

            migrationBuilder.DropIndex(
                name: "IX_Theses_StudentId",
                table: "Theses");

            migrationBuilder.DropIndex(
                name: "IX_Theses_SupervisorId",
                table: "Theses");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "Theses");

            migrationBuilder.DropColumn(
                name: "SupervisorId",
                table: "Theses");
        }
    }
}
