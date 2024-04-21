using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnsibeProject.Migrations
{
    /// <inheritdoc />
    public partial class CreateTableContracts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Professors_Contract_ContractType",
                table: "Professors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Contract",
                table: "Contract");

            migrationBuilder.RenameTable(
                name: "Contract",
                newName: "Contracts");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Contracts",
                table: "Contracts",
                column: "ContractType");

            migrationBuilder.AddForeignKey(
                name: "FK_Professors_Contracts_ContractType",
                table: "Professors",
                column: "ContractType",
                principalTable: "Contracts",
                principalColumn: "ContractType",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Professors_Contracts_ContractType",
                table: "Professors");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Contracts",
                table: "Contracts");

            migrationBuilder.RenameTable(
                name: "Contracts",
                newName: "Contract");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Contract",
                table: "Contract",
                column: "ContractType");

            migrationBuilder.AddForeignKey(
                name: "FK_Professors_Contract_ContractType",
                table: "Professors",
                column: "ContractType",
                principalTable: "Contract",
                principalColumn: "ContractType",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
