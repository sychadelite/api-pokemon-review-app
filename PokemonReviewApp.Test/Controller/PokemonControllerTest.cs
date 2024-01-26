using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Controllers;
using PokemonReviewApp.DTO;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PokemonReviewApp.Test.Controller
{
    public class PokemonControllerTest
    {
        private readonly IPokemonRepository _pokemonRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;

        public PokemonControllerTest()
        {
            _pokemonRepository = A.Fake<IPokemonRepository>();
            _reviewRepository = A.Fake<IReviewRepository>();
            _mapper = A.Fake<IMapper>();
        }

        [Fact]
        public void PokemonController_GetPokemons_ReturnOK()
        {
            // Arrange
            var pokemons = A.Fake<ICollection<PokemonDTO>>();
            var pokemonList = A.Fake<List<PokemonDTO>>();

            A.CallTo(() => _mapper.Map<List<PokemonDTO>>(pokemons)).Returns(pokemonList);

            var controller = new PokemonController(_pokemonRepository, _reviewRepository, _mapper);

            // Act
            var result = controller.GetPokemons();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof (OkObjectResult));
        }

        [Fact]
        public void PokemonController_CreatePokemon_ReturnOK()
        {
            // Arrange
            int ownerId = 1;
            int categoryId = 2;
             
            var pokemon = A.Fake<Pokemon>();
            var pokemonCreate = A.Fake<PokemonDTO>();
            var pokemons = A.Fake<ICollection<PokemonDTO>>();
            var pokemonList = A.Fake<List<PokemonDTO>>();

            A.CallTo(() => _pokemonRepository.GetPokemonTrimToUpper(pokemonCreate)).Returns(pokemon);

            A.CallTo(() => _mapper.Map<Pokemon>(pokemonCreate)).Returns(pokemon);

            A.CallTo(() => _pokemonRepository.CreatePokemon(ownerId, categoryId, pokemon)).Returns(true);

            var controller = new PokemonController(_pokemonRepository, _reviewRepository, _mapper);

            // Act
            var result = controller.CreatePokemon(ownerId, categoryId, pokemonCreate);

            // Assert
            result.Should().NotBeNull();
        }
    }
}
