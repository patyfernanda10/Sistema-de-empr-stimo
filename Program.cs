using Microsoft.EntityFrameworkCore;
using SistemaEmprestimos.Data;

var builder = WebApplication.CreateBuilder(args);

//conexão MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

builder.Services.AddControllers();

// Adiciona suporte a API Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Adiciona o serviço de Autorização
builder.Services.AddAuthorization(); // Esta linha foi adicionada

var app = builder.Build();

// Configuração do pipeline de requisição
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization(); // Esta linha foi adicionada

app.MapControllers();

app.Run();