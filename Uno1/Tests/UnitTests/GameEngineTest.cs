using Domain;
using Tests.TestUtils;
using UnoGameEngine;
using Xunit.Abstractions;

namespace Tests.UnitTests;

public class GameEngineTest
{
    [Fact]
    public void GameEngine_AfterInitialization_Have108CardsInDeck()
    {
        var sampleGameEngine = new GameEngine();
        
        Assert.Equal(108, sampleGameEngine.GameState.UnoDeck.Cards.Count);
    }

    [Fact]
    public void GameEngine_DeleteAllWildFourCards_WildFourCardsWereDeletedFromDeck()
    {
        var sampleGameEngine = new GameEngine();
        
        sampleGameEngine.DeleteCardsWithValueToAvoid(UnoCard.Value.WildFour);
        
        var noWildFourCardsLeft = SampleGameEngine.HaveNoCardsWithValueToAvoid(sampleGameEngine, UnoCard.Value.WildFour);
        Assert.True(noWildFourCardsLeft);
    }

    [Fact]
    public void GameEngine_GetOneHandWith108CardDeck_AfterInDeckRemainedOnMaxCardsAmountLessCards()
    {
        var sampleGameEngine = new GameEngine
        {
            GameState =
            {
                MaxCardsAmount = 5
            }
        };

        sampleGameEngine.GetOneHand();
        
        var cardsInDeckLeft = sampleGameEngine.GameState.UnoDeck.Cards.Count;
        Assert.Equal(103,cardsInDeckLeft);
    }
    
    [Fact]
    public void GameEngine_CheckFirstCardInGameWhenDeckHave5LastCardsToAvoid_AfterInDeckRemained102Cards()
    {
        var sampleGameEngine = SampleGameEngine.GetWithUnoDeckWith5LastCardsToAvoid();

        sampleGameEngine.CheckFirstCardInGame();
        
        Assert.Equal(102,sampleGameEngine.GameState.UnoDeck.Cards.Count);
    }
    
    [Fact]
    public void GameEngine_CheckFirstCardInGameWhenDeckHave5LastCardsToAvoid_AfterFirstStockPileCardIsRedZero()
    {
        var sampleGameEngine = SampleGameEngine.GetWithUnoDeckWith5LastCardsToAvoid();

        sampleGameEngine.CheckFirstCardInGame();
        var firstStockPileCard = sampleGameEngine.GameState.StockPile.Last();
        
        Assert.Equal(UnoCard.Color.Red.ToString(), firstStockPileCard.CardColor.ToString());
        Assert.Equal(UnoCard.Value.Zero.ToString(), firstStockPileCard.CardValue.ToString());
    }

    [Fact]
    public void GameEngine_IsValidCardPlaySameCards_SameCardsCanBePlacedOnTopOfEachOther()
    {
        var sampleGameEngine = new GameEngine();
        sampleGameEngine.GameState.StockPile.Add(new UnoCard(UnoCard.Color.Red, UnoCard.Value.Zero));
        
        var cardCanBePlacedByColor = sampleGameEngine.IsValidCardPlay(new UnoCard(UnoCard.Color.Red, UnoCard.Value.One));
        var cardCanBePlacedByValue = sampleGameEngine.IsValidCardPlay(new UnoCard(UnoCard.Color.Blue, UnoCard.Value.Zero));

        Assert.True(cardCanBePlacedByColor);
        Assert.True(cardCanBePlacedByValue);
    }
    
    [Fact]
    public void GameEngine_IsValidCardPlayDifferentCards_DifferentCardsCantBePlacedOnTopOfEachOther()
    {
        var sampleGameEngine = new GameEngine();
        sampleGameEngine.GameState.StockPile.Add(new UnoCard(UnoCard.Color.Red, UnoCard.Value.Zero));
        
        var cardCantBePlaced = sampleGameEngine.IsValidCardPlay(new UnoCard(UnoCard.Color.Blue, UnoCard.Value.One));

        Assert.False(cardCantBePlaced);
    }
 
    [Fact]
    public void GameEngine_IsValidCardPlayWhenWildCardColorWasSetRed_CardWithSetColorCanBePlacedOnTopOfWildCard()
    {
        var sampleGameEngine = new GameEngine
        {
            GameState =
            {
                CardColorChoice = UnoCard.Color.Red
            }
        };
        sampleGameEngine.GameState.StockPile.Add(new UnoCard(UnoCard.Color.Wild, UnoCard.Value.Wild));
        
        var cardCanBePlaced = sampleGameEngine.IsValidCardPlay(new UnoCard(UnoCard.Color.Red, UnoCard.Value.Zero));

        Assert.True(cardCanBePlaced);
    }
    
    [Fact]
    public void GameEngine_IsValidCardPlayWhenWildCardColorWasSet_CardWithDifferentColorThanChosenCantBePlacedOnTopOfWildCard()
    {
        var sampleGameEngine = new GameEngine
        {
            GameState =
            {
                CardColorChoice = UnoCard.Color.Red
            }
        };
        sampleGameEngine.GameState.StockPile.Add(new UnoCard(UnoCard.Color.Wild, UnoCard.Value.Wild));
        
        var cardCantBePlaced = sampleGameEngine.IsValidCardPlay(new UnoCard(UnoCard.Color.Blue, UnoCard.Value.Zero));

        Assert.False(cardCantBePlaced);
    }
    
    
    
    
}
