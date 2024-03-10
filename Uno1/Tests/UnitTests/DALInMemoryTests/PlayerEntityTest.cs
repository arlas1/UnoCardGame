using DAL;
using FluentAssert;
using FluentAssertions;
using Tests.TestUtils.DALTestsUtils.Repositories;

// ReSharper disable InconsistentNaming
namespace Tests.UnitTests.DALInMemoryTests;

public class PlayerEntityTest
{
    private readonly AppDbContext _dbContext;
    private const int SampleId = 0;
    
    public PlayerEntityTest()
    {
        _dbContext = new AppDbContext();
        _dbContext.Database.EnsureCreated();
    }
    
    [Fact]
    public async void PlayerEntity_AddPlayer_PlayerAddedSuccessfully()
    {
        // Arrange
        var playerRepository = new PlayerRepository(_dbContext);
        var samplePlayer = playerRepository.CreateEntityWithId(SampleId);

        // Act
        await playerRepository.AddAsync(samplePlayer);

        // Assert
        var addedPlayers = playerRepository.GetAllAsync();
        addedPlayers.Should()
                    .ContainSingle()
                    .Which
                    .Should()
                    .BeEquivalentTo(samplePlayer);
        
        // Clean up 
        await playerRepository.DeleteAllAsync();
    }
    
    [Fact]
    public async void PlayerEntity_AddAndRetrievePlayer_PlayerRetrievedSuccessfully()
    {
        // Arrange
        var playerRepository = new PlayerRepository(_dbContext);
        var samplePlayer = playerRepository.CreateEntityWithId(SampleId);

        // Act
        await playerRepository.AddAsync(samplePlayer);
        var addedPlayer = playerRepository.GetById(SampleId);

        // Assert
        addedPlayer.Should().NotBeNull();
        addedPlayer.Should().BeEquivalentTo(samplePlayer);
        // Clean up 
        await playerRepository.DeleteAllAsync();
    }
    
    [Theory]
    [InlineData("p2", 1, 1)]
    [InlineData("p3", 2, 2)]
    public async void PlayerEntity_AddUpdateAndRetrievePlayer_PlayerUpdatedAndRetrievedSuccessfully(
        string newName,
        int newRole,
        int newType)
    {
        // Arrange
        var playerRepository = new PlayerRepository(_dbContext);
        var samplePlayer = playerRepository.CreateEntityWithId(SampleId);
        
        var samplePlayerName = samplePlayer.Name; // Is "p1"
        var samplePlayerRole = samplePlayer.Role; // Is 0
        var samplePlayerType = samplePlayer.Type; // Is 0
        
        // Act
        await playerRepository.AddAsync(samplePlayer);
        
        var addedPlayer = playerRepository.GetById(SampleId);
        addedPlayer!.Name = newName;
        addedPlayer!.Role = newRole;
        addedPlayer!.Type = newType;
        
        await playerRepository.UpdateAsync(addedPlayer);
        
        // Assert
        var updatedPlayer = playerRepository.GetById(SampleId);
        
        updatedPlayer!.Name.Should().NotBeEquivalentTo(samplePlayerName);
        updatedPlayer.Name.ShouldBeEqualTo(newName);
        
        updatedPlayer!.Role.ShouldNotBeEqualTo(samplePlayerRole);
        updatedPlayer.Role.ShouldBeEqualTo(newType);
        
        updatedPlayer!.Type.ShouldNotBeEqualTo(samplePlayerType);
        updatedPlayer.Type.ShouldBeEqualTo(newType);

        // Clean up 
        await playerRepository.DeleteAllAsync();
    }
    
    [Fact]
    public async void PlayerEntity_AddAndDeletePlayer_PlayerDeletedSuccessfully()
    {
        // Arrange
        var playerRepository = new PlayerRepository(_dbContext);
        var samplePlayer = playerRepository.CreateEntityWithId(SampleId);
        
        // Act
        await playerRepository.AddAsync(samplePlayer);
        await playerRepository.DeleteAsync(SampleId);

        // Assert
        var listWithAllPlayers = playerRepository.GetAllAsync();
        listWithAllPlayers.Should().BeEmpty();
    }

}