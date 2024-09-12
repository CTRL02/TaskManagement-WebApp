using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace alot.Migrations
{
    /// <inheritdoc />
    public partial class updatedLogout : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "identity",
                table: "TodoLists",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "TodoLists",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "TodoLists",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "1fd64f8c-5847-427d-93cb-44965ba6a3d2");

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "c96716f6-6fc8-48df-86f7-17766dd0ecb5");

            migrationBuilder.CreateTable(
                name: "validator",
                schema: "identity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_validator", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "validator",
                schema: "identity");

            migrationBuilder.InsertData(
                schema: "identity",
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "1fd64f8c-5847-427d-93cb-44965ba6a3d2", 0, "78e64163-c633-4872-9fa6-2213ed9128c1", "user1@example.com", true, false, null, "USER1@EXAMPLE.COM", "USER1", "AQAAAAIAAYagAAAAENxvvFGt8+fyRJ4Rf2oVzK9TUsSuwQ3MT9KSbe65avhJWTdw/ZN8pyQBLI7vlOtTlA==", null, false, "80f647c1-6a9f-411f-920a-3da0c8a72d30", false, "user1" },
                    { "c96716f6-6fc8-48df-86f7-17766dd0ecb5", 0, "0c6466db-fc6a-49b1-9e8d-d964de380034", "user2@example.com", true, false, null, "USER2@EXAMPLE.COM", "USER2", "AQAAAAIAAYagAAAAEKiVbqhvA/vzClM6MeEtEqVzFSvHp3K7e/6bfTeFbnWi9BY85CuL3pDVqXMGFehCaA==", null, false, "7219bb20-81d8-46cb-8666-1032553e8b0d", false, "user2" }
                });

            migrationBuilder.InsertData(
                schema: "identity",
                table: "TodoLists",
                columns: new[] { "Id", "DateTime", "IsCompleted", "Todo", "UserId" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 9, 4, 1, 30, 2, 791, DateTimeKind.Local).AddTicks(9417), false, "Todo 1 for user1", "1fd64f8c-5847-427d-93cb-44965ba6a3d2" },
                    { 2, new DateTime(2024, 9, 4, 1, 30, 2, 791, DateTimeKind.Local).AddTicks(9471), true, "Todo 2 for user1", "1fd64f8c-5847-427d-93cb-44965ba6a3d2" },
                    { 3, new DateTime(2024, 9, 4, 1, 30, 2, 791, DateTimeKind.Local).AddTicks(9474), false, "Todo 1 for user2", "c96716f6-6fc8-48df-86f7-17766dd0ecb5" }
                });
        }
    }
}
