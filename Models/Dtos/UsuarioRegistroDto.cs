using System.ComponentModel.DataAnnotations;

namespace APIPeliculas.Models.Dtos {
  public class UsuarioRegistroDto {
    [Required(ErrorMessage = "El usuario es obligatorio")]
    public required string NombreUsuario { get; set; }
    [Required(ErrorMessage = "El nombre es obligatorio")]
    public required string Nombre { get; set; }
    [Required(ErrorMessage = "El password es obligatorio")]
    public required string Password { get; set; }
    [Required(ErrorMessage = "El rol es obligatorio")]
    public required string Rol { get; set; }
  }
}
