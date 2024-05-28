using System.ComponentModel.DataAnnotations;

namespace APIPeliculas.Models.Dtos {
  public class CategoriaDto {
    public int Id { get; set; }
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [MaxLength(100, ErrorMessage = "El número máximo de caracteres es de 100!")]
    public required string Nombre { get; set; }
  }
}
