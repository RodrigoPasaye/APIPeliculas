using System.ComponentModel.DataAnnotations;

namespace APIPeliculas.Models {
  public class Usuario {
    [Key]
    public int Id { get; set; }
    public required string NombreUsuario { get; set; }
    public required string Nombre { get; set; }
    public required string Password { get; set; }
    public required string Rol { get; set; }
  }
}
