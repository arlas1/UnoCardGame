using DAL;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace ConsoleUI;

public static class GameSetup
{
    public static string? NewGame()
    {
        // UnoDeck.PromptAvoidValues();
        GameState.UnoDeck.Create();
        GameState.UnoDeck.Shuffle();

        // Ask for Players amount
        var numPlayers = Game.PromptForNumberOfPlayers();

        // List with all players as objects
        Game.CreatePlayers(numPlayers);

        // First stockpile card check
        GameEngine.GameEngine.CheckFirstCardInGame(GameState.UnoDeck, GameState.StockPile);

        // Main game loop
        GameController.StartTheGame(numPlayers);

        return null!;
    }

    
    public static string? LoadGameJson()
    {
        var jsonFolderPath = @"C:\Users\lasim\RiderProjects\icd0008-23f\Uno1\DAL\JsonSaves/";

        // Display the list of available saved games
        var savedGames = Directory.GetFiles(jsonFolderPath, "*.json");
        if (savedGames.Length == 0)
        {
            Console.WriteLine("No saved games found.");
            return null!;
        }

        int selectedGameIndex = 0; // Default selection to the first game

        ConsoleKeyInfo key;

        do
        {
            Console.Clear();
            Console.WriteLine("Select a game to load:");

            for (int i = 0; i < savedGames.Length; i++)
            {
                if (i == selectedGameIndex)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }

                Console.WriteLine($"Game ID: {Path.GetFileNameWithoutExtension(savedGames[i])}");

                Console.ResetColor();
            }

            key = Console.ReadKey(true);

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    selectedGameIndex = (selectedGameIndex - 1 + savedGames.Length) % savedGames.Length;
                    break;
                case ConsoleKey.DownArrow:
                    selectedGameIndex = (selectedGameIndex + 1) % savedGames.Length;
                    break;
            }

        } while (key.Key != ConsoleKey.Enter);

        // Load the selected game
        var selectedGamePath = savedGames[selectedGameIndex];
        var jsonString = File.ReadAllText(selectedGamePath);

        // Update the game state
        JsonRepository.LoadFromJson(jsonString);

        // Continue the game from the loaded state
        GameController.StartTheGame(GameState.PlayersList.Count);

        return null!;
    }
    
    
    public static string? LoadGameDb()
    {
        var context = DbRepository.GetContext();
        context.Database.Migrate();
        
        // Get the list of saved games from the database
        var savedGames = context.GameStates.ToList();
        if (savedGames.Count == 0)
        {
            Console.WriteLine("No saved games found in the database.");
            return null;
        }

        int selectedGameIndex = 0; // Default selection to the first game

        ConsoleKeyInfo key;

        do
        {
            Console.Clear();
            Console.WriteLine("Select a game to load:");

            for (int i = 0; i < savedGames.Count; i++)
            {
                if (i == selectedGameIndex)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }

                Console.WriteLine($"Game ID: {savedGames[i].Id}");

                Console.ResetColor();
            }

            key = Console.ReadKey(true);

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    selectedGameIndex = (selectedGameIndex - 1 + savedGames.Count) % savedGames.Count;
                    break;
                case ConsoleKey.DownArrow:
                    selectedGameIndex = (selectedGameIndex + 1) % savedGames.Count;
                    break;
            }

        } while (key.Key != ConsoleKey.Enter);

        // Load the selected game from the database
        var selectedGameState = savedGames[selectedGameIndex];

        // Update the game state from the loaded database state
        DbRepository.LoadFromDb(selectedGameState.Id, context);

        // Continue the game from the loaded state
        GameController.StartTheGame(Domain.GameState.PlayersList.Count);

        return null;
    }
    
    
}