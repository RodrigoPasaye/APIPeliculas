using APIPeliculas.Data;
using APIPeliculas.Models;
using APIPeliculas.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace APIPeliculas.Repository {
  public class PeliculaRepository : IPeliculaRepository {

    private readonly ApplicationDbContext _context;

    public PeliculaRepository(ApplicationDbContext context) {
      _context = context;
    }

    public bool ActualizarPelicula(Pelicula pelicula) {
      pelicula.FechaCreacion = DateTime.Now;
      _context.Peliculas.Update(pelicula);
      return Guardar();
    }

    public bool BorrarPelicula(Pelicula pelicula) {
      _context.Peliculas.Remove(pelicula);
      return Guardar();
    }    

    public bool CrearPelicula(Pelicula pelicula) {
      pelicula.FechaCreacion = DateTime.Now;
      _context.Peliculas.Add(pelicula);
      return Guardar();
    }

    public bool ExistePelicula(string nombre) {
      bool existe = _context.Peliculas.Any(c => c.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
      return existe;
    }

    public bool ExistePelicula(int id) {
      return _context.Peliculas.Any(c => c.Id == id);
    }

    public Pelicula GetPelicula(int id) {
      return _context.Peliculas.FirstOrDefault(c => c.Id == id);
    }

    public ICollection<Pelicula> GetPeliculas() {
      return _context.Peliculas.OrderBy(c => c.Nombre).ToList();
    }

    public ICollection<Pelicula> GetPeliculasPorCategoria(int categoriaId) {
      return _context.Peliculas.Include(ca => ca.Categoria).Where(ca => ca.CategoriaId == categoriaId).ToList();
    }

    public ICollection<Pelicula> BuscarPelicula(string nombre) {
      IQueryable<Pelicula> query = _context.Peliculas;

      if (!string.IsNullOrEmpty(nombre)) {
        query = query.Where(p => p.Nombre.Contains(nombre) || p.Descripcion.Contains(nombre));
      }

      return query.ToList();
    }

    public bool Guardar() {
      return _context.SaveChanges() >= 0 ? true : false;
    }
  }
}
