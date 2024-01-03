using ConsoleUI;
using UnoGameEngine;
using DAL;

namespace ConsoleApp;

public static class Program
{
    private static void Main()
    {
        // using var dbContext = new AppDbContext();
        //
        // dbContext.GameStates.RemoveRange(dbContext.GameStates);
        // dbContext.Hands.RemoveRange(dbContext.Hands);
        // dbContext.Players.RemoveRange(dbContext.Players);
        // dbContext.StockPiles.RemoveRange(dbContext.StockPiles);
        // dbContext.UnoDecks.RemoveRange(dbContext.UnoDecks);
        //
        // dbContext.SaveChanges();
        
        var gameEngine = new GameEngine();
        
        GameConfiguration.PromptForRepositoryType(gameEngine);
        
        switch (gameEngine.GameState.RepositoryChoice)
        {
            case 1:
                Menu.Menu.RunMenu(() => GameSetup.NewGame(gameEngine), () => GameSetup.LoadGameJson(gameEngine));
                break;
            case 2:
                Menu.Menu.RunMenu(() => GameSetup.NewGame(gameEngine), () => GameSetup.LoadGameDb(gameEngine));
                break;
        }
    }
}