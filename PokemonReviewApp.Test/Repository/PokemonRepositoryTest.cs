using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using PokemonReviewApp.Data;
using PokemonReviewApp.Models;
using PokemonReviewApp.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonReviewApp.Test.Repository
{
    public class PokemonRepositoryTest
    {
        private async Task<DataContext> GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var databaseContext = new DataContext(options);

            databaseContext.Database.EnsureCreated();

            if (await databaseContext.Pokemons.CountAsync() <= 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    databaseContext.Pokemons.Add(new Pokemon()
                    {
                        Name = "Pikachu",
                        BirthDate = new DateTime(1903, 1, 1),
                        PokemonCategories = new List<PokemonCategory>()
                        {
                            new PokemonCategory
                            {
                                Category = new Category()
                                {
                                    Name = "Electric"
                                }
                            }
                        },

                        Reviews = new List<Review>()
                        {
                            new Review()
                            {
                                Title = "Pikachu Is Good",
                                Text = "Pikachu is the best pokemon, because it is electric",
                                Rating = 5,
                                Reviewer = new Reviewer()
                                {
                                    FirstName = "Barj",
                                    LastName = "Lazuardi"
                                }
                            },
                            new Review()
                            {
                                Title = "Pikachu Cute",
                                Text = "Pikachu is the cutest pokemon, because it has a unique voice and color",
                                Rating = 3,
                                Reviewer = new Reviewer()
                                {
                                    FirstName = "Lady",
                                    LastName = "Galadriel"
                                }
                            },
                            new Review()
                            {
                                Title = "Pikachu Awesome",
                                Text = "Pikachu is an awesome pokemon, because it is yellow",
                                Rating = 4,
                                Reviewer = new Reviewer()
                                {
                                    FirstName = "Tony",
                                    LastName = "Montana"
                                }
                            }
                        }
                    });

                    await databaseContext.SaveChangesAsync();
                }
            }

            return databaseContext;
        }

        [Fact]
        public async void PokemonRepository_GetPokemon_ReturnPokemon()
        {
            // Arrange
            var name = "Pikachu";
            var dbContext = await GetDatabaseContext();
            var pokemonRepository = new PokemonRepository(dbContext);

            // Act
            var result = pokemonRepository.GetPokemon(name);

            // Assert
            result.Should().NotBeNull();
            result .Should().BeOfType<Pokemon>();
        }

        [Fact]
        public async void PokemonRepository_GetPokemonRating_ReturnDecimalBetweenOneAndTen()
        {
            // Arrange
            var pokemonId = 1;
            var dbContext = await GetDatabaseContext();
            var pokemonRepository = new PokemonRepository (dbContext);

            // Act
            var result = pokemonRepository.GetPokemonRating(pokemonId);

            // Assert
            result.Should().NotBe(0);
            result.Should().BeInRange(1, 10);
        }
    }
}
