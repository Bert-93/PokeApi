namespace PokeApi.Models;

public class PokeName
{
    public string? name { get; set; }
    public int weight { get; set; }
    public List<string>? abilities { get; set; }
    public int base_experience { get; set; }
}