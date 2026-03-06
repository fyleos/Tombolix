using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TradeUp.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDrawContextAndDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DrawContexts",
                columns: table => new
                {
                    ID = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrawContexts", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DrawDatas",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    DrawContextId = table.Column<string>(type: "TEXT", nullable: false),
                    RawId = table.Column<string>(type: "TEXT", nullable: false),
                    DataKey = table.Column<string>(type: "TEXT", nullable: false),
                    DataValue = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrawDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DrawItems",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    DrawContextId = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrawItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DrawResults",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    ContextId = table.Column<string>(type: "TEXT", nullable: false),
                    TirageIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    DataRawId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrawResults", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DrawContexts");

            migrationBuilder.DropTable(
                name: "DrawDatas");

            migrationBuilder.DropTable(
                name: "DrawItems");

            migrationBuilder.DropTable(
                name: "DrawResults");
        }
    }
}
