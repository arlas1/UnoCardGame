using DAL;
using FluentAssert;
using FluentAssertions;
using Tests.TestUtils.DALTestsUtils;
using Tests.TestUtils.DALTestsUtils.PlayerEntityUtils;
using Xunit;

// ReSharper disable InconsistentNaming
namespace Tests.UnitTests.DALTests;

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
        var samplePlayer = EntityTestUtils.CreatePlayerEntityWithId(SampleId);
        var playerRepository = new PlayerRepository(_dbContext);

        // Act
        await playerRepository.AddPlayerAsync(samplePlayer);

        // Assert
        var addedPlayers = playerRepository.GetAllPlayersAsync();
        addedPlayers.Should()
                    .ContainSingle()
                    .Which
                    .Should()
                    .BeEquivalentTo(samplePlayer);
        
        // Clean up 
        await playerRepository.DeleteAllPlayersAsync();
    }
    
    [Fact]
    public async void PlayerEntity_AddAndRetrievePlayer_PlayerRetrievedSuccessfully()
    {
        // Arrange
        var samplePlayer = EntityTestUtils.CreatePlayerEntityWithId(SampleId);
        var playerRepository = new PlayerRepository(_dbContext);

        // Act
        await playerRepository.AddPlayerAsync(samplePlayer);
        var addedPlayer = playerRepository.GetPlayerById(SampleId);

        // Assert
        addedPlayer.Should().NotBeNull();
        addedPlayer.Should().BeEquivalentTo(samplePlayer);
        // Clean up 
        await playerRepository.DeleteAllPlayersAsync();
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
        var samplePlayer = EntityTestUtils.CreatePlayerEntityWithId(SampleId);
        
        var samplePlayerName = samplePlayer.Name; // Is "p1"
        var samplePlayerRole = samplePlayer.Role; // Is 0
        var samplePlayerType = samplePlayer.Type; // Is 0
        
        // Act
        await playerRepository.AddPlayerAsync(samplePlayer);
        
        var addedPlayer = playerRepository.GetPlayerById(SampleId);
        addedPlayer!.Name = newName;
        addedPlayer!.Role = newRole;
        addedPlayer!.Type = newType;
        
        await playerRepository.UpdatePlayerAsync(addedPlayer);
        
        // Assert
        var updatedPlayer = playerRepository.GetPlayerById(SampleId);
        
        updatedPlayer!.Name.Should().NotBeEquivalentTo(samplePlayerName);
        updatedPlayer.Name.ShouldBeEqualTo(newName);
        
        updatedPlayer!.Role.ShouldNotBeEqualTo(samplePlayerRole);
        updatedPlayer.Role.ShouldBeEqualTo(newType);
        
        updatedPlayer!.Type.ShouldNotBeEqualTo(samplePlayerType);
        updatedPlayer.Type.ShouldBeEqualTo(newType);

        // Clean up 
        await playerRepository.DeleteAllPlayersAsync();
    }
    
    [Fact]
    public async void PlayerEntity_AddAndDeletePlayer_PlayerDeletedSuccessfully()
    {
        // Arrange
        var samplePlayer = EntityTestUtils.CreatePlayerEntityWithId(SampleId);
        var playerRepository = new PlayerRepository(_dbContext);
        
        // Act
        await playerRepository.AddPlayerAsync(samplePlayer);
        await playerRepository.DeletePlayerAsync(SampleId);

        // Assert
        var listWithAllPlayers = playerRepository.GetAllPlayersAsync();
        listWithAllPlayers.Should().BeEmpty();
    }

}