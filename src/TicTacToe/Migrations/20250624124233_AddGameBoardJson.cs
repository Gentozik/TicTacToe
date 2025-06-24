using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicTacToe.Migrations
{
    /// <inheritdoc />
    public partial class AddGameBoardJson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GameBoard_BoardMatrix",
                table: "Games");

            migrationBuilder.AddColumn<string>(
                name: "GameBoard_BoardMatrixJson",
                table: "Games",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GameBoard_BoardMatrixJson",
                table: "Games");

            migrationBuilder.AddColumn<int[,]>(
                name: "GameBoard_BoardMatrix",
                table: "Games",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);
        }
    }
}
