using Brainbay.Core.Interfaces;
using Brainbay.Core.Models;
using Brainbay.Infrastructure.Data;
using Brainbay.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        string FindSolutionRoot(string startDir)
        {
            var dir = new DirectoryInfo(startDir);
            while (dir != null && !File.Exists(Path.Combine(dir.FullName, "Brainbay.sln")))
            {
                dir = dir.Parent;
            }
            if (dir == null) throw new Exception("Solution root not found!");
            return dir.FullName;
        }

        // Começa na pasta do binário
        var binDir = AppContext.BaseDirectory;
        var solutionRoot = FindSolutionRoot(binDir);

        // Pasta Database na raiz da solução
        var databaseDir = Path.Combine(solutionRoot, "Database");

        if (!Directory.Exists(databaseDir))
            Directory.CreateDirectory(databaseDir);

        var dbPath = Path.Combine(databaseDir, "brainbay.db");

        services.AddDbContext<ApplicationDbContext>(opt => opt.UseSqlite($"Data Source={dbPath}"));
        services.AddScoped<ICharacterRepository, CharacterRepository>();
        services.AddHttpClient("rick", client => client.BaseAddress = new Uri("https://rickandmortyapi.com/api/"));
    })
    .Build();

using var scope = builder.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
await db.Database.EnsureCreatedAsync();

var repo = scope.ServiceProvider.GetRequiredService<ICharacterRepository>();
var httpFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
var client = httpFactory.CreateClient("rick");

Console.WriteLine("Fetching characters from Rick and Morty API...");

int page = 1;
var aliveCharacters = new List<Character>();

while (true)
{
    var resp = await client.GetFromJsonAsync<JsonElement>($"character?page={page}");
    if (resp.ValueKind == JsonValueKind.Undefined) break;

    if (!resp.TryGetProperty("results", out var results)) break;

    foreach (var item in results.EnumerateArray())
    {
        var status = item.GetProperty("status").GetString() ?? "";
        if (!status.Equals("Alive", StringComparison.OrdinalIgnoreCase)) continue;

        var character = new Character
        {
            Id = item.GetProperty("id").GetInt32(),
            Name = item.GetProperty("name").GetString() ?? string.Empty,
            Status = status,
            Species = item.GetProperty("species").GetString() ?? string.Empty,
            Type = item.GetProperty("type").GetString() ?? string.Empty,
            Gender = item.GetProperty("gender").GetString() ?? string.Empty,
            Origin = item.GetProperty("origin").GetProperty("name").GetString() ?? string.Empty,
            Location = item.GetProperty("location").GetProperty("name").GetString() ?? string.Empty,
            Image = item.GetProperty("image").GetString() ?? string.Empty
        };

        aliveCharacters.Add(character);
    }

    if (!resp.TryGetProperty("info", out var info)) break;
    var hasNext = info.TryGetProperty("next", out var next) && next.ValueKind == JsonValueKind.String && !string.IsNullOrEmpty(next.GetString());
    if (!hasNext) break;
    page++;
}

Console.WriteLine($"Found {aliveCharacters.Count} alive characters. Clearing database and saving...");
await repo.ClearAsync();
await repo.AddRangeAsync(aliveCharacters);

var count = await db.Characters.CountAsync();
Console.WriteLine($"Characters in DB: {count}");

Console.WriteLine("Done.");