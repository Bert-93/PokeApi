using Microsoft.AspNetCore.Mvc;
using PokeApi.Models;
using PokeApi.Service;

namespace PokeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PokemonController : ControllerBase
{
    private readonly PokeService _pokeService;
    public PokemonController(PokeService pokeService)
    {
        _pokeService = pokeService;
    }

    [HttpGet("{name}")]
    public async Task<IActionResult> GetPoke(string name)
    {
        try
        {
            var pokemon = await _pokeService.GetPokeAsync(name);
            return Ok(pokemon);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("species/{pokemon}")]
    public async Task<IActionResult> GetPokeSpecie(string pokemon)
    {
        try
        {
            var specie = await _pokeService.GetPokeSpecieAsync(pokemon);
            return Ok(specie);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}