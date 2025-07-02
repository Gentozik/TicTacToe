using System.Text.Json.Serialization;
using TicTacToe.Domain.Model;

namespace TicTacToe.Datasource.Model
{
    public class GameDTO
    {
        [JsonPropertyName("id")]
        public Guid Id { get; init; } = new Guid();

        [JsonPropertyName("gameBoard")]
        public GameBoardDTO GameBoard { get; init; } = new GameBoardDTO();

        [JsonPropertyName("gameOutcome")]
        public GameOutcome GameOutcome { get; set; } = GameOutcome.None;

        [JsonPropertyName("playerXId")]
        public Guid PlayerXId { get; init; }

        [JsonPropertyName("playerOId")]
        public Guid? PlayerOId { get; set; }
    }
}
