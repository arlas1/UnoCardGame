using Domain;

namespace Tests.UnitTests.DomainTests;

public class GameStateTest
{
    [Fact]
    public void GameState_Initialize_CreatesObject()
    {
        // Arrange & Act
        var gameState = new GameState();

        // Assert
        Assert.NotNull(gameState);
    }

    [Fact]
    public void GameState_InitializeAndCheckDefaultValues_AreSetCorrectly()
    {
        // Arrange & Act
        var gameState = new GameState();

        // Assert
        Assert.NotNull(gameState.UnoDeck);
        Assert.NotNull(gameState.StockPile);
        Assert.NotNull(gameState.PlayersList);
        Assert.False(gameState.GameDirection);
        Assert.False(gameState.IsColorChosen);
        Assert.Equal(default, gameState.CardColorChoice);
        Assert.Equal(0, gameState.MaxCardsAmount);
        Assert.Equal(0, gameState.CurrentPlayerIndex);
        Assert.Equal(0, gameState.SelectedCardIndex);
        Assert.Equal(0, gameState.MaxPlayersAmount);
        Assert.Equal(0, gameState.IsGameStarted);
        Assert.Equal(0, gameState.IsGameEnded);
        Assert.Equal(0, gameState.IsConsoleSaved);
        Assert.Equal(0, gameState.RepositoryChoice);
    }

    [Fact]
    public void GameState_SetValues_AreRetrievedCorrectly()
    {
        // Arrange
        var gameState = new GameState();
        var playersList = new List<Player>();
        playersList.Add(new Player(1, "Player 1", Player.PlayerType.Human));

        // Act
        gameState.MaxPlayersAmount = 4;
        gameState.PlayersList = playersList;

        // Assert
        Assert.Equal(4, gameState.MaxPlayersAmount);
        Assert.Equal(playersList, gameState.PlayersList);
    }
}