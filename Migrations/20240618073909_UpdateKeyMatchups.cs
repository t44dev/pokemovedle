using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PokeMovedle.Migrations
{
    /// <inheritdoc />
    public partial class UpdateKeyMatchups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_matchups",
                table: "matchups");

            migrationBuilder.AddPrimaryKey(
                name: "PK_matchups",
                table: "matchups",
                columns: new[] { "attacker", "defender" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_matchups",
                table: "matchups");

            migrationBuilder.AddPrimaryKey(
                name: "PK_matchups",
                table: "matchups",
                column: "attacker");
        }
    }
}
