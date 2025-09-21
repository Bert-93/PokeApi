using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using PokeApi.Models;

namespace PokeApi.Service;

public class PokeService
{
    private readonly HttpClient _httpClient;
    private readonly string? _baseUrl;

    public PokeService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _baseUrl = configuration["PokeApi:BaseUrl"];
    }

    public async Task<PokeName> GetPokeAsync(string name)
    {
        var url = $"{_baseUrl}/pokemon/{name.ToLower()}";
        Console.WriteLine($"Fetching data from URL: {url}");

        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Error fetching data from PokeAPI");
        }

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        var root = doc.RootElement;

        // Para devolver un listado de habilidades
        var abilities = root.GetProperty("abilities")
            .EnumerateArray()
            .Select(a => a.GetProperty("ability").GetProperty("name").GetString() ?? "")
            .ToList();

        return new PokeName
        {
            name = root.GetProperty("name").GetString(),
            weight = root.GetProperty("weight").GetInt32(),
            abilities = abilities,
            base_experience = root.GetProperty("base_experience").GetInt32()
        };
    }

    public async Task<PokeSpecie> GetPokeSpecieAsync(string pokemon) 
    {
        var url = $"{_baseUrl}/pokemon-species/{pokemon.ToLower()}";
        Console.WriteLine($"Fetching data from URL: {url}");

        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Error fetching data from PokeAPI");
        }

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        var root = doc.RootElement;

        // description y habitat pueden no ser valores NULL, por lo que tendremos que traerlos de forma segura

        // Manejar description de forma segura
        string? description = root.GetProperty("flavor_text_entries")
            .EnumerateArray()
            .FirstOrDefault(e =>
                e.TryGetProperty("language", out var lang) &&
                lang.GetProperty("name").GetString() == "en")
            .GetProperty("flavor_text")
            .GetString();

        // Manejar habitat de forma segura
        string? habitat = root.TryGetProperty("habitat", out var habitatElement) &&
                          habitatElement.ValueKind == JsonValueKind.Object &&
                          habitatElement.TryGetProperty("name", out var habitatName)
            ? habitatName.GetString()
            : "unknown";

        return new PokeSpecie
        {
            base_happiness = root.GetProperty("base_happiness").GetInt32(),
            capture_rate = root.GetProperty("capture_rate").GetInt32(),
            description = description ?? "No description avaible",
            habitat = habitat
        };

    }
}