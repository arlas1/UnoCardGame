using Microsoft.EntityFrameworkCore;
namespace Domain.Database;

public static class DbOptions
{
    
    // Get context from the db
    public static AppDbContext GetContext()
    {
        var connectionString =
            "Server=barrel.itcollege.ee;User Id=student;Password=Student.Pass.1;Database=student_arlasi;MultipleActiveResultSets=true";

        var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(connectionString)
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging()
            .Options;
        return new AppDbContext(contextOptions);
        
    }


    // Save to the db
    public static void SaveIntoDb()
    {
        var context = GetContext();
        
        var gameStateEntity = new GameState()
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
            var playerEntity = new Player()
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
            var unoDeckEntity = new UnoDeck()
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
    public static void LoadFromDb(int gameStateId)
    {
        var context = GetContext();
        
        // Get the game state from the database based on the provided ID
        var gameStateEntity = context.GameStates.FirstOrDefault(g => g.Id == gameStateId);

        if (gameStateEntity != null)
        {
            // Create a new game state object
            Domain.GameState.GameDirection = gameStateEntity.GameDirection == 1;
            Domain.GameState.CurrentPlayerIndex = gameStateEntity.CurrentPlayerIndex;
            Domain.GameState.IsColorChosen = gameStateEntity.IsColorChosen == 1;
            Domain.GameState.SelectedCardIndex = gameStateEntity.SelectedCardIndex;

            // Get players associated with the game state
            var players = context.Players.Where(p => p.GameStateId == gameStateEntity.Id).ToList();

            // Populate the player list in the game state
            Domain.GameState.PlayersList.Clear();
            foreach (var playerEntity in players)
            {
                var player = new Domain.Player(playerEntity.Id, playerEntity.Name,
                    (Domain.Player.PlayerType)playerEntity.Type);

                // Get cards in the player's hand
                var cardsInHand = context.Hands
                    .Where(h => h.GameStateId == gameStateEntity.Id && h.PlayerId == playerEntity.Id)
                    .ToList();

                // Populate the player's hand
                foreach (var cardEntity in cardsInHand)
                {
                    var card = new UnoCard((UnoCard.Color)cardEntity.CardColor,
                        (UnoCard.Value)cardEntity.CardValue);
                    player.Hand.Add(card);
                }

                Domain.GameState.PlayersList.Add(player);
            }

            // Get cards in the stock pile
            var stockPileCards = context.StockPiles
                .Where(s => s.GameStateId == gameStateEntity.Id)
                .ToList();

            // Populate the stock pile in the game state
            Domain.GameState.StockPile.Clear();
            foreach (var cardEntity in stockPileCards)
            {
                var card = new Domain.UnoCard((UnoCard.Color)cardEntity.CardColor,
                    (UnoCard.Value)cardEntity.CardValue);
                Domain.GameState.StockPile.Add(card);
            }

            // Get cards in the Uno deck
            var unoDeckCards = context.UnoDecks
                .Where(u => u.GameStateId == gameStateEntity.Id)
                .ToList();

            // Populate the Uno deck in the game state
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