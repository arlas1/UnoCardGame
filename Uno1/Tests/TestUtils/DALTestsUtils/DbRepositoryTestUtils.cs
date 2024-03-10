using DAL;
using Domain;
using Microsoft.EntityFrameworkCore;
using UnoGameEngine;

namespace Tests.TestUtils.DALTestsUtils;

public static class DbRepositoryTestUtils
{
   public static GameEngine CreateSampleGameEngine()
   {
      var gameEngine = new GameEngine()
      {
         GameState =
         {
            GameDirection = false,
            CurrentPlayerIndex = 0,
            IsColorChosen = false,
            SelectedCardIndex = 0,
            CardColorChoice = 0,
            MaxCardsAmount = 0,
            IsGameStarted = 0,
            IsGameEnded = 0,
            IsConsoleSaved = 0
         }
      };
      gameEngine.GameState.StockPile.Add(new UnoCard(UnoCard.Color.Red, UnoCard.Value.Zero));
      GameEngineTestUtils.LoadAmountOfHumanPlayers(3, gameEngine);

      return gameEngine;
   }

   public static async Task CleanUpTheDb(AppDbContext dbContext)
   {
      var lastGameStateId = await dbContext.GameStates
         .OrderByDescending(gs => gs.Id)
         .Select(gs => gs.Id)
         .FirstOrDefaultAsync();

      
      var lastGameState = await dbContext.GameStates.FindAsync(lastGameStateId);
      if (lastGameState != null)
      {
         dbContext.GameStates.Remove(lastGameState);
      }

      await RemoveEntitiesByGameStateId(dbContext, lastGameStateId);
      
      await dbContext.SaveChangesAsync();
   }

   private static async Task RemoveEntitiesByGameStateId(AppDbContext dbContext, int gameStateId)
   {
      var handsToRemove = await dbContext.Hands
         .Where(h => h.GameStateId == gameStateId)
         .ToListAsync();
      dbContext.Hands.RemoveRange(handsToRemove);

      var playersToRemove = await dbContext.Players
         .Where(p => p.GameStateId == gameStateId)
         .ToListAsync();
      dbContext.Players.RemoveRange(playersToRemove);

      var stockPilesToRemove = await dbContext.StockPiles
         .Where(sp => sp.GameStateId == gameStateId)
         .ToListAsync();
      dbContext.StockPiles.RemoveRange(stockPilesToRemove);

      var unoDecksToRemove = await dbContext.UnoDecks
         .Where(ud => ud.GameStateId == gameStateId)
         .ToListAsync();
      dbContext.UnoDecks.RemoveRange(unoDecksToRemove);
   }

}