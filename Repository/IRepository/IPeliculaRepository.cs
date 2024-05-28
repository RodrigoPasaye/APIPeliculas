using APIPeliculas.Models;

namespace APIPeliculas.Repository.IRepository {
  public interface IPeliculaRepository {
    ICollection<Pelicula> GetPeliculas();
    Pelicula GetPelicula(int id);
    bool ExistePelicula(string nombre);
    bool ExistePelicula(int id);
    bool CrearPelicula(Pelicula pelicula);
    bool ActualizarPelicula(Pelicula pelicula);
    bool BorrarPelicula(Pelicula pelicula);
    ICollection<Pelicula> GetPeliculasPorCategoria(int categoriaId);
    ICollection<Pelicula> BuscarPelicula(string nombre);
    bool Guardar();
  }
}
