using Xunit;
using Microsoft.EntityFrameworkCore;
using Brainbay.Infrastructure.Data;
using Brainbay.Infrastructure.Repositories;
using Brainbay.Core.Models;

public class CharacterRepositoryIntegrationTests
{
    private ApplicationDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task AddAndGetCharacter_WorksCorrectly()
    {
        using var db = CreateDbContext();
        var repo = new CharacterRepository(db);

        var character = new Character { Name = "Rick", Origin = "Earth" };
        await repo.AddAsync(character);

        var characters = await repo.GetAllAsync();

        Assert.Single(characters);
        Assert.Equal("Rick", characters.First().Name);
    }
}