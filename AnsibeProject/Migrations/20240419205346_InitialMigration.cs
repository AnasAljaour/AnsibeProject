using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnsibeProject.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    CourseCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CourseDescription = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NumberOfCredits = table.Column<int>(type: "int", nullable: false),
                    TotalNumberOfHours = table.Column<int>(type: "int", nullable: false),
                    NumberOfHours = table.Column<int>(type: "int", nullable: false),
                    TP = table.Column<int>(type: "int", nullable: false),
                    TD = table.Column<int>(type: "int", nullable: false),
                    Semester = table.Column<int>(type: "int", nullable: false),
                    CourseState = table.Column<int>(type: "int", nullable: false),
                    Major = table.Column<int>(type: "int", nullable: false),
                    Obligatory = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.CourseCode);
                });

            migrationBuilder.CreateTable(
                name: "Professors",
                columns: table => new
                {
                    FileNumber = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Speciality = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FullNameInArabic = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ContractType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Professors", x => x.FileNumber);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Professors");
        }
    }
}
