using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokeMovedle.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "matchups",
                columns: table => new
                {
                    attacker = table.Column<int>(type: "INTEGER", nullable: false),
                    defender = table.Column<int>(type: "INTEGER", nullable: false),
                    multiplier = table.Column<float>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_matchups", x => x.attacker);
                });

            migrationBuilder.CreateTable(
                name: "moves",
                columns: table => new
                {
                    id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    name = table.Column<string>(type: "TEXT", nullable: false),
                    power = table.Column<int>(type: "INTEGER", nullable: true),
                    pp = table.Column<int>(type: "INTEGER", nullable: true),
                    accuracy = table.Column<int>(type: "INTEGER", nullable: true),
                    type = table.Column<int>(type: "INTEGER", nullable: false),
                    damageClass = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_moves", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "matchups");

            migrationBuilder.DropTable(
                name: "moves");
        }
    }
}
