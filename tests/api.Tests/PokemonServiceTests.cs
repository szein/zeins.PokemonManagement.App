using System.Net;
using System.Net.Http.Json;
using api.Services;
using FakeItEasy;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Shouldly;
using static api.DTOs.PokemonDTOs;

namespace api.Tests
{
  public class PokemonServiceTests
  {
    [Fact]
    public async Task GetPokemonsAsync_should_Return_Pokemon_List()
    {
      var expected = new PokemonListResponse(
        1,
        null,
        null,
        new List<PokemonListItem> { new("pikachu", "https://pokeapi.test/pokemon/25") });
      var handler = new StubHttpMessageHandler(_ => JsonResponse(expected));
      var sut = CreateSut(handler);

      var actual = await sut.GetPokemonsAsync(1, 5);

      actual.Count.ShouldBe(expected.Count);
      actual.Results.Count.ShouldBe(1);
      actual.Results[0].Name.ShouldBe(expected.Results[0].Name);
      actual.Results[0].Url.ShouldBe(expected.Results[0].Url);
      handler.Requests.ShouldContain("/pokemon?limit=1&offset=5");
    }

    [Fact]
    public async Task GetPokemonsAsync_should_Reuse_Cached_Response()
    {
      var handler = new StubHttpMessageHandler(_ => JsonResponse(
        new PokemonListResponse(0, null, null, new List<PokemonListItem>())));
      var sut = CreateSut(handler);

      await sut.GetPokemonsAsync(10, 0);
      await sut.GetPokemonsAsync(10, 0);

      handler.Requests.Count.ShouldBe(1);
    }

    [Fact]
    public async Task GetPokemonAsync_should_Return_Pokemon_And_Reuse_Cache_Case_Insensitively()
    {
      var expected = new PokemonDTO(25, "pikachu", 4, 60, "https://pokeapi.test/pokemon/25");
      var handler = new StubHttpMessageHandler(_ => JsonResponse(expected));
      var sut = CreateSut(handler);

      var first = await sut.GetPokemonAsync("Pikachu");
      var second = await sut.GetPokemonAsync("PIKACHU");

      first.ShouldBe(expected);
      second.ShouldBe(expected);
      handler.Requests.ShouldBe(new[] { "/pokemon/pikachu" });
    }

    [Fact]
    public async Task GetPokemonAsync_should_Return_Null_When_Not_Found()
    {
      var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.NotFound));
      var sut = CreateSut(handler);

      var actual = await sut.GetPokemonAsync("missingno");

      actual.ShouldBeNull();
      handler.Requests.ShouldContain("/pokemon/missingno");
    }

    [Fact]
    public async Task GetPokemonAsync_should_Throw_For_NonNotFound_Http_Errors()
    {
      var handler = new StubHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));
      var sut = CreateSut(handler);

      var exception = await Should.ThrowAsync<HttpRequestException>(() => sut.GetPokemonAsync("pikachu"));

      exception.StatusCode.ShouldBe(HttpStatusCode.ServiceUnavailable);
    }

    private static PokemonService CreateSut(StubHttpMessageHandler handler)
    {
      var httpClient = new HttpClient(handler)
      {
        BaseAddress = new Uri("https://pokeapi.test/")
      };
      var logger = A.Fake<ILogger<PokemonService>>();
      var cache = new MemoryCache(new MemoryCacheOptions());
      return new PokemonService(httpClient, logger, cache);
    }

    private static HttpResponseMessage JsonResponse<T>(T value)
    {
      return new HttpResponseMessage(HttpStatusCode.OK)
      {
        Content = JsonContent.Create(value)
      };
    }

    private sealed class StubHttpMessageHandler : HttpMessageHandler
    {
      private readonly Func<HttpRequestMessage, HttpResponseMessage> _responseFactory;

      public StubHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
      {
        _responseFactory = responseFactory;
      }

      public List<string> Requests { get; } = new();

      protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
      {
        Requests.Add(request.RequestUri!.PathAndQuery);
        return Task.FromResult(_responseFactory(request));
      }
    }
  }
}
