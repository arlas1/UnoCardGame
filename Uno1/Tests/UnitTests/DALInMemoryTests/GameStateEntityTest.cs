using DAL;
using FluentAssert;
using FluentAssertions;
using Tests.TestUtils.DALTestsUtils.GameStateEntityTestUtils;

namespace Tests.UnitTests.DALInMemoryTests;

public class GameStateEntityTest
{
    private readonly AppDbContext _dbContext;
    private const int SampleId = 0;
    
    public GameStateEntityTest()
    {
        _dbContext = new AppDbContext();
        _dbContext.Database.EnsureCreated();
    }
    
    [Fact]
    public async void GameStateEntity_AddGameState_GameStateAddedSuccessfully()
    {
        // Arrange
        var gameStateRepository = new GameStateRepository(_dbContext);
        var sampleGameState = gameStateRepository.CreateEntityWithId(SampleId);

        // Act
        await gameStateRepository.AddAsync(sampleGameState);

        // Assert
        var addedGameStates = gameStateRepository.GetAllAsync();
        addedGameStates.Should()
                    .ContainSingle()
                    .Which
                    .Should()
                    .BeEquivalentTo(sampleGameState);
        
        // Clean up 
        await gameStateRepository.DeleteAllAsync();
    }
    
    [Fact]
    public async void GameStateEntity_AddAndRetrieveGameState_GameStateRetrievedSuccessfully()
    {
        // Arrange
        var gameStateRepository = new GameStateRepository(_dbContext);
        var sampleGameState = gameStateRepository.CreateEntityWithId(SampleId);

        // Act
        await gameStateRepository.AddAsync(sampleGameState);
        var addedGameState = gameStateRepository.GetById(SampleId);

        // Assert
        addedGameState.Should().NotBeNull();
        addedGameState.Should().BeEquivalentTo(sampleGameState);
        // Clean up 
        await gameStateRepository.DeleteAllAsync();
    }
    
    [Theory]
    [InlineData(1, 1, 1)]
    [InlineData(2, 2, 2)]
    public async void GameStateEntity_AddUpdateAndRetrieveGameState_GameStateUpdatedAndRetrievedSuccessfully(
        int newGameDirection,
        int newCurrentPlayerIndex,
        int newSelectedCardIndex)
    {
        // Arrange
        var gameStateRepository = new GameStateRepository(_dbContext);
        var sampleGameState = gameStateRepository.CreateEntityWithId(SampleId);
        
        var sampleGameDirection = sampleGameState.GameDirection; // Is 0
        var sampleCurrentPlayerIndex = sampleGameState.CurrentPlayerIndex; // Is 0
        var samplesSelectedCardIndex = sampleGameState.SelectedCardIndex; // Is 0
        
        // Act
        await gameStateRepository.AddAsync(sampleGameState);
        
        var addedGameState = gameStateRepository.GetById(SampleId);
        addedGameState!.GameDirection = newGameDirection;
        addedGameState!.CurrentPlayerIndex = newCurrentPlayerIndex;
        addedGameState!.SelectedCardIndex = newSelectedCardIndex;
        
        await gameStateRepository.UpdateAsync(addedGameState);
        
        // Assert
        var updatedGameState = gameStateRepository.GetById(SampleId);
        
        updatedGameState!.GameDirection.ShouldNotBeEqualTo(sampleGameDirection);
        updatedGameState.GameDirection.ShouldBeEqualTo(newGameDirection);
        
        updatedGameState!.CurrentPlayerIndex.ShouldNotBeEqualTo(sampleCurrentPlayerIndex);
        updatedGameState.CurrentPlayerIndex.ShouldBeEqualTo(newCurrentPlayerIndex);
        
        updatedGameState!.SelectedCardIndex.ShouldNotBeEqualTo(samplesSelectedCardIndex);
        updatedGameState.SelectedCardIndex.ShouldBeEqualTo(newSelectedCardIndex);

        // Clean up 
        await gameStateRepository.DeleteAllAsync();
    }
    
    [Fact]
    public async void GameStateEntity_AddAndDeleteGameState_GameStateDeletedSuccessfully()
    {
        // Arrange
        var gameStateRepository = new GameStateRepository(_dbContext);
        var sampleGameState = gameStateRepository.CreateEntityWithId(SampleId);
        
        // Act
        await gameStateRepository.AddAsync(sampleGameState);
        await gameStateRepository.DeleteAsync(SampleId);

        // Assert
        var listWithAllGameStates = gameStateRepository.GetAllAsync();
        listWithAllGameStates.Should().BeEmpty();
    }
}