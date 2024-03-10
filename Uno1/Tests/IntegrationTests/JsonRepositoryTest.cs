using DAL;
using DAL.DbEntities;
using Microsoft.EntityFrameworkCore;
using Tests.TestUtils.DALTestsUtils;
using UnoGameEngine;

namespace Tests.IntegrationTests.DALTests;

public class JsonRepositoryTest
{
    [Fact]
    public void JsonRepository_SaveAndLoadGameEngine_SavedAndLoadedSameEngines()
    {
        // Arrange
        var sampleGameEngine = DbRepositoryTestUtils.CreateSampleGameEngine();
    
        // Act
        JsonRepository.SaveIntoJson(sampleGameEngine);
        
        var jsonFolderPath = JsonRepository.GetPathForTheJsonSaves();
        var jsonSaves = Directory.GetFiles(jsonFolderPath);
        
        var savedGameEngineJsonString = File.ReadAllText(jsonSaves.Last());
        var newGameEngine = new GameEngine();
        
        JsonRepository.LoadFromJson(savedGameEngineJsonString, newGameEngine);
        
        // // Assert
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
        var lastFilePath = jsonSaves.LastOrDefault();
        File.Delete(lastFilePath!);
    }
}