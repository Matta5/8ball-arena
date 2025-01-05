namespace BLL.DTOs;

public class CreateUserDTO
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string ProfilePicture { get; set; }
    public DateTime DateJoined { get; set; }
}
