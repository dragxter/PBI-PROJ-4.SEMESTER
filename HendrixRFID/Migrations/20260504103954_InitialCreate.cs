using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HendrixRFID.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Lamps",
                columns: table => new
                {
                    LampId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lamps", x => x.LampId);
                });

            migrationBuilder.CreateTable(
                name: "Pigs",
                columns: table => new
                {
                    PigId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pigs", x => x.PigId);
                });

            migrationBuilder.CreateTable(
                name: "RawScans",
                columns: table => new
                {
                    ScanId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PigId = table.Column<string>(type: "TEXT", nullable: false),
                    LampId = table.Column<string>(type: "TEXT", nullable: false),
                    SignalStrength = table.Column<int>(type: "INTEGER", nullable: false),
                    ScanTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RawScans", x => x.ScanId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Lamps");

            migrationBuilder.DropTable(
                name: "Pigs");

            migrationBuilder.DropTable(
                name: "RawScans");
        }
    }
}
