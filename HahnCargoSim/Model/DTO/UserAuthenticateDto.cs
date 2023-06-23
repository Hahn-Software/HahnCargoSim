using System.ComponentModel.DataAnnotations;

namespace HahnCargoSim.Model.DTO
{
  public class UserAuthenticateDto
  {
    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }
  }
}
