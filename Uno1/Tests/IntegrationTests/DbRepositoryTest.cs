using DAL;
using Microsoft.EntityFrameworkCore;
using Tests.TestUtils.DALTestsUtils;
using UnoGameEngine;

namespace Tests.IntegrationTests.DALTests;

public class DbRepositoryTest
{
    [Fact]
    public void DbRepository_GetContext_CreatedContextIsNotNull()
    {
        // Arrange & Act
        var dbContext = DbRepository.GetContext();
        
        // Assert
        Assert.NotNull(dbContext);
    }
    
    [Fact]
    public async Task DbRepository_SaveAndLoadGameEngine_SavedAndLoadedSameEngines()
    {
        var dbContext = DbRepository.GetContext();
        // Arrange
        var sampleGameEngine = DbRepositoryTestUtils.CreateSampleGameEngine();
    
        // Act
        DbRepository.SaveIntoDb(sampleGameEngine);
        var savedGameStateId = await dbContext.GameStates
            .OrderByDescending(gs => gs.Id)
            .Select(gs => gs.Id)
            .LastAsync();


        var newGameEngine = new GameEngine();
        DbRepository.LoadFromDb(savedGameStateId, dbContext, newGameEngine);
    
        // Assert
        Assert.Equal(sampleGameEngine.GameState.GameDirection, newGameEngine.GameState.GameDirection);
        Assert.Equal(sampleGameEngine.GameState.CurrentPlayerIndex, newGameEngine.GameState.CurrentPlayerIndex);
        Assert.Equal(sampleGameEngine.GameState.IsColorChosen, newGameEngine.GameState.IsColorChosen);
        Assert.Equal(sampleGameEngine.GameState.SelectedCardIndex, newGameEngine.GameState.SelectedCardIndex);
        Assert.Equal(sampleGameEngine.GameState.CardColorChoice, newGameEngine.GameState.CardColorChoice);
        Assert.Equal(sampleGameEngine.GameState.MaxCardsAmount, newGameEngine.GameState.MaxCardsAmount);
        Assert.Equal(sampleGameEngine.GameState.IsGameStarted, newGameEngine.GameState.IsGameStarted);
        Assert.Equal(sampleGameEngine.GameState.IsGameEnded, newGameEngine.GameState.IsGameEnded);
        Assert.Equal(sampleGameEngine.GameState.IsColorChosen, newGameEngine.GameState.IsColorChosen);
        Assert.Equal(sampleGameEngine.GameState.StockPile, newGameEngine.GameState.StockPile);
        
        // Cleanup
        await DbRepositoryTestUtils.CleanUpTheDb(dbContext);
    }
    
}