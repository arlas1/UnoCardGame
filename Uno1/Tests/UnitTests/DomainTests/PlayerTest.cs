using Domain;

namespace Tests.UnitTests.DomainTests;

public class PlayerTest
{
    [Theory]
    [InlineData(0, "Alice", Player.PlayerType.Human)]
    [InlineData(1, "Bob", Player.PlayerType.Ai)]
    public void Player_Initialize_SuccessfullyInitialized(int id, string name, Player.PlayerType type)
    {
        // Act
        var player = new Player(id, name, type);

        // Assert
        Assert.Equal(id, player.Id);
        Assert.Equal(name, player.Name);
        Assert.Equal(type, player.Type);
        Assert.NotNull(player.Hand);
        Assert.Empty(player.Hand);
    }

    [Theory]
    [InlineData(0, "Alice", Player.PlayerType.Human)]
    [InlineData(1, "Bob", Player.PlayerType.Ai)]
    public void Player_HandInitialization_SuccessfullyInitialized(int id, string name, Player.PlayerType type)
    {
        // Arrange
        var player = new Player(id, name, type);

        // Act
        List<UnoCard> hand = player.Hand;

        // Assert
        Assert.NotNull(hand);
        Assert.Empty(hand);
    }
    
    [Theory]
    [InlineData(0, "Alice", Player.PlayerType.Human)]
    [InlineData(1, "Bob", Player.PlayerType.Ai)]
    public void Player_AddOneCardToEmptyHand_SuccessfullyAdded(int id, string name, Player.PlayerType type)
    {
        // Arrange
        var player = new Player(id, name, type);
        var card = new UnoCard(UnoCard.Color.Red, UnoCard.Value.Zero);

        // Act
        player.Hand.Add(card);

        // Assert
        var onlyOneCardInHand = player.Hand.Count == 1;
        
        Assert.True(onlyOneCardInHand);
        Assert.Contains(card, player.Hand);
    }
    
    [Theory]
    [InlineData(0, "Alice", Player.PlayerType.Human)]
    [InlineData(1, "Bob", Player.PlayerType.Ai)]
    public void Player_RemoveOneCardFromHandWithOneCard_SuccessfullyRemoved(int id, string name, Player.PlayerType type)
    {
        // Arrange
        var player = new Player(id, name, type);
        var card = new UnoCard(UnoCard.Color.Red, UnoCard.Value.Zero);
        player.Hand.Add(card);

        // Act
        player.Hand.Remove(card);

        // Assert
        Assert.Empty(player.Hand);
    }
    
    [Theory]
    [InlineData(0, "Alice", Player.PlayerType.Human)]
    [InlineData(1, "Bob", Player.PlayerType.Ai)]
    public void Player_RemoveNonExistingInHandCardFromHandWithOneCard_NoChangesInHand(int id, string name, Player.PlayerType type)
    {
        // Arrange
        var player = new Player(id, name, type);
        var cardInHand = new UnoCard(UnoCard.Color.Red, UnoCard.Value.Zero);
        var cardNotInHand = new UnoCard(UnoCard.Color.Green, UnoCard.Value.Seven);
        player.Hand.Add(cardInHand);

        // Act
        player.Hand.Remove(cardNotInHand);

        // Assert
        Assert.Single(player.Hand);
        Assert.Contains(cardInHand, player.Hand);
    }
    
    [Fact]
    public void Player_Equals_ReturnsTrueForEqualPlayers()
    {
        // Arrange
        var player1 = new Player(1, "Alice", Player.PlayerType.Human);
        var player2 = new Player(1, "Alice", Player.PlayerType.Human);

        // Act
        var result = player1.Equals(player2);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Player_Equals_ReturnsFalseForDifferentPlayers()
    {
        // Arrange
        var player1 = new Player(1, "Alice", Player.PlayerType.Human);
        var player2 = new Player(2, "Bob", Player.PlayerType.Human);

        // Act
        var result = player1.Equals(player2);

        // Assert
        Assert.False(result);
    }

}