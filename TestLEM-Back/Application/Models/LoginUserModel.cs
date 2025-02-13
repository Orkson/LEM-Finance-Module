using System.ComponentModel.DataAnnotations;

public class LoginUserModel
{
    [Required]
    public string Username { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
}