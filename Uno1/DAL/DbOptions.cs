using GameState = Domain.Database.GameState;
using Player = Domain.Database.Player;
using Hand = Domain.Database.Hand;
using UnoDeck = Domain.Database.UnoDeck;
using StockPile = Domain.Database.StockPile;

namespace DAL;

public class DbOptions
{
    public static AppDbContext Context;

    public DbOptions(AppDbContext context)
    {
        Context = context;
    }

    // Save to the db
    public static void SaveIntoDb()
    {
        var gameStateEntity = new GameState()
        {
            GameDirection = Domain.GameState.GameDirection ? 1 : 0,
            CurrentPlayerIndex = Domain.GameState.CurrentPlayerIndex,
            IsColorChosen = Domain.GameState.IsColorChosen ? 1 : 0,
            SelectedCardIndex = Domain.GameState.SelectedCardIndex
        };

        Context.GameStates.Add(gameStateEntity);
        Context.SaveChanges(); // Save changes to get the ID

        foreach (var player in Domain.GameState.PlayersList)
        {
            var playerEntity = new Player()
            {
                Name = player.Name,
                Type = (int)player.Type,
                GameStateId = gameStateEntity.Id
            };

            Context.Players.Add(playerEntity);
            Context.SaveChanges(); // Save changes to get the ID

            foreach (var card in player.Hand)
            {
                var handEntity = new Hand()
                {
                    CardColor = (int)card.CardColor,
                    CardValue = (int)card.CardValue,
                    PlayerId = playerEntity.Id,
                    GameStateId = gameStateEntity.Id
                };

                Context.Hands.Add(handEntity);
            }
        }

        foreach (var card in Domain.GameState.StockPile)
        {
            var stockPileEntity = new StockPile()
            {
                CardColor = (int)card.CardColor,
                CardValue = (int)card.CardValue,
                GameStateId = gameStateEntity.Id
            };

            Context.StockPiles.Add(stockPileEntity);
        }

        foreach (var card in Domain.GameState.UnoDeck.SerializedCards)
        {
            var unoDeckEntity = new UnoDeck()
            {
                CardColor = (int)card.CardColor,
                CardValue = (int)card.CardValue,
                GameStateId = gameStateEntity.Id
            };

            Context.UnoDecks.Add(unoDeckEntity);
        }

        Context.SaveChanges();
    }

    // Load to the db
    public static void LoadFromDb(int gameStateId)
    {
        // Fetch the game state from the database based on the provided ID
        var gameStateEntity = Context.GameStates.FirstOrDefault(g => g.Id == gameStateId);

        if (gameStateEntity != null)
        {
            // Create a new game state object
            Domain.GameState.GameDirection = gameStateEntity.GameDirection == 1;
            Domain.GameState.CurrentPlayerIndex = gameStateEntity.CurrentPlayerIndex;
            Domain.GameState.IsColorChosen = gameStateEntity.IsColorChosen == 1;
            Domain.GameState.SelectedCardIndex = gameStateEntity.SelectedCardIndex;

            // Fetch players associated with the game state
            var players = Context.Players.Where(p => p.GameStateId == gameStateEntity.Id).ToList();

            // Populate the player list in the game state
            Domain.GameState.PlayersList.Clear();
            foreach (var playerEntity in players)
            {
                var player = new Domain.Player(playerEntity.Id, playerEntity.Name,
                    (Domain.Player.PlayerType)playerEntity.Type);

                // Fetch cards in the player's hand
                var cardsInHand = Context.Hands
                    .Where(h => h.GameStateId == gameStateEntity.Id && h.PlayerId == playerEntity.Id)
                    .ToList();

                // Populate the player's hand
                foreach (var cardEntity in cardsInHand)
                {
                    var card = new Domain.UnoCard((Domain.UnoCard.Color)cardEntity.CardColor,
                        (Domain.UnoCard.Value)cardEntity.CardValue);
                    player.Hand.Add(card);
                }

                Domain.GameState.PlayersList.Add(player);
            }

            // Fetch cards in the stock pile
            var stockPileCards = Context.StockPiles
                .Where(s => s.GameStateId == gameStateEntity.Id)
                .ToList();

            // Populate the stock pile in the game state
            Domain.GameState.StockPile.Clear();
            foreach (var cardEntity in stockPileCards)
            {
                var card = new Domain.UnoCard((Domain.UnoCard.Color)cardEntity.CardColor,
                    (Domain.UnoCard.Value)cardEntity.CardValue);
                Domain.GameState.StockPile.Add(card);
            }

            // Fetch cards in the Uno deck
            var unoDeckCards = Context.UnoDecks
                .Where(u => u.GameStateId == gameStateEntity.Id)
                .ToList();

            // Populate the Uno deck in the game state
            Domain.GameState.UnoDeck.Clear();
            foreach (var cardEntity in unoDeckCards)
            {
                var card = new Domain.UnoCard((Domain.UnoCard.Color)cardEntity.CardColor,
                    (Domain.UnoCard.Value)cardEntity.CardValue);
                Domain.GameState.UnoDeck.AddCardToDeck(card);
            }
        }
    }

}