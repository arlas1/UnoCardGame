﻿using Domain;

namespace UnoGameEngine;

public class GameEngine
{
    public GameState GameState { get; set; } = new();
    
    public GameEngine()
    {
        GameState.UnoDeck.Create();
        GameState.UnoDeck.Shuffle();
    }

    public void DeleteCardsWithValueToAvoid(UnoCard.Value? valueToAvoid)
    {
        GameState.UnoDeck.RemoveCardsWithValue(valueToAvoid);
    }
    
    public List<UnoCard> GetOneHand()
    {
        List<UnoCard> listOfCards = [];
        for (var j = 0; j < GameState.MaxCardsAmount; j++)
        {
            var drawnCard = GameState.UnoDeck.DrawCard();
            listOfCards.Add(drawnCard);
        }

        return listOfCards;
    }
    
    public UnoCard CheckFirstCardInGame()
    {
        var isValid = false;
        UnoCard initialTakenCard = null!;

        while (!isValid)
        {
            var initialCard = GameState.UnoDeck.DrawCard();
            initialTakenCard = initialCard;
            
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
                GameState.StockPile.Add(initialCard);
                break;
            }
        }

        return initialTakenCard;
    }
    
    public bool IsValidCardPlay(UnoCard card)
    {
        if (GameState.StockPile.Last().CardColor == UnoCard.Color.Wild &&
            GameState.StockPile.Last().CardValue == UnoCard.Value.Wild)
        {
            return card.CardColor == GameState.CardColorChoice;
        }

        return card.CardColor == GameState.StockPile.Last().CardColor ||
                card.CardValue == GameState.StockPile.Last().CardValue ||
                UnoCard.Color.Wild == GameState.StockPile.Last().CardColor ||
                card.CardColor == UnoCard.Color.Wild;
    }
    
    public void GetNextPlayerId(int currentPlayerId)
    {
        if (GameState.StockPile.Last().CardValue == UnoCard.Value.Skip)
        {
            if (!GameState.GameDirection)
            {
                // Move forward if skip
                GameState.CurrentPlayerIndex = (currentPlayerId + 2) % GameState.PlayersList.Count;

            }
            else
            {
                // Move backward if skip
                GameState.CurrentPlayerIndex = (currentPlayerId - 2 + GameState.PlayersList.Count) % GameState.PlayersList.Count;
            }
        }
        else
        {
            if (!GameState.GameDirection)
            {
                // Move forward
                GameState.CurrentPlayerIndex = (currentPlayerId + 1) % GameState.PlayersList.Count;
            }
            else
            {
                // Move backward
                GameState.CurrentPlayerIndex = (currentPlayerId - 1 + GameState.PlayersList.Count) % GameState.PlayersList.Count;
            }
        }
    }
    
    public void SubmitPlayerCard(int playerId,UnoCard card)
    {
        switch (card.CardValue)
        {
            case UnoCard.Value.Reverse:
                GameState.GameDirection = !GameState.GameDirection;
                break;
            case UnoCard.Value.DrawTwo:
            {
                var nextPlayerId = (playerId + 1) % GameState.PlayersList.Count;

                if (!GameState.GameDirection)
                {
                    DrawTwoCards(nextPlayerId);
                }
                else
                {
                    DrawTwoCards((playerId - 1 + GameState.PlayersList.Count) % GameState.PlayersList.Count);
                }
                
                break;
            }
            case UnoCard.Value.WildFour:
            {
                var nextPlayerId = (playerId + 1) % GameState.PlayersList.Count;

                if (!GameState.GameDirection)
                {
                    DrawFourCards(nextPlayerId);
                }
                else
                {
                    DrawFourCards((playerId - 1 + GameState.PlayersList.Count) % GameState.PlayersList.Count);
                }
                
                break;
            }
        }
    }
    
    private void DrawTwoCards(int playerId)
    {
        GameState.PlayersList[playerId].Hand.Add(GameState.UnoDeck.DrawCard());
        GameState.PlayersList[playerId].Hand.Add(GameState.UnoDeck.DrawCard());
    }
    
    private void DrawFourCards(int playerId)
    {
        for (int i = 0; i < 4; i++)
        {
            GameState.PlayersList[playerId].Hand.Add(GameState.UnoDeck.DrawCard());
        }
    }

    public GameStateCopy GetGameStateCopy()
    {
        return new GameStateCopy
        {
            GameDirection = GameState.GameDirection,
            CurrentPlayerIndex = GameState.CurrentPlayerIndex,
            UnoDeck = GameState.UnoDeck,
            StockPile = GameState.StockPile,
            PlayersList = GameState.PlayersList,
            CardColorChoice = GameState.CardColorChoice,
            IsColorChosen = GameState.IsColorChosen,
            SelectedCardIndex = GameState.SelectedCardIndex
        };
    }
}