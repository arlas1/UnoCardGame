using Domain;
using Tests.TestUtils;
using UnoGameEngine;

namespace Tests.UnitTests.UnoGameEngineTests;

public class GameEngineTest
{
    [Fact]   // Which class tested / Action / Result
    public void GameEngine_Initialize_GameStatePropertyInitialized()
    {
        // Arrange
        var sampleGameEngine = new GameEngine();
        
        // Act
        var gameState = sampleGameEngine.GameState;
        
        // Assert
        Assert.NotNull(gameState);
    }
    
    [Fact]
    public void GameEngine_Initialize_GameStateHave108CardsInDeck()
    {
        // Arrange, Act
        var sampleGameEngine = new GameEngine();

        // Assert
        Assert.Equal(108, sampleGameEngine.GameState.UnoDeck.Cards.Count);
    }

    [Fact]
    public void GameEngine_DeleteAllWildFourCards_WildFourCardsWereDeletedFromDeck()
    {
        // Arrange
        var sampleGameEngine = new GameEngine();
        
        // Act
        sampleGameEngine.DeleteCardsWithValueToAvoid(UnoCard.Value.WildFour);
        
        // Assert
        var noWildFourCardsLeft = GameEngineUtils.NoCardsWithValue(sampleGameEngine, UnoCard.Value.WildFour);
        Assert.True(noWildFourCardsLeft);
    }

    [Fact]
    public void GameEngine_GetOneHandWith108CardDeck_AfterInDeckRemainedOnMaxCardsAmountLessCards()
    {
        // Arrange
        var sampleGameEngine = new GameEngine
        {
            GameState =
            {
                MaxCardsAmount = 5
            }
        };

        // Act
        sampleGameEngine.GetOneHand();

        // Assert
        var cardsInDeckLeft = sampleGameEngine.GameState.UnoDeck.Cards.Count;
        Assert.Equal(103, cardsInDeckLeft);
    }

    [Fact]
    public void GameEngine_CheckFirstCardInGameWhenDeckHave5LastCardsToAvoid_AfterInDeckRemained102Cards()
    {
        // Arrange
        var sampleGameEngine = GameEngineUtils.CreateWithUnoDeckWith5LastCardsToAvoid();

        // Act
        sampleGameEngine.CheckFirstCardInGame();

        // Assert
        Assert.Equal(102, sampleGameEngine.GameState.UnoDeck.Cards.Count);
    }

    [Fact]
    public void GameEngine_CheckFirstCardInGameWhenDeckHave5LastCardsToAvoid_AfterFirstStockPileCardIsRedZero()
    {
        // Arrange
        var sampleGameEngine = GameEngineUtils.CreateWithUnoDeckWith5LastCardsToAvoid();

        // Act
        sampleGameEngine.CheckFirstCardInGame();
        var firstStockPileCard = sampleGameEngine.GameState.StockPile.Last();

        // Assert
        Assert.Equal(UnoCard.Color.Red.ToString(), firstStockPileCard.CardColor.ToString());
        Assert.Equal(UnoCard.Value.Zero.ToString(), firstStockPileCard.CardValue.ToString());
    }

    [Fact]
    public void GameEngine_IsValidCardPlaySameCards_SameCardsCanBePlacedOnTopOfEachOther()
    {
        // Arrange
        var sampleGameEngine = new GameEngine();
        sampleGameEngine.GameState.StockPile.Add(new UnoCard(UnoCard.Color.Red, UnoCard.Value.Zero));

        // Act
        var cardCanBePlacedByColor = sampleGameEngine
            .IsValidCardPlay(new UnoCard(UnoCard.Color.Red, UnoCard.Value.One));
        var cardCanBePlacedByValue = sampleGameEngine
            .IsValidCardPlay(new UnoCard(UnoCard.Color.Blue, UnoCard.Value.Zero));
        
        // Assert
        Assert.True(cardCanBePlacedByColor);
        Assert.True(cardCanBePlacedByValue);
    }

