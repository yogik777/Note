using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Entities.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    CountryID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountryName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.CountryID);
                });

            migrationBuilder.CreateTable(
                name: "Persons",
                columns: table => new
                {
                    PersonID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonName = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CountryID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReceiveNewsLetters = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Persons", x => x.PersonID);
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "CountryID", "CountryName" },
                values: new object[,]
                {
                    { new Guid("307dbf16-e6f6-40c4-9999-d78687571605"), "Australia" },
                    { new Guid("455d502d-640b-4d78-af5d-4ff6a93acc2c"), "India" },
                    { new Guid("4d01475e-2525-4c94-960c-f4595c3ee212"), "UK" },
                    { new Guid("8b47d7f5-aac7-48a5-9321-099e3e9cb322"), "Canada" },
                    { new Guid("d761edb1-ba5c-4220-9065-f131048e2f85"), "USA" }
                });

            migrationBuilder.InsertData(
                table: "Persons",
                columns: new[] { "PersonID", "Address", "CountryID", "DateOfBirth", "Email", "Gender", "PersonName", "ReceiveNewsLetters" },
                values: new object[,]
                {
                    { new Guid("0e04232b-e9ec-4721-9909-496cb40a7245"), "a0858 Novick Terrace", new Guid("d761edb1-ba5c-4220-9065-f131048e2f85"), new DateTime(1923, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Aguste@booking.com", "Male", "Aguste", true },
                    { new Guid("35f68dc8-ddf0-4e02-b4d9-9b74195f26ae"), "c0858 Novick Terrace", new Guid("d761edb1-ba5c-4220-9065-f131048e2f85"), new DateTime(1993, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Ramesh@booking.com", "Male", "Ramesh", false },
                    { new Guid("80ca16b9-1b8c-4815-9f5a-ddb5823243b7"), "b0858 Novick Terrace", new Guid("8b47d7f5-aac7-48a5-9321-099e3e9cb322"), new DateTime(1963, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sam@booking.com", "Male", "Sam", false },
                    { new Guid("bb6d1d8d-f849-4cbb-9117-8108d2c2992c"), "d0858 Novick Terrace", new Guid("4d01475e-2525-4c94-960c-f4595c3ee212"), new DateTime(1994, 1, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sundar@booking.com", "Male", "Sundar", true }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "Persons");
        }
    }
}
