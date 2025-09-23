using Brainbay.Core.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Brainbay.Core.Interfaces;

/// <summary>
/// Repository interface for performing CRUD operations on <see cref="Character"/> entity.
/// </summary>
public interface ICharacterRepository
{
    /// <summary>
    /// Clears all characters from the repository.
    /// </summary>
    /// <param name="ct">Optional cancellation token.</param>
    Task ClearAsync(CancellationToken ct = default);

    /// <summary>
    /// Adds multiple characters to the repository.
    /// </summary>
    /// <param name="characters">The characters to add.</param>
    /// <param name="ct">Optional cancellation token.</param>
    Task AddRangeAsync(IEnumerable<Character> characters, CancellationToken ct = default);

    /// <summary>
    /// Retrieves all characters from the repository.
    /// </summary>
    /// <param name="ct">Optional cancellation token.</param>
    /// <returns>A read-only list of all characters.</returns>
    Task<IReadOnlyList<Character>> GetAllAsync(CancellationToken ct = default);

    /// <summary>
    /// Adds a single character to the repository.
    /// </summary>
    /// <param name="character">The character to add.</param>
    /// <param name="ct">Optional cancellation token.</param>
    Task AddAsync(Character character, CancellationToken ct = default);

    /// <summary>
    /// Counts the total number of characters in the repository.
    /// </summary>
    /// <param name="ct">Optional cancellation token.</param>
    /// <returns>The total count of characters.</returns>
    Task<int> CountAsync(CancellationToken ct = default);

    /// <summary>
    /// Retrieves characters filtered by their planet of origin.
    /// </summary>
    /// <param name="planet">The name of the planet.</param>
    /// <param name="ct">Optional cancellation token.</param>
    /// <returns>A read-only list of characters from the specified planet.</returns>
    Task<IReadOnlyList<Character>> GetByPlanetAsync(string planet, CancellationToken ct = default);
}