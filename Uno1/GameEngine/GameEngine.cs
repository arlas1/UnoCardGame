using Domain;

namespace GameEngine;

public static class GameEngine
{
    
    public static void CheckFirstCardInGame(UnoDeck unoDeck, List<UnoCard> stockPile)
    {
        var isValid = false;

        while (!isValid)
        {
            var initialCard = unoDeck.DrawCard();

            // Invalid card types for the start
            if (initialCard.CardValue is
                UnoCard.Value.Wild or
                UnoCard.Value.DrawTwo or
                UnoCard.Value.WildFour or
                UnoCard.Value.Skip or
                UnoCard.Value.Reverse)
            {
                isValid = false;
            }
            else
            {
                stockPile.Add(initialCard);
                break;
            }
        }
    }
    
    
    public static bool IsValidCardPlay(UnoCard card)
    {
        if (GameState.StockPile.Last().CardColor == UnoCard.Color.Wild &&
            GameState.StockPile.Last().CardValue == UnoCard.Value.Wild)
        {
            return card.CardColor == GameState.CardColorChoice;
        }

        return (card.CardColor == GameState.StockPile.Last().CardColor ||
                card.CardValue == GameState.StockPile.Last().CardValue ||
                UnoCard.Color.Wild == GameState.StockPile.Last().CardColor ||
                card.CardColor == UnoCard.Color.Wild);
    }
    
    
    public static void GetNextPlayerId(int playerId, int numPlayers)
    {
        if ((GameState.StockPile.Last().CardValue == UnoCard.Value.Skip))
        {
            if (!GameState.GameDirection)
            {
                // Move forward if skip
                GameState.CurrentPlayerIndex = (playerId + 2) % numPlayers;

            }
            else
            {
                // Move backward if skip
                GameState.CurrentPlayerIndex = (playerId - 2 + numPlayers) % numPlayers;
            }
        }
        else
        {
            if (!GameState.GameDirection)
            {
                // Move forward
                GameState.CurrentPlayerIndex = (playerId + 1) % numPlayers;
            }
            else
            {
                // Move backward
                GameState.CurrentPlayerIndex = (playerId - 1 + numPlayers) % numPlayers;
            }
        }
    }
    
    
    public static void SubmitPlayerCard(UnoCard card, int playerId, int numPlayers)
    {
        if (card.CardValue == UnoCard.Value.Reverse)
        {
            GameState.GameDirection = !GameState.GameDirection;
        }

        if (card is { CardColor: UnoCard.Color.Wild, CardValue: UnoCard.Value.Wild })
        {
            var currentPlayer = GameState.PlayersList[GameState.CurrentPlayerIndex];

            if (currentPlayer.Type == Player.PlayerType.Human)
            {
                Game.PromptForWildCardColorHuman();
            }
            else
            {
                Game.PromptForWildCardColorAi();
            }
            
        }

        if (card.CardValue == UnoCard.Value.DrawTwo)
        {
            var nextPlayerId = (playerId + 1) % numPlayers;

            if (!GameState.GameDirection)
            {
                DrawTwoCards(nextPlayerId);
            }
            else
            {
                DrawTwoCards((playerId - 1 + numPlayers) % numPlayers);
            }
        }

        if (card is { CardColor: UnoCard.Color.Wild, CardValue: UnoCard.Value.WildFour })
        {
            var nextPlayerId = (playerId + 1) % numPlayers;

            if (!GameState.GameDirection)
            {
                DrawFourCards(nextPlayerId);
            }
            else
            {
                DrawFourCards((playerId - 1 + numPlayers) % numPlayers);
            }
        }
    }

    
    private static void DrawTwoCards(int playerId)
    {
        GameState.PlayersList[playerId].Hand.Add(GameState.UnoDeck.DrawCard());
        GameState.PlayersList[playerId].Hand.Add(GameState.UnoDeck.DrawCard());
    }
    
    
    private static void DrawFourCards(int playerId)
    {
        for (int i = 0; i < 4; i++)
        {
            GameState.PlayersList[playerId].Hand.Add(GameState.UnoDeck.DrawCard());
        }
    }

    
}