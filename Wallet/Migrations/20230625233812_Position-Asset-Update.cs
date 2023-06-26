using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wallet.Migrations
{
    /// <inheritdoc />
    public partial class PositionAssetUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentPrice",
                table: "POSITION");

            migrationBuilder.AddColumn<double>(
                name: "Price",
                table: "ASSET",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "ASSET");

            migrationBuilder.AddColumn<double>(
                name: "CurrentPrice",
                table: "POSITION",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
