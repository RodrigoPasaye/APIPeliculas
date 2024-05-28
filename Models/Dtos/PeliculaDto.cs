using System.ComponentModel.DataAnnotations;

namespace APIPeliculas.Models.Dtos {
  public class PeliculaDto {
    public int Id { get; set; }
    [Required(ErrorMessage = "El nombre es obligatorio")]
    public required string Nombre { get; set; }
    public string RutaImagen { get; set; }
    [Required(ErrorMessage = "La descripción es obligatoria")]
    public required string Descripcion { get; set; }
    [Required(ErrorMessage = "La duración es obligatoria")]
    public int Duracion { get; set; }
    public enum TipoClasificacion { Siete, Trece, Dieciseis, Dieciocho }
    public TipoClasificacion Clasificacion { get; set; }
    public DateTime FechaCreacion { get; set; }
    public int CategoriaId { get; set; }
  }
}
