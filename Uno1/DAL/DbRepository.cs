using Domain;
using DAL.DbEntities;
using Microsoft.EntityFrameworkCore;
using UnoGameEngine;

namespace DAL;

public static class DbRepository
{
    public static AppDbContext GetContext()
    {
        var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer("Server=DESKTOP-FEPAJ2M\\MSSQLSERVER01;Database=Uno;Trusted_Connection=True;TrustServerCertificate=True;")
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging()
            .Options;

        return new AppDbContext(contextOptions);
    }
    
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
            PlayersMaxAmount = gameEngine.GameState.MaxPlayersAmount,
            IsGameStarted = gameEngine.GameState.IsGameStarted,
            ConsoleSaved = 1
            
        };

        context.GameStates.Add(gameStateEntity);
        context.SaveChanges();

        foreach (var player in gameEngine.GameState.PlayersList)
        {
            var playerEntity = new DbEntities.Player()
            {
                Name = player.Name,
                Type = (int)player.Type,
                GameStateId = gameStateEntity.Id
            };

            context.Players.Add(playerEntity);
            context.SaveChanges();

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

            context.StockPile.Add(stockPileEntity);
        }

        foreach (var card in gameEngine.GameState.UnoDeck.SerializedCards)
        {
            var unoDeckEntity = new DbEntities.UnoDeck()
            {
                CardColor = (int)card.CardColor,
                CardValue = (int)card.CardValue,
                GameStateId = gameStateEntity.Id
            };

            context.UnoDeck.Add(unoDeckEntity);
        }

        context.SaveChanges();
    }
    
    public static void LoadFromDb(int gameStateId, AppDbContext context, GameEngine gameEngine)
    {
        var gameStateEntity = context.GameStates.FirstOrDefault(g => g.Id == gameStateId);

        if (gameStateEntity != null)
        {
            gameEngine.GameState.GameDirection = gameStateEntity.GameDirection == 1;
            gameEngine.GameState.CurrentPlayerIndex = gameStateEntity.CurrentPlayerIndex;
            gameEngine.GameState.IsColorChosen = gameStateEntity.IsColorChosen == 1;
            gameEngine.GameState.SelectedCardIndex = gameStateEntity.SelectedCardIndex;
            
            // Retrieve players and hands
            var players = context.Players.Where(p => p.GameStateId == gameStateEntity.Id).ToList();
            
            gameEngine.GameState.PlayersList.Clear();
            foreach (var playerEntity in players)
            {
                var player = new Domain.Player(playerEntity.Id, playerEntity.Name,
                    (Domain.Player.PlayerType)playerEntity.Type);
                
                var cardsInHand = context.Hands
                    .Where(h => h.GameStateId == gameStateEntity.Id && h.PlayerId == playerEntity.Id)
                    .Select(h => new UnoCard((UnoCard.Color)h.CardColor, (UnoCard.Value)h.CardValue))
                    .ToList();
                
                player.Hand.AddRange(cardsInHand);

                gameEngine.GameState.PlayersList.Add(player);
            }
            
            // Retrieve stockpile cards
            var stockPileCards = context.StockPile
                .Where(s => s.GameStateId == gameStateEntity.Id)
                .Select(s => new UnoCard((UnoCard.Color)s.CardColor, (UnoCard.Value)s.CardValue))
                .ToList();
            
            gameEngine.GameState.StockPile.Clear();
            gameEngine.GameState.StockPile.AddRange(stockPileCards);
            
            // Retrieve deck cards
            var unoDeckCards = context.UnoDeck
                .Where(u => u.GameStateId == gameStateEntity.Id)
                .Select(u => new UnoCard((UnoCard.Color)u.CardColor, (UnoCard.Value)u.CardValue))
                .ToList();
            
            gameEngine.GameState.UnoDeck.Clear();
            foreach (var card in unoDeckCards)
            {
                gameEngine.GameState.UnoDeck.AddCardToDeck(card);
            }
        }
    }
        
}