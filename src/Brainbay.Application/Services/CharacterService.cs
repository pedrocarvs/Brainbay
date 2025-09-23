using Brainbay.Core.Interfaces;
using Brainbay.Core.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Brainbay.Application;


public class CharacterService : ICharacterService
{
    private readonly ICharacterRepository _repo;
    private readonly IMemoryCache _cache;
    private const string CacheKey = "Characters";

    /// <summary>
    /// Initializes a new instance of <see cref="CharacterService"/>.
    /// </summary>
    /// <param name="repo">The character repository.</param>
    /// <param name="cache">The memory cache instance.</param>
    public CharacterService(ICharacterRepository repo, IMemoryCache cache)
    {
        _repo = repo;
        _cache = cache;
    }

    /// <inheritdoc/>
    public async Task<(List<Character> Characters, bool FromDatabase)> GetAllCharactersAsync()
    {
        if (!_cache.TryGetValue(CacheKey, out List<Character> characters) || characters == null)
        {
            characters = (await _repo.GetAllAsync()).ToList() ?? new List<Character>();

            var options = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

            _cache.Set(CacheKey, characters, options);
            return (characters, true);
        }

        return (characters, false);
    }

    /// <inheritdoc/>
    public async Task AddCharacterAsync(Character character)
    {
        await _repo.AddAsync(character);
        _cache.Remove(CacheKey);
    }

    /// <inheritdoc/>
    public async Task<List<Character>> GetCharactersByPlanetAsync(string planet)
    {
        var characters = (await _repo.GetByPlanetAsync(planet)).ToList() ?? new List<Character>();

        return characters;
    }

}