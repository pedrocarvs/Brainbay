using Brainbay.Core.Models;

namespace Brainbay.Application
{
    /// <summary>
    /// Application service for operations related to Characters.
    /// </summary>
    public interface ICharacterService
    {
        /// <summary>
        /// Retrieves all characters.
        /// </summary>
        /// <returns>
        /// A tuple containing the list of characters and a boolean indicating if the data was fetched from the database.
        /// </returns>
        Task<(List<Character> Characters, bool FromDatabase)> GetAllCharactersAsync();

        /// <summary>
        /// Adds a new character and invalidates the cache.
        /// </summary>
        /// <param name="character">The character to be added.</param>
        Task AddCharacterAsync(Character character);

        /// <summary>
        /// Retrieves characters filtered by their planet of origin.
        /// </summary>
        /// <param name="planet">The name of the planet.</param>
        /// <returns>A list of characters originating from the specified planet.</returns>
        Task<List<Character>> GetCharactersByPlanetAsync(string planet);
    }
}