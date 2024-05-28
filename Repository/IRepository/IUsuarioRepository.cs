using APIPeliculas.Models;
using APIPeliculas.Models.Dtos;

namespace APIPeliculas.Repository.IRepository {
  public interface IUsuarioRepository {
    ICollection<Usuario> GetUsuarios();
    Usuario GetUsuario(int id);
    bool IsUniqueUser(string usuario);
    Task<UsuarioLoginRequestDto> Login(UsuarioLoginDto usuarioLoginDto);
    Task<Usuario> Registro(UsuarioRegistroDto usuarioRegistroDto);
  }
}