    [Fact]
    public void GameEngine_IsValidCardPlayDifferentCards_DifferentCardsCantBePlacedOnTopOfEachOther()
    {
        // Arrange
        var sampleGameEngine = new GameEngine();
        sampleGameEngine.GameState.StockPile.Add(new UnoCard(UnoCard.Color.Red, UnoCard.Value.Zero));

        // Act
        var cardCantBePlaced = sampleGameEngine.IsValidCardPlay(new UnoCard(UnoCard.Color.Blue, UnoCard.Value.One));

        // Assert
        Assert.False(cardCantBePlaced);
    }

    [Fact]
    public void GameEngine_IsValidCardPlayWhenWildCardColorWasSetRed_CardWithSetColorCanBePlacedOnTopOfWildCard()
    {
        // Arrange
        var sampleGameEngine = GameEngineUtils.CreateWithColorChoice(UnoCard.Color.Red);
        sampleGameEngine.GameState.StockPile.Add(new UnoCard(UnoCard.Color.Wild, UnoCard.Value.Wild));

        // Act
        var cardCanBePlaced = sampleGameEngine.IsValidCardPlay(new UnoCard(UnoCard.Color.Red, UnoCard.Value.Zero));

        // Assert
        Assert.True(cardCanBePlaced);
    }

    [Fact]
    public void GameEngine_IsValidCardPlayWhenWildCardColorWasSet_CardWithDifferentColorOtherThanChosenCantBePlacedOnTopOfWildCard()
    {
        // Arrange
        var sampleGameEngine = GameEngineUtils.CreateWithColorChoice(UnoCard.Color.Red);
        sampleGameEngine.GameState.StockPile.Add(new UnoCard(UnoCard.Color.Wild, UnoCard.Value.Wild));

        // Act
        var cardCantBePlaced = sampleGameEngine.IsValidCardPlay(new UnoCard(UnoCard.Color.Blue, UnoCard.Value.Zero));
        
        // Assert
        Assert.False(cardCantBePlaced);
    }

    [Fact]
    public void GameEngine_GetNextPlayerIdWithNonSkipCardAndClockwiseGameDirection_HappensMoveForward()
    {
        // Arrange
        var sampleGameEngine = GameEngineUtils.CreateWithRequiredGameDirection(false); // Clockwise
        sampleGameEngine.GameState.StockPile.Add(new UnoCard(UnoCard.Color.Red, UnoCard.Value.Zero));
        GameEngineUtils.LoadAmountOfHumanPlayers(3, sampleGameEngine);

        // Act
        sampleGameEngine.GetNextPlayerId(0);

        // Assert
        var nextPlayerId = sampleGameEngine.GameState.CurrentPlayerIndex;
        Assert.Equal(1, nextPlayerId);
    }

    [Fact]
    public void GameEngine_GetNextPlayerIdWithNonSkipCardAndCounterClockwiseGameDirection_HappensMoveBackward()
    {
        // Arrange
        var sampleGameEngine = GameEngineUtils.CreateWithRequiredGameDirection(true); // Counter-clockwise
        sampleGameEngine.GameState.StockPile.Add(new UnoCard(UnoCard.Color.Red, UnoCard.Value.Zero));
        GameEngineUtils.LoadAmountOfHumanPlayers(3, sampleGameEngine);

        // Act
        sampleGameEngine.GetNextPlayerId(0);

        // Assert
        var nextPlayerId = sampleGameEngine.GameState.CurrentPlayerIndex;
        Assert.Equal(2, nextPlayerId);
    }

