using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TradeUp.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserUptionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "62a40f40-b15f-496d-8b82-2612e44f8517");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "74abb300-5844-47c1-8e99-6b21a0c1a2b2");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "7ba0a0f5-8124-4a6e-afc1-f5bdb2be0ee4", "e877407a-5b4b-4de3-b36d-76ddb93fa98d", "User", "USER" },
                    { "d8948491-fccf-4dfc-95c1-2febd6e55749", "dd8e3cbc-e822-46a2-ae6a-3a2e41cede43", "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "7ba0a0f5-8124-4a6e-afc1-f5bdb2be0ee4");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d8948491-fccf-4dfc-95c1-2febd6e55749");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "62a40f40-b15f-496d-8b82-2612e44f8517", "48a10582-4d03-4267-a046-a2af95b3fd4f", "Admin", "ADMIN" },
                    { "74abb300-5844-47c1-8e99-6b21a0c1a2b2", "d89353dd-3eca-4d7b-b28b-fac80d54bf88", "User", "USER" }
                });
        }
    }
}
