
using Domain;
using DAL.DbEntities;
using Microsoft.EntityFrameworkCore;
using UnoGameEngine;

namespace DAL;

public static class DbRepository
{
    
    // Get context from the db
    public static AppDbContext GetContext()
    {
        var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer("Server=DESKTOP-FEPAJ2M\\MSSQLSERVER01;Database=Uno;Trusted_Connection=True;TrustServerCertificate=True;")
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging()
            .Options;

        return new AppDbContext(contextOptions);
        
    }


    // Save to the db
    public static void SaveIntoDb(GameEngine gameEngine)
    {
        var context = GetContext();
        context.Database.Migrate();
        
        var gameStateEntity = new DbEntities.GameState()
        {
            GameDirection = gameEngine.GameState.GameDirection ? 1 : 0,
            CurrentPlayerIndex = gameEngine.GameState.CurrentPlayerIndex,
            IsColorChosen = gameEngine.GameState.IsColorChosen ? 1 : 0,
            SelectedCardIndex = gameEngine.GameState.SelectedCardIndex,
            PlayersMaxAmount = gameEngine.GameState.PlayersMaxAmount,
            IsGameStarted = gameEngine.GameState.IsGameStarted,
            ConsoleSaved = 1
            
        };

        context.GameStates.Add(gameStateEntity);
        context.SaveChanges(); // Save changes to get the ID

        foreach (var player in gameEngine.GameState.PlayersList)
        {
            var playerEntity = new DbEntities.Player()
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

        foreach (var card in gameEngine.GameState.StockPile)
        {
            var stockPileEntity = new StockPile()
            {
                CardColor = (int)card.CardColor,
                CardValue = (int)card.CardValue,
                GameStateId = gameStateEntity.Id
            };

            context.StockPiles.Add(stockPileEntity);
        }

        foreach (var card in gameEngine.GameState.UnoDeck.SerializedCards)
        {
            var unoDeckEntity = new DbEntities.UnoDeck()
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
    public static void LoadFromDb(int gameStateId, AppDbContext context, GameEngine gameEngine)
    {
        
        var gameStateEntity = context.GameStates.FirstOrDefault(g => g.Id == gameStateId);

        if (gameStateEntity != null)
        {
            gameEngine.GameState.GameDirection = gameStateEntity.GameDirection == 1;
            gameEngine.GameState.CurrentPlayerIndex = gameStateEntity.CurrentPlayerIndex;
            gameEngine.GameState.IsColorChosen = gameStateEntity.IsColorChosen == 1;
            gameEngine.GameState.SelectedCardIndex = gameStateEntity.SelectedCardIndex;
            
            
            var players = context.Players.Where(p => p.GameStateId == gameStateEntity.Id).ToList();

            gameEngine.GameState.PlayersList.Clear();
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

                gameEngine.GameState.PlayersList.Add(player);
            }

            
            var stockPileCards = context.StockPiles
                .Where(s => s.GameStateId == gameStateEntity.Id)
                .ToList();

            gameEngine.GameState.StockPile.Clear();
            foreach (var cardEntity in stockPileCards)
            {
                var card = new UnoCard((UnoCard.Color)cardEntity.CardColor,
                    (UnoCard.Value)cardEntity.CardValue);
                gameEngine.GameState.StockPile.Add(card);
            }

            
            var unoDeckCards = context.UnoDecks
                .Where(u => u.GameStateId == gameStateEntity.Id)
                .ToList();

            gameEngine.GameState.UnoDeck.Clear();
            foreach (var cardEntity in unoDeckCards)
            {
                var card = new UnoCard((UnoCard.Color)cardEntity.CardColor,
                    (UnoCard.Value)cardEntity.CardValue);
                gameEngine.GameState.UnoDeck.AddCardToDeck(card);
            }
        }
    }

}