using Domain;

namespace Tests.UnitTests.DomainTests;

public class UnoDeckTest
{
    [Fact]
    public void UnoDeck_Create_CreatesDeckWithExpectedCards()
    {
        // Arrange
        var unoDeck = new UnoDeck();

        // Act
        unoDeck.Create();

        // Assert
        Assert.Equal(108, unoDeck.Cards.Count);

        // Check if each color has the expected number of cards
        foreach (var color in Enum.GetValues(typeof(UnoCard.Color)).Cast<UnoCard.Color>())
        {
            var count = unoDeck.Cards.Count(card => card.CardColor == color);
            Assert.Equal(25, count);
        }

        // Check if each value (except wild cards) has the expected number of cards
        foreach (var value in Enum.GetValues(typeof(UnoCard.Value)).Cast<UnoCard.Value>())
        {
            if (value != UnoCard.Value.Wild && value != UnoCard.Value.WildFour)
            {
                var count = unoDeck.Cards.Count(card => card.CardValue == value);
                Assert.Equal(4, count);
            }
        }
    }

    [Fact]
    public void UnoDeck_AddCardToDeck_CardsCountIncreasesByOne()
    {
        // Arrange
        var unoDeck = new UnoDeck();
        unoDeck.Create();
        var initialCount = unoDeck.Cards.Count;
        var card = new UnoCard(UnoCard.Color.Blue, UnoCard.Value.One);

        // Act
        unoDeck.AddCardToDeck(card);

        // Assert
        Assert.Equal(initialCount + 1, unoDeck.Cards.Count);
    }

    [Fact]
    public void UnoDeck_RemoveCardsWithValue_CardsWithSpecifiedValueAreRemoved()
    {
        // Arrange
        var unoDeck = new UnoDeck();
        unoDeck.Create();
        var valueToRemove = UnoCard.Value.Five;

        // Act
        unoDeck.RemoveCardsWithValue(valueToRemove);

        // Assert
        Assert.DoesNotContain(unoDeck.Cards, card => card.CardValue == valueToRemove);
    }

    [Fact]
    public void UnoDeck_IsEmpty_ReturnsTrueIfDeckIsEmpty()
    {
        // Arrange
        var unoDeck = new UnoDeck();

        // Act & Assert
        Assert.True(unoDeck.IsEmpty());

        // Add a card to the deck
        unoDeck.AddCardToDeck(new UnoCard(UnoCard.Color.Red, UnoCard.Value.Zero));

        // Act & Assert
        Assert.False(unoDeck.IsEmpty());
    }

    [Fact]
    public void UnoDeck_Shuffle_CardsAreShuffled()
    {
        // Arrange
        var unoDeck = new UnoDeck();
        unoDeck.Create();
        var originalOrder = new List<UnoCard>(unoDeck.Cards);

        // Act
        unoDeck.Shuffle();

        // Assert
        Assert.NotEqual(originalOrder, unoDeck.Cards);
    }

    [Fact]
    public void UnoDeck_DrawCard_RemovesTopCardFromDeckAndReturnsIt()
    {
        // Arrange
        var unoDeck = new UnoDeck();
        unoDeck.Create();
        var expectedCard = unoDeck.Cards.Last();

        // Act
        var drawnCard = unoDeck.DrawCard();

        // Assert
        Assert.Equal(expectedCard, drawnCard);
        Assert.DoesNotContain(drawnCard, unoDeck.Cards);
    }

    [Fact]
    public void UnoDeck_Clear_RemovesAllCardsFromDeck()
    {
        // Arrange
        var unoDeck = new UnoDeck();
        unoDeck.Create();

        // Act
        unoDeck.Clear();

        // Assert
        Assert.Empty(unoDeck.Cards);
    }
}