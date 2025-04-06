using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddNpaDoc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                schema: "hackathon",
                table: "TechnicalSpecs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                schema: "hackathon",
                table: "TechnicalSpecs",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdate",
                schema: "hackathon",
                table: "TechnicalSpecs",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Status",
                schema: "hackathon",
                table: "TechnicalSpecs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "NpaDocuments",
                schema: "hackathon",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    FilePath = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NpaDocuments", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NpaDocuments",
                schema: "hackathon");

            migrationBuilder.DropColumn(
                name: "Category",
                schema: "hackathon",
                table: "TechnicalSpecs");

            migrationBuilder.DropColumn(
                name: "Description",
                schema: "hackathon",
                table: "TechnicalSpecs");

            migrationBuilder.DropColumn(
                name: "LastUpdate",
                schema: "hackathon",
                table: "TechnicalSpecs");

            migrationBuilder.DropColumn(
                name: "Status",
                schema: "hackathon",
                table: "TechnicalSpecs");
        }
    }
}
