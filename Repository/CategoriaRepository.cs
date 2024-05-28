using APIPeliculas.Data;
using APIPeliculas.Models;
using APIPeliculas.Repository.IRepository;

namespace APIPeliculas.Repository {
  public class CategoriaRepository : ICategoriaRepository {

    private readonly ApplicationDbContext _context;

    public CategoriaRepository(ApplicationDbContext context) {
      _context = context;
    }

    public bool ActualizarCategoria(Categoria categoria) {
      categoria.FechaCreacion = DateTime.Now;
      _context.Categorias.Update(categoria);
      return Guardar();
    }

    public bool BorrarCategoria(Categoria categoria) {
      _context.Categorias.Remove(categoria);
      return Guardar();
    }

    public bool CrearCategoria(Categoria categoria) {
      categoria.FechaCreacion = DateTime.Now;
      _context.Categorias.Add(categoria);
      return Guardar();
    }

    public bool ExisteCategoria(string nombre) {
      bool existe = _context.Categorias.Any(c => c.Nombre.ToLower().Trim() == nombre.ToLower().Trim());
      return existe;
    }

    public bool ExisteCategoria(int id) {
      return _context.Categorias.Any(c => c.Id == id);
    }

    public Categoria GetCategoria(int id) {
      return _context.Categorias.FirstOrDefault(c => c.Id == id);
    }

    public ICollection<Categoria> GetCategorias() {
      return _context.Categorias.OrderBy(c => c.Nombre).ToList();
    }

    public bool Guardar() {
      return _context.SaveChanges() >= 0 ? true : false;
    }
  }
}
