using System.ComponentModel.DataAnnotations;

namespace _8ball_arena.Models
{
    public class CreateDuelViewModel
    {
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; }
    }
}
