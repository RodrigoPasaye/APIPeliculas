using APIPeliculas.Data;
using APIPeliculas.PeliculasMappers;
using APIPeliculas.Repository;
using APIPeliculas.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

//Configuramos la conexion a SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options => {
  options.UseSqlServer(builder.Configuration.GetConnectionString("ConexionSql"));
});

//Agregamos cache
builder.Services.AddResponseCaching();

//Agregamos los repositorios
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IPeliculaRepository, PeliculaRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

var key = builder.Configuration.GetValue<string>("ApiSettings:Secret");

//Agregamos el AutoMappper
builder.Services.AddAutoMapper(typeof(PeliculasMapper));

//Aquí se configura la Autenticación
builder.Services.AddAuthentication(x => {
  x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x => {
  x.RequireHttpsMetadata = false;
  x.SaveToken = true;
  x.TokenValidationParameters = new TokenValidationParameters {
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
    ValidateIssuer = false,
    ValidateAudience = false,
  };
}) ;

// Add services to the container.

//builder.Services.AddControllers();

//Añadiendo un perfil global para la cache
builder.Services.AddControllers(option => {
  option.CacheProfiles.Add("20Segundos", new CacheProfile() { Duration = 20 });
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => {
  options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme {
    Description = "Autenticación JWT usando el esquema Bearer. \r\n\r\n" +
    "Ingresa la palabra 'Bearer' seguida de un [espacio] y despues su token en el campo de abajo \r\n\r\n" +
    "Ejemplo: \"Bearer tkdofkodn\"",
    Name = "Authorization",
    In = ParameterLocation.Header,
    Scheme = "Bearer"
  });
  options.AddSecurityRequirement(new OpenApiSecurityRequirement() {
    {
      new OpenApiSecurityScheme {
        Reference = new OpenApiReference {
          Type = ReferenceType.SecurityScheme,
          Id = "Bearer"
        },
        Scheme = "oauth2",
        Name = "Bearer",
        In = ParameterLocation.Header
      },
      new List<string>()
    }
  });
});

//Soporte para CORS
//Se pueden habilitar: 1 dominio, 2 o multiples 
//cualquier dominio (tener en cuenta seguridad)
//Usamos de ejemplo el dominio: http://localhost:3223, se debe cambiar por el correcto
//Se usa (*) para todos los dominios
builder.Services.AddCors(p => p.AddPolicy("PolicyCors", build => {
  build.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Se agregan CORS
app.UseCors("PolicyCors");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
