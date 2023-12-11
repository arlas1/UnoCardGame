
using Domain;
using DAL.DbEntities;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public static class DbRepository
{
    
    // Get context from the db
    public static AppDbContext GetContext()
    {
        var dbFilePath = @"C:\Users\lasim\RiderProjects\icd0008-23f\Uno1\DAL\UnoDb.db";
        var connectionString = $"Data Source={dbFilePath};";

        var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connectionString)
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging()
            .Options;

        return new AppDbContext(contextOptions);
        
    }


    // Save to the db
    public static void SaveIntoDb()
    {
        var context = GetContext();
        context.Database.Migrate();
        
        var gameStateEntity = new DAL.DbEntities.GameState()
        {
            GameDirection = Domain.GameState.GameDirection ? 1 : 0,
            CurrentPlayerIndex = Domain.GameState.CurrentPlayerIndex,
            IsColorChosen = Domain.GameState.IsColorChosen ? 1 : 0,
            SelectedCardIndex = Domain.GameState.SelectedCardIndex
        };

        context.GameStates.Add(gameStateEntity);
        context.SaveChanges(); // Save changes to get the ID

        foreach (var player in Domain.GameState.PlayersList)
        {
            var playerEntity = new DAL.DbEntities.Player()
            {
                Name = player.Name,
                Type = (int)player.Type,
                GameStateId = gameStateEntity.Id
            };

            context.Players.Add(playerEntity);
            context.SaveChanges(); // Save changes to get the ID

            foreach (var card in player.Hand)
            {
                var handEntity = new Hand()
                {
                    CardColor = (int)card.CardColor,
                    CardValue = (int)card.CardValue,
                    PlayerId = playerEntity.Id,
                    GameStateId = gameStateEntity.Id
                };

                context.Hands.Add(handEntity);
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

            context.StockPiles.Add(stockPileEntity);
        }

        foreach (var card in Domain.GameState.UnoDeck.SerializedCards)
        {
            var unoDeckEntity = new DAL.DbEntities.UnoDeck()
            {
                CardColor = (int)card.CardColor,
                CardValue = (int)card.CardValue,
                GameStateId = gameStateEntity.Id
            };

            context.UnoDecks.Add(unoDeckEntity);
        }

        context.SaveChanges();
    }

    // Load from the db
    public static void LoadFromDb(int gameStateId, AppDbContext context)
    {
        
        var gameStateEntity = context.GameStates.FirstOrDefault(g => g.Id == gameStateId);

        if (gameStateEntity != null)
        {
            Domain.GameState.GameDirection = gameStateEntity.GameDirection == 1;
            Domain.GameState.CurrentPlayerIndex = gameStateEntity.CurrentPlayerIndex;
            Domain.GameState.IsColorChosen = gameStateEntity.IsColorChosen == 1;
            Domain.GameState.SelectedCardIndex = gameStateEntity.SelectedCardIndex;
            
            
            var players = context.Players.Where(p => p.GameStateId == gameStateEntity.Id).ToList();

            Domain.GameState.PlayersList.Clear();
            foreach (var playerEntity in players)
            {
                var player = new Domain.Player(playerEntity.Id, playerEntity.Name,
                    (Domain.Player.PlayerType)playerEntity.Type);

                // By gamestate and player id
                var cardsInHand = context.Hands
                    .Where(h => h.GameStateId == gameStateEntity.Id && h.PlayerId == playerEntity.Id)
                    .ToList();

                
                foreach (var cardEntity in cardsInHand)
                {
                    var card = new UnoCard((UnoCard.Color)cardEntity.CardColor,
                        (UnoCard.Value)cardEntity.CardValue);
                    player.Hand.Add(card);
                }

                Domain.GameState.PlayersList.Add(player);
            }

            
            var stockPileCards = context.StockPiles
                .Where(s => s.GameStateId == gameStateEntity.Id)
                .ToList();

            Domain.GameState.StockPile.Clear();
            foreach (var cardEntity in stockPileCards)
            {
                var card = new UnoCard((UnoCard.Color)cardEntity.CardColor,
                    (UnoCard.Value)cardEntity.CardValue);
                Domain.GameState.StockPile.Add(card);
            }

            
            var unoDeckCards = context.UnoDecks
                .Where(u => u.GameStateId == gameStateEntity.Id)
                .ToList();

            Domain.GameState.UnoDeck.Clear();
            foreach (var cardEntity in unoDeckCards)
            {
                var card = new UnoCard((UnoCard.Color)cardEntity.CardColor,
                    (UnoCard.Value)cardEntity.CardValue);
                Domain.GameState.UnoDeck.AddCardToDeck(card);
            }
        }
    }

}