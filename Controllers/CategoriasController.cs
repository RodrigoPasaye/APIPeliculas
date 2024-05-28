using APIPeliculas.Models;
using APIPeliculas.Models.Dtos;
using APIPeliculas.Repository.IRepository;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace APIPeliculas.Controllers {

  [ApiController]
  [Route("api/[controller]")]
  public class CategoriasController : ControllerBase {

    private readonly ICategoriaRepository _categoriasRepository;
    private readonly IMapper _mapper;

    public CategoriasController(ICategoriaRepository categoriasRepository, IMapper mapper) {
      _categoriasRepository = categoriasRepository;
      _mapper = mapper;
    }

    [AllowAnonymous]
    [HttpGet]
    [ResponseCache(Duration = 20)]
    //Sirve para no guardar los errores y que cuando se guarde la cache no este mandando los errores en caso de un error en la peticion
    //[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public IActionResult GetCategorias() {

      var listCategorias = _categoriasRepository.GetCategorias();

      var listCategoriasDto = new List<CategoriaDto>();

      foreach (var item in listCategorias) {
        listCategoriasDto.Add(_mapper.Map<CategoriaDto>(item));
      }

      return Ok(listCategoriasDto);
    }

    [AllowAnonymous]
    [HttpGet("{Id:int}", Name = "GetCategoria")]
    //[ResponseCache(Duration = 30)]
    //Usando la cache global
    [ResponseCache(CacheProfileName = "20Segundos")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetCategoria(int Id) {

      var categoria = _categoriasRepository.GetCategoria(Id);

      if (categoria == null) {
        return NotFound();
      }

      var categoriaDto = _mapper.Map<CategoriaDto>(categoria);

      return Ok(categoriaDto);
    }

    [Authorize(Roles = "Administrador")]
    [HttpPost]
    [ProducesResponseType(201, Type = typeof(CategoriaDto))]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public IActionResult CrearCategoria([FromBody] CrearCategoriaDto categoriaDto) {

      if (!ModelState.IsValid) { return BadRequest(ModelState); }

      if (categoriaDto == null) {
        return BadRequest(ModelState);
      }

      if (_categoriasRepository.ExisteCategoria(categoriaDto.Nombre)) {
        ModelState.AddModelError("", "La categoría ya existe");
        return StatusCode(404, ModelState);
      }

      var categoria = _mapper.Map<Categoria>(categoriaDto);

      if (!_categoriasRepository.CrearCategoria(categoria)) {
        ModelState.AddModelError("", $"Algo salió mal al guardar el registro {categoria.Nombre}");
        return StatusCode(500, ModelState);
      }

      return CreatedAtRoute("GetCategoria", new { categoria.Id }, categoria);
    }

    [Authorize(Roles = "Administrador")]
    [HttpPatch("{Id:int}", Name = "ActualizarCategoria")]
    [ProducesResponseType(201, Type = typeof(CategoriaDto))]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult ActualizarCategoria(int Id, [FromBody] CategoriaDto categoriaDto) {

      if (!ModelState.IsValid) { return BadRequest(ModelState); }

      if (categoriaDto == null || Id != categoriaDto.Id) {
        return BadRequest(ModelState);
      }      

      var categoria = _mapper.Map<Categoria>(categoriaDto);

      if (!_categoriasRepository.ActualizarCategoria(categoria)) {
        ModelState.AddModelError("", $"Algo salió mal al actualizar el registro {categoria.Nombre}");
        return StatusCode(500, ModelState);
      }

      return NoContent();
    }

    [Authorize(Roles = "Administrador")]
    [HttpDelete("{Id:int}", Name = "BorrarCategoria")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult BorrarCategoria(int Id) {


      if (!_categoriasRepository.ExisteCategoria(Id)) {
        return NotFound();
      }

      var categoria = _categoriasRepository.GetCategoria(Id);

      if (!_categoriasRepository.BorrarCategoria(categoria)) {
        ModelState.AddModelError("", $"Algo salió mal al borrar el registro {categoria.Nombre}");
        return StatusCode(500, ModelState);
      }

      return NoContent();
    }

  }
}
