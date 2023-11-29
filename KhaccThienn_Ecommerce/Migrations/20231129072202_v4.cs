using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KhaccThienn_Ecommerce.Migrations
{
    /// <inheritdoc />
    public partial class v4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contact",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(150)", nullable: false),
                    Email = table.Column<string>(type: "varchar(150)", nullable: false),
                    Messsage = table.Column<string>(type: "ntext", nullable: false),
                    Created_Date = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contact", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 0,
                columns: new[] { "Created_Date", "Descriptions" },
                values: new object[] { new DateTime(2023, 11, 29, 14, 22, 1, 808, DateTimeKind.Local).AddTicks(3576), "Admin Of Application" });

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created_Date", "Descriptions" },
                values: new object[] { new DateTime(2023, 11, 29, 14, 22, 1, 808, DateTimeKind.Local).AddTicks(3593), "User Of Application" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Contact");

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 0,
                columns: new[] { "Created_Date", "Descriptions" },
                values: new object[] { new DateTime(2023, 11, 27, 17, 7, 16, 989, DateTimeKind.Local).AddTicks(3078), null });

            migrationBuilder.UpdateData(
                table: "Role",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created_Date", "Descriptions" },
                values: new object[] { new DateTime(2023, 11, 27, 17, 7, 16, 989, DateTimeKind.Local).AddTicks(3110), null });
        }
    }
}
