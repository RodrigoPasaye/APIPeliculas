using APIPeliculas.Data;
using APIPeliculas.Models;
using APIPeliculas.Models.Dtos;
using APIPeliculas.Repository.IRepository;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using XSystem.Security.Cryptography;

namespace APIPeliculas.Repository {
  public class UsuarioRepository : IUsuarioRepository {

    private readonly ApplicationDbContext _context;
    private string claveSecret;

    public UsuarioRepository(ApplicationDbContext context, IConfiguration configuration) {
      _context = context;
      claveSecret = configuration.GetValue<string>("ApiSettings:Secret");
    }

    public ICollection<Usuario> GetUsuarios() {
      return _context.Usuarios.OrderBy(u => u.NombreUsuario).ToList();
    }

    public Usuario GetUsuario(int id) {
      return _context.Usuarios.FirstOrDefault(u => u.Id == id);
    }

    public bool IsUniqueUser(string usuario) {
      var usuarioBd = _context.Usuarios.FirstOrDefault(u => u.NombreUsuario == usuario);

      if (usuarioBd == null) {
        return true;
      }
      return false;
    }

    public async Task<UsuarioLoginRequestDto> Login(UsuarioLoginDto usuarioLoginDto) {
      var passwordEncriptado = obtenermd5(usuarioLoginDto.Password);

      var usuario = _context.Usuarios.FirstOrDefault(
        u => u.NombreUsuario.ToLower() == usuarioLoginDto.NombreUsuario.ToLower()
        && u.Password == passwordEncriptado);

      if (usuario == null) {
        return new UsuarioLoginRequestDto() {
          Token = "",
          Usuario = null,
        };
      }

      var manejadorToken = new JwtSecurityTokenHandler();
      var key = Encoding.ASCII.GetBytes(claveSecret);

      var tokenDescriptor = new SecurityTokenDescriptor {
        Subject = new ClaimsIdentity(new Claim[] {
          new Claim(ClaimTypes.Name, usuario.NombreUsuario),
          new Claim(ClaimTypes.Role, usuario.Rol)
        }),
        Expires = DateTime.UtcNow.AddDays(1),
        SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
      };

      var token = manejadorToken.CreateToken(tokenDescriptor);

      UsuarioLoginRequestDto loginRequestDto = new() {
        Token = manejadorToken.WriteToken(token),
        Usuario = usuario,
      };

      return loginRequestDto;
    }

    public async Task<Usuario> Registro(UsuarioRegistroDto usuarioRegistroDto) {
      var passwordEncriptado = obtenermd5(usuarioRegistroDto.Password);

      Usuario usuario = new() {
        Nombre = usuarioRegistroDto.Nombre,
        NombreUsuario = usuarioRegistroDto.NombreUsuario,
        Password = passwordEncriptado,
        Rol = usuarioRegistroDto.Rol,
      };

      _context.Usuarios.Add(usuario);
      await _context.SaveChangesAsync();
      usuario.Password = passwordEncriptado;
      return usuario;
    }

    //Método para encriptar contraseña con MD5 se usa tanto en el Acceso como en el Registro
    public static string obtenermd5(string valor) {
      MD5CryptoServiceProvider x = new MD5CryptoServiceProvider();
      byte[] data = System.Text.Encoding.UTF8.GetBytes(valor);
      data = x.ComputeHash(data);
      string resp = "";
      for (int i = 0; i < data.Length; i++)
        resp += data[i].ToString("x2").ToLower();
      return resp;
    }
  }
}
