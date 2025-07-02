using System.ComponentModel.DataAnnotations;

namespace TicTacToe.Datasource.Model
{
    public class UserDTO
    {
        [Key] public string Uuid { get; set; } = "";
        [Required] public string Login { get; set; } = "";
        [Required] public string Password { get; set; } = "";
    }
}
