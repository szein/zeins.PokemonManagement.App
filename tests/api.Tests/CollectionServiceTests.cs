using api.Services;
using core.Models;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using Shouldly;

namespace api.Tests
{
  public class CollectionServiceTests
  {

    private DbContextFactory _dbContextFactory = new DbContextFactory();

    [Fact]
    public async Task CreateAsync_should_Add_Collection()
    {
      string expectedName = "TEST123";
      var context = _dbContextFactory.CreateFakeDbContext();
      var userId = Guid.NewGuid();
      var sut = CreateSut(context, userId);

      await sut.CreateAsync(expectedName);

      var actual = context.Collections.FirstOrDefault(x => x.Name == expectedName);
      actual.ShouldNotBeNull();
      actual.Name.ShouldBe(expectedName);
      actual.UserId.ShouldBe(userId);

    }

    [Fact]
    public async Task GetAllAsync_should_Return_User_Collections()
    {
      var context = _dbContextFactory.CreateFakeDbContext();
      var userId = Guid.NewGuid();
      var expectedCollection = new Collection { Name = "Mine", UserId = userId };
      context.Collections.AddRange(
        expectedCollection,
        new Collection { Name = "Other user's", UserId = Guid.NewGuid() });
      await context.SaveChangesAsync();
      var sut = CreateSut(context, userId);

      var actual = await sut.GetAllAsync();

      actual.Count.ShouldBe(1);
      actual[0].Id.ShouldBe(expectedCollection.Id.ToString());
      actual[0].Name.ShouldBe(expectedCollection.Name);
      actual[0].OwnerId.ShouldBe(userId.ToString());
    }

    [Fact]
    public async Task GetAllAsync_should_Create_Default_Collection_When_User_Has_None()
    {
      var context = _dbContextFactory.CreateFakeDbContext();
      var userId = Guid.NewGuid();
      var sut = CreateSut(context, userId);

      var actual = await sut.GetAllAsync();

      actual.Count.ShouldBe(1);
      actual[0].Name.ShouldBe("My Default Collection");
      actual[0].OwnerId.ShouldBe(userId.ToString());
    }

    [Fact]
    public async Task GetByIdAsync_should_Return_Collection_When_Found()
    {
      var context = _dbContextFactory.CreateFakeDbContext();
      var expected = new Collection { Name = "Found", UserId = Guid.NewGuid() };
      context.Collections.Add(expected);
      await context.SaveChangesAsync();
      var sut = CreateSut(context, Guid.NewGuid());

      var actual = await sut.GetByIdAsync(expected.Id);

      actual.ShouldNotBeNull();
      actual.Id.ShouldBe(expected.Id);
      actual.Name.ShouldBe(expected.Name);
    }

    [Fact]
    public async Task GetByIdAsync_should_Return_Null_When_Not_Found()
    {
      var context = _dbContextFactory.CreateFakeDbContext();
      var sut = CreateSut(context, Guid.NewGuid());

      var actual = await sut.GetByIdAsync(Guid.NewGuid());

      actual.ShouldBeNull();
    }

    [Fact]
    public async Task TryGetUserCollection_should_Return_Owned_Collection()
    {
      var context = _dbContextFactory.CreateFakeDbContext();
      var userId = Guid.NewGuid();
      var expected = new Collection { Name = "Mine", UserId = userId };
      context.Collections.Add(expected);
      await context.SaveChangesAsync();
      var sut = CreateSut(context, userId);

      var actual = await sut.TryGetUserCollection(expected.Id);

      actual.Status.ShouldBe(ServiceStatus.Ok);
      actual.Value.ShouldBe(expected);
    }

    [Fact]
    public async Task TryGetUserCollection_should_Return_NotFound_When_Missing()
    {
      var context = _dbContextFactory.CreateFakeDbContext();
      var sut = CreateSut(context, Guid.NewGuid());

      var actual = await sut.TryGetUserCollection(Guid.NewGuid());

      actual.Status.ShouldBe(ServiceStatus.NotFound);
    }

    [Fact]
    public async Task TryGetUserCollection_should_Return_Forbidden_For_Another_User()
    {
      var context = _dbContextFactory.CreateFakeDbContext();
      var expected = new Collection { Name = "Not mine", UserId = Guid.NewGuid() };
      context.Collections.Add(expected);
      await context.SaveChangesAsync();
      var sut = CreateSut(context, Guid.NewGuid());

      var actual = await sut.TryGetUserCollection(expected.Id);

      actual.Status.ShouldBe(ServiceStatus.Forbidden);
    }

    [Fact]
    public async Task AddItem_should_Add_Item_To_Collection()
    {
      var context = _dbContextFactory.CreateFakeDbContext();
      var collection = new Collection { Name = "Items", UserId = Guid.NewGuid() };
      context.Collections.Add(collection);
      await context.SaveChangesAsync();
      var sut = CreateSut(context, collection.UserId);
      var expected = new CollectionItem
      {
        PokemonId = "25",
        PokemonName = "Pikachu",
        PokemonHeight = "4",
        PokemonWeight = "60",
        PokemonUrl = "https://example.test/pokemon/25"
      };

      var actual = await sut.AddItem(collection.Id, expected);

      actual.ShouldBe(expected);
      context.CollectionItems.Single().PokemonName.ShouldBe(expected.PokemonName);
    }

    [Fact]
    public async Task RemoveItem_should_Remove_Existing_Item()
    {
      var context = _dbContextFactory.CreateFakeDbContext();
      var collection = new Collection { Name = "Items", UserId = Guid.NewGuid() };
      var item = new CollectionItem { PokemonId = "25", PokemonName = "Pikachu" };
      collection.CollectionItems.Add(item);
      context.Collections.Add(collection);
      await context.SaveChangesAsync();
      var sut = CreateSut(context, collection.UserId);

      var actual = await sut.RemoveItem(collection.Id, item.Id);

      actual.ShouldBeTrue();
      context.CollectionItems.ShouldBeEmpty();
    }

    [Fact]
    public async Task RemoveItem_should_Return_False_When_Item_Does_Not_Exist()
    {
      var context = _dbContextFactory.CreateFakeDbContext();
      var sut = CreateSut(context, Guid.NewGuid());

      var actual = await sut.RemoveItem(Guid.NewGuid(), Guid.NewGuid());

      actual.ShouldBeFalse();
    }

    [Fact]
    public async Task UpdateAsync_should_Change_Collection_Name()
    {
      var context = _dbContextFactory.CreateFakeDbContext();
      var collection = new Collection { Name = "Old name", UserId = Guid.NewGuid() };
      context.Collections.Add(collection);
      await context.SaveChangesAsync();
      var sut = CreateSut(context, collection.UserId);

      var actual = await sut.UpdateAsync(collection.Id, "New name");

      actual.ShouldNotBeNull();
      actual.Name.ShouldBe("New name");
      context.Collections.Find(collection.Id)!.Name.ShouldBe("New name");
    }

    [Fact]
    public async Task UpdateAsync_should_Return_Null_When_Collection_Does_Not_Exist()
    {
      var context = _dbContextFactory.CreateFakeDbContext();
      var sut = CreateSut(context, Guid.NewGuid());

      var actual = await sut.UpdateAsync(Guid.NewGuid(), "New name");

      actual.ShouldBeNull();
    }

    private static CollectionService CreateSut(AppDbContext context, Guid userId)
    {
      var userContext = A.Fake<IUserContext>();
      userContext.UserId = userId.ToString();
      A.CallTo(() => userContext.GetUserId()).Returns(userId);
      return new CollectionService(context, userContext, A.Fake<ILogger<CollectionService>>());
    }
  }
}
