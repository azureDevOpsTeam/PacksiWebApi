using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfrastructureLayer.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDecmialType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "BalanceAfter",
                schema: "dbo",
                table: "WalletTransaction",
                type: "decimal(28,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                schema: "dbo",
                table: "WalletTransaction",
                type: "decimal(28,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Reserved",
                schema: "dbo",
                table: "Wallet",
                type: "decimal(28,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Balance",
                schema: "dbo",
                table: "Wallet",
                type: "decimal(28,6)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,2)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "BalanceAfter",
                schema: "dbo",
                table: "WalletTransaction",
                type: "decimal(28,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Amount",
                schema: "dbo",
                table: "WalletTransaction",
                type: "decimal(28,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Reserved",
                schema: "dbo",
                table: "Wallet",
                type: "decimal(28,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,6)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Balance",
                schema: "dbo",
                table: "Wallet",
                type: "decimal(28,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(28,6)");
        }
    }
}
