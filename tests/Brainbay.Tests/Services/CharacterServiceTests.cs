using Xunit;
using NSubstitute;
using Brainbay.Core.Interfaces;
using Brainbay.Core.Models;
using Brainbay.Application;
using Microsoft.Extensions.Caching.Memory;
using FluentAssertions;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CharacterServiceTests
{
    private readonly ICharacterRepository _repo = Substitute.For<ICharacterRepository>();
    private readonly IMemoryCache _cache = new MemoryCache(new MemoryCacheOptions());
    private readonly CharacterService _service;

    public CharacterServiceTests()
    {
        _service = new CharacterService(_repo, _cache);
    }

    [Fact]
    public async Task GetAllCharactersAsync_ShouldReturnFromCache_WhenCacheExists()
    {
        // Arrange
        var cachedCharacters = new List<Character> { new Character { Name = "Rick" } };
        _cache.Set("Characters", cachedCharacters);

        // Act
        var (characters, fromDb) = await _service.GetAllCharactersAsync();

        // Assert
        characters.Should().BeEquivalentTo(cachedCharacters);
        fromDb.Should().BeFalse();
        await _repo.DidNotReceive().GetAllAsync(default);
    }

    [Fact]
    public async Task GetAllCharactersAsync_ShouldReturnFromDatabase_WhenCacheEmpty()
    {
        // Arrange
        var dbCharacters = new List<Character> { new Character { Name = "Morty" } };
        _repo.GetAllAsync(default).Returns(Task.FromResult((IReadOnlyList<Character>)dbCharacters));

        // Act
        var (characters, fromDb) = await _service.GetAllCharactersAsync();

        // Assert
        characters.Should().BeEquivalentTo(dbCharacters);
        fromDb.Should().BeTrue();
        await _repo.Received(1).GetAllAsync(default);
    }

    [Fact]
    public async Task AddCharacterAsync_ShouldAddCharacterAndClearCache()
    {
        // Arrange
        var character = new Character { Name = "Summer" };
        _cache.Set("Characters", new List<Character> { character });

        // Act
        await _service.AddCharacterAsync(character);

        // Assert
        await _repo.Received(1).AddAsync(character, default);
        _cache.TryGetValue("Characters", out _).Should().BeFalse();
    }

    [Fact]
    public async Task GetCharactersByPlanetAsync_ShouldReturnCharactersFromRepo()
    {
        // Arrange
        var planet = "Earth";
        var characters = new List<Character> { new Character { Name = "Beth", Origin = "Earth" } };
        _repo.GetByPlanetAsync(planet, default).Returns(Task.FromResult((IReadOnlyList<Character>)characters));

        // Act
        var result = await _service.GetCharactersByPlanetAsync(planet);

        // Assert
        result.Should().BeEquivalentTo(characters);
        await _repo.Received(1).GetByPlanetAsync(planet, default);
    }
}