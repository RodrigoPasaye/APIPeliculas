using APIPeliculas.Models;
using APIPeliculas.Models.Dtos;
using APIPeliculas.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APIPeliculas.Controllers {

  [Route("api/[controller]")]
  [ApiController]
  public class PeliculasController : ControllerBase {

    private readonly IPeliculaRepository _peliculaRepository;
    private readonly IMapper _mapper;

    public PeliculasController(IPeliculaRepository peliculaRepository, IMapper mapper) {
      _peliculaRepository = peliculaRepository;
      _mapper = mapper;
    }

    [AllowAnonymous]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult GetPeliculas() {

      var listaPeliculas = _peliculaRepository.GetPeliculas();

      var listPeliculasDto = new List<PeliculaDto>();

      foreach (var item in listaPeliculas) {
        listPeliculasDto.Add(_mapper.Map<PeliculaDto>(item));
      }

      return Ok(listPeliculasDto);
    }

    [AllowAnonymous]
    [HttpGet("{Id:int}", Name = "GetPelicula")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetPelicula(int Id) {

      var pelicula = _peliculaRepository.GetPelicula(Id);

      if (pelicula == null) {
        return NotFound();
      }

      var peliculaDto = _mapper.Map<PeliculaDto>(pelicula);

      return Ok(peliculaDto);
    }

    [Authorize(Roles = "Administrador")]
    [HttpPost]
    [ProducesResponseType(201, Type = typeof(PeliculaDto))]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public IActionResult CrearPelicula([FromBody] PeliculaDto peliculaDto) {

      if (!ModelState.IsValid) { return BadRequest(ModelState); }

      if (peliculaDto == null) {
        return BadRequest(ModelState);
      }

      if (_peliculaRepository.ExistePelicula(peliculaDto.Nombre)) {
        ModelState.AddModelError("", "La pelicula ya existe");
        return StatusCode(404, ModelState);
      }

      var pelicula = _mapper.Map<Pelicula>(peliculaDto);

      if (!_peliculaRepository.CrearPelicula(pelicula)) {
        ModelState.AddModelError("", $"Algo salió mal al guardar el registro {pelicula.Nombre}");
        return StatusCode(500, ModelState);
      }

      return CreatedAtRoute("GetPelicula", new { pelicula.Id }, pelicula);
    }

    [Authorize(Roles = "Administrador")]
    [HttpPatch("{Id:int}", Name = "ActualizarPelicula")]
    [ProducesResponseType(204)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public IActionResult ActualizarPelicula(int Id, [FromBody] PeliculaDto peliculaDto) {

      if (!ModelState.IsValid) { return BadRequest(ModelState); }

      var pelicula = _mapper.Map<Pelicula>(peliculaDto);

      if (!_peliculaRepository.ActualizarPelicula(pelicula)) {
        ModelState.AddModelError("", $"Algo salió mal al actualizar el registro {pelicula.Nombre}");
        return StatusCode(500, ModelState);
      }

      return NoContent();
    }

    [Authorize(Roles = "Administrador")]
    [HttpDelete("{Id:int}", Name = "BorrarPelicula")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult BorrarPelicula(int Id) {


      if (!_peliculaRepository.ExistePelicula(Id)) {
        return NotFound();
      }

      var pelicula = _peliculaRepository.GetPelicula(Id);

      if (!_peliculaRepository.BorrarPelicula(pelicula)) {
        ModelState.AddModelError("", $"Algo salió mal al borrar el registro {pelicula.Nombre}");
        return StatusCode(500, ModelState);
      }

      return NoContent();
    }

    [AllowAnonymous]
    [HttpGet("GetPeliculasPorCategoria/{Id:int}")]
    public IActionResult GetPeliculasPorCategoria(int Id) {

      var listPeliculas = _peliculaRepository.GetPeliculasPorCategoria(Id);

      if (listPeliculas == null) {
        return NotFound();
      }

      var listPeliculasDto = new List<PeliculaDto>();

      foreach (var item in listPeliculas) { 
        listPeliculasDto.Add(_mapper.Map<PeliculaDto>(item)); 
      }

      return Ok(listPeliculasDto);
    }

    [AllowAnonymous]
    [HttpGet("BuscarPelicula")]
    public IActionResult BuscarPelicula(string nombre) {
      try {
        var peliculas = _peliculaRepository.BuscarPelicula(nombre.Trim());
        if (peliculas.Any()) return Ok(peliculas);
        return NotFound();
      } catch (Exception) {
        return StatusCode(StatusCodes.Status500InternalServerError, "Error al recuperar la información");
      }
    }

  }
}
