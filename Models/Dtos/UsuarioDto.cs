namespace APIPeliculas.Models.Dtos {
  public class UsuarioDto {
    public int Id { get; set; }
    public required string NombreUsuario { get; set; }
    public required string Nombre { get; set; }
    public required string Password { get; set; }
    public required string Rol { get; set; }
  }
}
