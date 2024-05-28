using APIPeliculas.Models;
using APIPeliculas.Models.Dtos;
using AutoMapper;

namespace APIPeliculas.PeliculasMappers {
  public class PeliculasMapper : Profile {
    public PeliculasMapper() {
      CreateMap<Categoria, CategoriaDto>().ReverseMap();
      CreateMap<Categoria, CrearCategoriaDto>().ReverseMap();
      CreateMap<Pelicula, PeliculaDto>().ReverseMap();
      CreateMap<Usuario, UsuarioDto>().ReverseMap();
    }
  }
}
