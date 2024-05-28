using System.ComponentModel.DataAnnotations;

namespace APIPeliculas.Models {
  public class Categoria {
    [Key]
    public int Id { get; set; }
    [Required]
    public required string Nombre { get; set; }
    public DateTime FechaCreacion { get; set; }
  }
}
