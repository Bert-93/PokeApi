using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// A�adir servicios al contenedor

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuraci�n de HttpClient con BaseAddress desde appsettings.json
builder.Services.AddHttpClient<PokeApi.Service.PokeService>(client =>
{
    var baseUrl = builder.Configuration["PokeApi:BaseUrl"];
    if (string.IsNullOrWhiteSpace(baseUrl))
    {
        throw new InvalidOperationException("La configuraci�n 'PokeApi:BaseUrl' no est� definida o es inv�lida.");
    }
    client.BaseAddress = new Uri(baseUrl);
});

var app = builder.Build();

// Configuraci�n de respuesta HTTP pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// HTTPS redirection (opcional, pero recomendado)
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();