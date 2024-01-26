using Microsoft.EntityFrameworkCore;
using PokemonReviewApp.Data;
using PokemonReviewApp.DTO;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Repository
{
    public class PokemonRepository : IPokemonRepository
    {
        private readonly DataContext _context;

        public PokemonRepository(DataContext context) 
        {
            _context = context;
        }

        public bool CreatePokemon(int ownerId, int categoryId, Pokemon pokemon)
        {
            var existedOwner = _context.Owners.Where(o => o.Id == ownerId).FirstOrDefault();
            var existedCategory = _context.Categories.Where(c => c.Id == categoryId).FirstOrDefault();

            if (existedOwner == null || existedCategory == null)
            {
                // Handle case where owner or category doesn't exist
                return false;
            }

            var pokemonOwner = new PokemonOwner()
            {
                Owner = existedOwner,
                Pokemon = pokemon,
            };

            var pokemonCategory = new PokemonCategory()
            {
                Category = existedCategory,
                Pokemon = pokemon,
            };

            // Add PokemonOwner and PokemonCategory to context
            _context.Add(pokemonOwner);
            _context.Add(pokemonCategory);

            // Add Pokemon to context (this is sufficient for tracking)
            _context.Add(pokemon);

            return Save();
        }

        public bool DeletePokemon(Pokemon pokemon)
        {
            _context.Remove(pokemon);

            return Save();
        }

        public Pokemon GetPokemon(int id)
        {
            return _context.Pokemons.Where(p => p.Id == id).FirstOrDefault();
        }

        public Pokemon GetPokemon(string name)
        {
            return _context.Pokemons.Where(p => p.Name == name).FirstOrDefault();
        }

        public decimal GetPokemonRating(int pokemonId)
        {
            var review = _context.Reviews.Where(p => p.Pokemon.Id == pokemonId);

            if (review.Count() <= 0)
                return 0;

            return ((decimal)review.Sum(r => r.Rating) / review.Count());
        }

        public ICollection<Pokemon> GetPokemons()
        {
            return _context.Pokemons.OrderBy(p => p.Id).ToList();
        }

        public Pokemon GetPokemonTrimToUpper(PokemonDTO pokemonCreate)
        {
            return GetPokemons().Where(c => c.Name.Trim().ToUpper() == pokemonCreate.Name.TrimEnd().ToUpper()).FirstOrDefault();
        }

        public bool PokemonExists(int pokemonId)
        {
            return _context.Pokemons.Any(p => p.Id == pokemonId);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();

            return saved > 0;
        }

        public bool UpdatePokemon(int ownerId, int categoryId, Pokemon pokemon)
        {
            var existedOwner = _context.Owners.FirstOrDefault(o => o.Id == ownerId);
            var existedCategory = _context.Categories.FirstOrDefault(c => c.Id == categoryId);

            if (existedOwner == null || existedCategory == null)
                return false;

            try
            {
                // Remove existing PokemonOwners
                var existingPokemonOwners = _context.PokemonOwners.Where(po => po.PokemonId == pokemon.Id).ToList();
                var existingPokemonCategories = _context.PokemonCategories.Where(po => po.PokemonId == pokemon.Id).ToList();

                _context.PokemonOwners.RemoveRange(existingPokemonOwners);
                _context.PokemonCategories.RemoveRange(existingPokemonCategories);

                // Add new PokemonOwner
                var newPokemonOwner = new PokemonOwner
                {
                    Owner = existedOwner,
                    Pokemon = pokemon
                };

                // Add new PokemonCategory
                var newPokemonCategory = new PokemonCategory
                {
                    Category = existedCategory,
                    Pokemon = pokemon
                };

                _context.PokemonOwners.Add(newPokemonOwner);
                _context.PokemonCategories.Add(newPokemonCategory);

                _context.Update(pokemon);

                return Save();
            }
            catch (Exception ex)
            {
                // Handle exceptions (log, throw, etc.)
                return false;
            }
        }
    }
}
