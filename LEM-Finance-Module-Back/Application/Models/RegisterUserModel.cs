using System.ComponentModel.DataAnnotations;

public class RegisterUserModel
{
    [Required]
    [StringLength(100)]
    public string Username { get; set; }

    [Required]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    public bool IsAdmin { get; set; } = false;
}