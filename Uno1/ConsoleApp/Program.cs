using Menu;
using Domain;
using DAL;

namespace ConsoleApp;

public static class Program
{
    private static void Main()
    {
        // Start the menu
        Menu.Menu.RunMenu(NewGame, LoadGame);
        return;
    }
    
    private static string? NewGame()
    {
        GameState.UnoDeck.Create();
        GameState.UnoDeck.Shuffle();
        
        // Ask for Players amount
        var numPlayers = Game.PromptForNumberOfPlayers();
        
        // List with all players as objects
        Game.CreatePlayers(numPlayers);
        
        // First stockpile card check
        Game.CheckFirstCard(GameState.UnoDeck, GameState.StockPile);

        // Main game loop
        Game.StartTheGame(numPlayers);
        
        return null;
    }

    private static string? LoadGame()
    {
        Console.WriteLine("Not Implemented Yet!");
        return null;
    }
}