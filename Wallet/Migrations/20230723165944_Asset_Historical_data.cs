using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wallet.Migrations
{
    /// <inheritdoc />
    public partial class Asset_Historical_data : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ASSET_HISTORICAL_DATA",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    AssetId = table.Column<string>(type: "text", nullable: true),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Open = table.Column<double>(type: "double precision", nullable: false),
                    High = table.Column<double>(type: "double precision", nullable: false),
                    Low = table.Column<double>(type: "double precision", nullable: false),
                    Close = table.Column<double>(type: "double precision", nullable: false),
                    AdjustedClose = table.Column<double>(type: "double precision", nullable: false),
                    Volume = table.Column<long>(type: "bigint", nullable: false),
                    DividendAmount = table.Column<double>(type: "double precision", nullable: false),
                    SplitCoefficient = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ASSET_HISTORICAL_DATA", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ASSET_HISTORICAL_DATA_ASSET_AssetId",
                        column: x => x.AssetId,
                        principalTable: "ASSET",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ASSET_HISTORICAL_DATA_AssetId",
                table: "ASSET_HISTORICAL_DATA",
                column: "AssetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ASSET_HISTORICAL_DATA");
        }
    }
}
