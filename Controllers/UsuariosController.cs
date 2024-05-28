using APIPeliculas.Models;
using APIPeliculas.Models.Dtos;
using APIPeliculas.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace APIPeliculas.Controllers {

  [Route("api/[controller]")]
  [ApiController]
  public class UsuariosController : ControllerBase {

    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IMapper _mapper;
    protected RespuestaAPI _respuestaAPI;

    public UsuariosController(IUsuarioRepository usuarioRepository, IMapper mapper) {
      _usuarioRepository = usuarioRepository;
      _mapper = mapper;
      _respuestaAPI = new();
    }

    [Authorize(Roles = "Administrador")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult GetUsuarios() {

      var listUsuarios = _usuarioRepository.GetUsuarios();

      var listUsuariosDto = new List<UsuarioDto>();

      foreach (var item in listUsuarios) {
        listUsuariosDto.Add(_mapper.Map<UsuarioDto>(item));
      }

      return Ok(listUsuariosDto);
    }

    [Authorize(Roles = "Administrador")]
    [HttpGet("{Id:int}", Name = "GetUsuario")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetUsuario(int Id) {

      var usuario = _usuarioRepository.GetUsuario(Id);

      if (usuario == null) {
        return NotFound();
      }

      var usuarioDto = _mapper.Map<UsuarioDto>(usuario);

      return Ok(usuarioDto);
    }

    [AllowAnonymous]
    [HttpPost("Registro")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public async Task<IActionResult> Registro([FromBody] UsuarioRegistroDto usuarioRegistroDto) {

      bool isUniqueUser = _usuarioRepository.IsUniqueUser(usuarioRegistroDto.NombreUsuario);

      if (!isUniqueUser) {
        _respuestaAPI.StatusCode = HttpStatusCode.BadRequest;
        _respuestaAPI.IsSuccess = false;
        _respuestaAPI.ErrorMessages.Add("El nombre de usuario ya existe");
        return BadRequest(_respuestaAPI);
      }

      var usuario = await _usuarioRepository.Registro(usuarioRegistroDto);

      if (usuario == null) {
        _respuestaAPI.StatusCode = HttpStatusCode.BadRequest;
        _respuestaAPI.IsSuccess = false;
        _respuestaAPI.ErrorMessages.Add("Error al momento de registrar el usuario");
        return BadRequest(_respuestaAPI);
      }

      _respuestaAPI.StatusCode= HttpStatusCode.OK;
      _respuestaAPI.IsSuccess = true;

      return Ok(_respuestaAPI);
    }

    [AllowAnonymous]
    [HttpPost("Login")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public async Task<IActionResult> Login([FromBody] UsuarioLoginDto usuarioLoginDto) {

      var respuestaLogin = await _usuarioRepository.Login(usuarioLoginDto);

      if (respuestaLogin.Usuario == null || string.IsNullOrEmpty(respuestaLogin.Token)) {
        _respuestaAPI.StatusCode = HttpStatusCode.BadRequest;
        _respuestaAPI.IsSuccess = false;
        _respuestaAPI.ErrorMessages.Add("El nombre de usuario o contraseña es incorrecto");
        return BadRequest(_respuestaAPI);
      }      

      _respuestaAPI.StatusCode = HttpStatusCode.OK;
      _respuestaAPI.IsSuccess = true;
      _respuestaAPI.Result = respuestaLogin;

      return Ok(_respuestaAPI);
    }
  }
}
