using Domain;

namespace Tests.UnitTests.DomainTests;

public class UnoCardTest
{
    [Theory]
    [InlineData(UnoCard.Color.Red,UnoCard.Value.Zero)]
    [InlineData(UnoCard.Color.Blue,UnoCard.Value.One)]
    public void UnoCard_Initialize_SuccessfullyInitialized(UnoCard.Color color, UnoCard.Value value)
    {
        // Act
        var card = new UnoCard(color, value);

        // Assert
        Assert.Equal(color, card.CardColor);
        Assert.Equal(value, card.CardValue);
    }

    [Fact]
    public void UnoCard_ToString_ReturnsExpectedString()
    {
        // Arrange
        var card = new UnoCard(UnoCard.Color.Red,UnoCard.Value.Zero);

        // Act
        var result = card.ToString();

        // Assert
        Assert.Equal("Red_Zero", result);
    }
    
    [Fact]
    public void UnoCard_Equals_ReturnsTrueForEqualCards()
    {
        // Arrange
        var card1 = new UnoCard(UnoCard.Color.Green, UnoCard.Value.Seven);
        var card2 = new UnoCard(UnoCard.Color.Green, UnoCard.Value.Seven);

        // Act
        var result = card1.Equals(card2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void UnoCard_Equals_ReturnsFalseForDifferentCards()
    {
        // Arrange
        var card1 = new UnoCard(UnoCard.Color.Yellow, UnoCard.Value.DrawTwo);
        var card2 = new UnoCard(UnoCard.Color.Blue, UnoCard.Value.DrawTwo);

        // Act
        var result = card1.Equals(card2);

        // Assert
        Assert.False(result);
    }
}