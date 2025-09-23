using Brainbay.Core.Interfaces;
using Brainbay.Core.Models;
using Brainbay.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Brainbay.Infrastructure.Repositories;

public class CharacterRepository : ICharacterRepository
{
    private readonly ApplicationDbContext _db;
    public CharacterRepository(ApplicationDbContext db) => _db = db;

    public async Task ClearAsync(CancellationToken ct = default)
    {
        _db.Characters.RemoveRange(_db.Characters);
        await _db.SaveChangesAsync(ct);
    }

    public async Task AddRangeAsync(IEnumerable<Character> characters, CancellationToken ct = default)
    {
        await _db.Characters.AddRangeAsync(characters, ct);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<Character>> GetAllAsync(CancellationToken ct = default)
        => await _db.Characters.AsNoTracking().OrderBy(c => c.Name).ToListAsync(ct);

    public async Task AddAsync(Character character, CancellationToken ct = default)
    {
        _db.Characters.Add(character);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<int> CountAsync(CancellationToken ct = default)
        => await _db.Characters.CountAsync(ct);

    public async Task<IReadOnlyList<Character>> GetByPlanetAsync(string planet, CancellationToken ct = default)
        => await _db.Characters.AsNoTracking()
            .Where(c => c.Origin.ToLower() == planet.ToLower() || c.Location.ToLower() == planet.ToLower())
            .OrderBy(c => c.Name)
            .ToListAsync(ct);
}