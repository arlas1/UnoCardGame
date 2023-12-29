using DAL;
using UnoGameEngine;
using Microsoft.EntityFrameworkCore;

namespace ConsoleUI;

public class GameSetup
{
    public static Action NewGame(GameEngine gameEngine)
    {
        
        gameEngine.DeleteCardWithValueToAvoid(GameConfiguration.PromptForCardValueToAvoid());
        
        // Ask for Players amount
        var numPlayers = GameConfiguration.PromptForNumberOfPlayers();

        // List with all players as objects
        GameConfiguration.CreatePlayers(numPlayers, gameEngine);

        // First stockpile card check
        var asd = gameEngine.CheckFirstCardInGame();
        
        var gameController = new GameController(gameEngine);

        // Main game loop
        gameController.Run();

        return null!;
    }

    
    public static Action LoadGameJson(GameEngine gameEngine)
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
        JsonRepository.LoadFromJson(jsonString, gameEngine);
        
        var gameController = new GameController(gameEngine);

        // Main game loop
        gameController.Run();

        return null!;
    }
    
    
    public static Action LoadGameDb(GameEngine gameEngine)
    {
        var context = DbRepository.GetContext();
        context.Database.Migrate();
        
        // Get the list of saved games from the database
        var savedGames = context.GameStates.ToList();
        if (savedGames.Count == 0)
        {
            Console.WriteLine("No saved games found in the database.");
            return null!;
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
        DbRepository.LoadFromDb(selectedGameState.Id, context, gameEngine);
        
        var gameController = new GameController(gameEngine);

        // Continue the game from the loaded state
        gameController.Run();

        return null!;
    }
    
    
}