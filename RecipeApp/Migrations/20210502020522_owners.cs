using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RecipeApp.Migrations
{
    public partial class owners : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "id",
                table: "user_mock");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "user_mock");

            migrationBuilder.DropColumn(
                name: "created_by_id",
                table: "recipes");

            migrationBuilder.AddColumn<string>(
                name: "id",
                table: "user_mock",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "recipes",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "users_id",
                table: "user_mock",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_recipes_OwnerId",
                table: "recipes",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_recipes_user_mock_OwnerId",
                table: "recipes",
                column: "OwnerId",
                principalTable: "user_mock",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_recipes_user_mock_OwnerId",
                table: "recipes");

            migrationBuilder.DropPrimaryKey(
                name: "users_id",
                table: "user_mock");

            migrationBuilder.DropIndex(
                name: "IX_recipes_OwnerId",
                table: "recipes");

            migrationBuilder.DropColumn(
                name: "id",
                table: "user_mock");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "recipes");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "user_mock",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<string>(
                name: "created_by_id",
                table: "recipes",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "id",
                table: "user_mock",
                column: "UserId");
        }
    }
}
