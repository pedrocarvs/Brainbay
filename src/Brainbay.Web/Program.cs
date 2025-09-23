using Brainbay.Application;
using Brainbay.Core.Interfaces;
using Brainbay.Infrastructure.Data;
using Brainbay.Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddControllersWithViews();
builder.Services.AddMemoryCache();

builder.Services.AddScoped<ICharacterRepository, CharacterRepository>();
builder.Services.AddScoped<ICharacterService, CharacterService>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.MapControllerRoute(name: "default", pattern: "{controller=Characters}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.EnsureCreatedAsync();

    var count = await db.Characters.CountAsync();
    Console.WriteLine($"Characters in DB: {count}");

    var all = await db.Characters.ToListAsync();
    foreach (var c in all)
        Console.WriteLine($"{c.Id} - {c.Name} - {c.Origin}");
}

app.Run();