using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfrastructureLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddAvatar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Conversation_DifferentUsers",
                schema: "dbo",
                table: "Conversation");

            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                schema: "dbo",
                table: "UserAccount",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Conversation_DifferentUsers",
                schema: "dbo",
                table: "Conversation",
                sql: "[User1Id] <> [User2Id]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Conversation_DifferentUsers",
                schema: "dbo",
                table: "Conversation");

            migrationBuilder.DropColumn(
                name: "Avatar",
                schema: "dbo",
                table: "UserAccount");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Conversation_DifferentUsers",
                schema: "dbo",
                table: "Conversation",
                sql: "[User1Id] != [User2Id]");
        }
    }
}
