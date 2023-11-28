using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KhaccThienn_Ecommerce.Migrations
{
    /// <inheritdoc />
    public partial class v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "Status",
                table: "Order",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0,
                oldClrType: typeof(byte),
                oldType: "tinyint");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte>(
                name: "Status",
                table: "Order",
                type: "tinyint",
                nullable: false,
                oldClrType: typeof(byte),
                oldType: "tinyint",
                oldDefaultValue: (byte)0);

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 0,
                column: "Created_Date",
                value: new DateTime(2023, 11, 26, 0, 10, 21, 533, DateTimeKind.Local).AddTicks(6879));

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 1,
                column: "Created_Date",
                value: new DateTime(2023, 11, 26, 0, 10, 21, 533, DateTimeKind.Local).AddTicks(6896));
        }
    }
}
