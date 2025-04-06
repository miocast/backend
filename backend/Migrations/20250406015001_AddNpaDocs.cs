using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddNpaDocs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DocumentAnalys",
                schema: "hackathon",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    DocumentId = table.Column<string>(type: "text", nullable: false),
                    Text = table.Column<string>(type: "text", nullable: false),
                    Explanation = table.Column<string>(type: "text", nullable: false),
                    Regulation = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentAnalys", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DocumentNpas",
                schema: "hackathon",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    DocumentId = table.Column<string>(type: "text", nullable: false),
                    NpaText = table.Column<string>(type: "text", nullable: false),
                    Source = table.Column<string>(type: "text", nullable: false),
                    DistancePercent = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentNpas", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DocumentAnalys",
                schema: "hackathon");

            migrationBuilder.DropTable(
                name: "DocumentNpas",
                schema: "hackathon");
        }
    }
}
