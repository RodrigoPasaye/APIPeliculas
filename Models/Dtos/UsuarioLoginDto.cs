using System.ComponentModel.DataAnnotations;

namespace APIPeliculas.Models.Dtos {
  public class UsuarioLoginDto {
    [Required(ErrorMessage = "El usuario es obligatorio")]
    public required string NombreUsuario { get; set; }
    [Required(ErrorMessage = "El password es obligatorio")]
    public required string Password { get; set; }
  }
}