    [Fact]
    public void GameEngine_GetNextPlayerIdWithSkipCardAndClockwiseGameDirection_HappensDoubleMoveForward()
    {
        // Arrange
        var sampleGameEngine = GameEngineUtils.CreateWithRequiredGameDirection(false); // Clockwise
        sampleGameEngine.GameState.StockPile.Add(new UnoCard(UnoCard.Color.Red, UnoCard.Value.Skip));
        GameEngineUtils.LoadAmountOfHumanPlayers(3, sampleGameEngine);

        // Act
        sampleGameEngine.GetNextPlayerId(0);

        // Assert
        var nextPlayerId = sampleGameEngine.GameState.CurrentPlayerIndex;
        Assert.Equal(2, nextPlayerId);
    }

    [Fact]
    public void GameEngine_GetNextPlayerIdWithSkipCardAndCounterClockwiseGameDirection_HappensDoubleMoveBackward()
    {
        // Arrange
        var sampleGameEngine = GameEngineUtils.CreateWithRequiredGameDirection(true); // Counter-clockwise
        sampleGameEngine.GameState.StockPile.Add(new UnoCard(UnoCard.Color.Red, UnoCard.Value.Skip));
        GameEngineUtils.LoadAmountOfHumanPlayers(3, sampleGameEngine);

        // Act
        sampleGameEngine.GetNextPlayerId(0);

        // Assert
        var nextPlayerId = sampleGameEngine.GameState.CurrentPlayerIndex;
        Assert.Equal(1, nextPlayerId);
    }

    [Fact]
    public void GameEngine_SubmitPlayerCardReverse_GameDirectionWasReversed()
    {
        // Arrange
        var sampleGameEngine = GameEngineUtils.CreateWithRequiredGameDirection(false); // Clockwise
        
        // Act
        sampleGameEngine.SubmitPlayerCard(0, new UnoCard(UnoCard.Color.Red, UnoCard.Value.Reverse));

        // Assert
        var gameDirectionChanged = sampleGameEngine.GameState.GameDirection;
        Assert.True(gameDirectionChanged);
    }
    
    [Fact]
    public void GameEngine_SubmitPlayerCardDrawTwoWithClockwiseGameDirection_NextForwardPlayerHad0CardsNowHave2()
    {
        // Arrange
        var sampleGameEngine = GameEngineUtils.CreateWithRequiredGameDirection(false); // Clockwise
        GameEngineUtils.LoadAmountOfHumanPlayers(3, sampleGameEngine);
        sampleGameEngine.GameState.StockPile.Add(new UnoCard(UnoCard.Color.Red, UnoCard.Value.Zero));

        // Act
        var currentPlayerId = 0;
        sampleGameEngine.SubmitPlayerCard(currentPlayerId, new UnoCard(UnoCard.Color.Red, UnoCard.Value.DrawTwo));
        sampleGameEngine.GetNextPlayerId(currentPlayerId);
        
        // Assert
        var nextPlayerId = sampleGameEngine.GameState.CurrentPlayerIndex;
        var nextPlayerCardsAmount = sampleGameEngine.GameState.PlayersList[nextPlayerId].Hand.Count;
        
        Assert.Equal(2,nextPlayerCardsAmount);
    }

    [Fact]
    public void GameEngine_SubmitPlayerCardDrawTwoWithCounterClockwiseGameDirection_NextBackwardPlayerHad0CardsNowHave2()
    {
        // Arrange
        var sampleGameEngine = GameEngineUtils.CreateWithRequiredGameDirection(true); // Clockwise
        GameEngineUtils.LoadAmountOfHumanPlayers(3, sampleGameEngine);
        sampleGameEngine.GameState.StockPile.Add(new UnoCard(UnoCard.Color.Red, UnoCard.Value.Zero));
        
        // Act
        var currentPlayerId = 0;
        sampleGameEngine.SubmitPlayerCard(currentPlayerId, new UnoCard(UnoCard.Color.Red, UnoCard.Value.DrawTwo));
        sampleGameEngine.GetNextPlayerId(currentPlayerId);

        // Assert
        var nextPlayerId = sampleGameEngine.GameState.CurrentPlayerIndex;
        var nextPlayerCardsAmount = sampleGameEngine.GameState.PlayersList[nextPlayerId].Hand.Count;
        
        Assert.Equal(2,nextPlayerCardsAmount);
    }

