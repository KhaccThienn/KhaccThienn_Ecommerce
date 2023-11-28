using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KhaccThienn_Ecommerce.Migrations
{
    /// <inheritdoc />
    public partial class v3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 0,
                column: "Created_Date",
                value: new DateTime(2023, 11, 27, 17, 7, 16, 989, DateTimeKind.Local).AddTicks(3078));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 1,
                column: "Created_Date",
                value: new DateTime(2023, 11, 27, 17, 7, 16, 989, DateTimeKind.Local).AddTicks(3110));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 0,
                column: "Created_Date",
                value: new DateTime(2023, 11, 27, 12, 18, 56, 965, DateTimeKind.Local).AddTicks(8920));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 1,
                column: "Created_Date",
                value: new DateTime(2023, 11, 27, 12, 18, 56, 965, DateTimeKind.Local).AddTicks(8934));
        }
    }
}
