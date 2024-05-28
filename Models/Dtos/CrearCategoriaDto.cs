using System.ComponentModel.DataAnnotations;

namespace APIPeliculas.Models.Dtos {
  public class CrearCategoriaDto {
    [Required(ErrorMessage = "El nombre es obligatorio")]
    [MaxLength(60, ErrorMessage = "El número máximo de caracteres es de 60!")]
    public required string Nombre { get; set; }
  }
}