    [Fact]
    public void GameEngine_SubmitPlayerCardWildFourWithClockwiseGameDirection_NextForwardPlayerHad0CardsNowHave4()
    {
        // Arrange
        var sampleGameEngine = GameEngineUtils.CreateWithRequiredGameDirection(false); // Clockwise
        GameEngineUtils.LoadAmountOfHumanPlayers(3, sampleGameEngine);
        sampleGameEngine.GameState.StockPile.Add(new UnoCard(UnoCard.Color.Red, UnoCard.Value.Zero));

        // Act
        var currentPlayerId = 0;
        sampleGameEngine.SubmitPlayerCard(currentPlayerId, new UnoCard(UnoCard.Color.Wild, UnoCard.Value.WildFour));
        sampleGameEngine.GetNextPlayerId(currentPlayerId);
        
        // Assert
        var nextPlayerId = sampleGameEngine.GameState.CurrentPlayerIndex;
        var nextPlayerCardsAmount = sampleGameEngine.GameState.PlayersList[nextPlayerId].Hand.Count;
        
        Assert.Equal(4,nextPlayerCardsAmount);
    }

    [Fact]
    public void GameEngine_SubmitPlayerCardWildFourWithCounterClockwiseGameDirection_NextBackwardPlayerHad0CardsNowHave4()
    {
        // Arrange
        var sampleGameEngine = GameEngineUtils.CreateWithRequiredGameDirection(true); // Clockwise
        GameEngineUtils.LoadAmountOfHumanPlayers(3, sampleGameEngine);
        sampleGameEngine.GameState.StockPile.Add(new UnoCard(UnoCard.Color.Red, UnoCard.Value.Zero));
        
        // Act
        var currentPlayerId = 0;
        sampleGameEngine.SubmitPlayerCard(currentPlayerId, new UnoCard(UnoCard.Color.Wild, UnoCard.Value.WildFour));
        sampleGameEngine.GetNextPlayerId(currentPlayerId);

        // Assert
        var nextPlayerId = sampleGameEngine.GameState.CurrentPlayerIndex;
        var nextPlayerCardsAmount = sampleGameEngine.GameState.PlayersList[nextPlayerId].Hand.Count;
        
        Assert.Equal(4,nextPlayerCardsAmount);
    }

    [Fact]
    public void GameEngine_GetGameStateCopy_ReturnsCopy()
    {
        // Arrange
        var sampleGameEngine = GameEngineUtils.CreateWithAllPropertiesSet();

        // Act
        var sampleGameEngineCopy = sampleGameEngine.GetGameStateCopy();
        
        // Assert
        Assert.Equal(sampleGameEngine.GameState.GameDirection, sampleGameEngineCopy.GameDirection);
        Assert.Equal(sampleGameEngine.GameState.CurrentPlayerIndex, sampleGameEngineCopy.CurrentPlayerIndex);
        Assert.Equal(sampleGameEngine.GameState.CardColorChoice, sampleGameEngineCopy.CardColorChoice);
        Assert.Equal(sampleGameEngine.GameState.IsColorChosen, sampleGameEngineCopy.IsColorChosen);
        Assert.Equal(sampleGameEngine.GameState.SelectedCardIndex, sampleGameEngineCopy.SelectedCardIndex);
        Assert.Equal(sampleGameEngine.GameState.PlayersList, sampleGameEngineCopy.PlayersList);
        Assert.Equal(sampleGameEngine.GameState.StockPile, sampleGameEngineCopy.StockPile);
        Assert.Equal(sampleGameEngine.GameState.UnoDeck, sampleGameEngineCopy.UnoDeck);
    }
    
}
