using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wallet.Migrations
{
    /// <inheritdoc />
    public partial class PositionUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "POSITION",
                newName: "Amount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "POSITION",
                newName: "Quantity");
        }
    }
}
