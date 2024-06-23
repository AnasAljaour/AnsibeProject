using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnsibeProject.Migrations
{
    /// <inheritdoc />
    public partial class AnsibeSection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ansibes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ansibes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Sections",
                columns: table => new
                {
                    SectionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseCode = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    ProfessorFileNumber = table.Column<int>(type: "int", nullable: true),
                    TP = table.Column<int>(type: "int", nullable: true),
                    TD = table.Column<int>(type: "int", nullable: true),
                    CourseHours = table.Column<int>(type: "int", nullable: true),
                    Language = table.Column<int>(type: "int", nullable: false),
                    AnsibeId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sections", x => x.SectionId);
                    table.ForeignKey(
                        name: "FK_Sections_Ansibes_AnsibeId",
                        column: x => x.AnsibeId,
                        principalTable: "Ansibes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Sections_Courses_CourseCode",
                        column: x => x.CourseCode,
                        principalTable: "Courses",
                        principalColumn: "CourseCode",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sections_Professors_ProfessorFileNumber",
                        column: x => x.ProfessorFileNumber,
                        principalTable: "Professors",
                        principalColumn: "FileNumber");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Sections_AnsibeId",
                table: "Sections",
                column: "AnsibeId");

            migrationBuilder.CreateIndex(
                name: "IX_Sections_CourseCode",
                table: "Sections",
                column: "CourseCode");

            migrationBuilder.CreateIndex(
                name: "IX_Sections_ProfessorFileNumber",
                table: "Sections",
                column: "ProfessorFileNumber");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Sections");

            migrationBuilder.DropTable(
                name: "Ansibes");
        }
    }
}
