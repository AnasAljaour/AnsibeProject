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
                name: "Contracts",
                columns: table => new
                {
                    ContractType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MaxHours = table.Column<int>(type: "int", nullable: true),
                    MinHours = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.ContractType);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    CourseCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
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
                    FileNumber = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MiddleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Speciality = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FullNameInArabic = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Rank = table.Column<int>(type: "int", nullable: false),
                    ActiveState = table.Column<int>(type: "int", nullable: false),
                    ContractType = table.Column<string>(type: "nvarchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Professors", x => x.FileNumber);
                    table.ForeignKey(
                        name: "FK_Professors_Contracts_ContractType",
                        column: x => x.ContractType,
                        principalTable: "Contracts",
                        principalColumn: "ContractType",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Professors_ContractType",
                table: "Professors",
                column: "ContractType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Professors");

            migrationBuilder.DropTable(
                name: "Contracts");
        }
    }
}
