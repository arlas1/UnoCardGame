using DAL;
using UnoGameEngine;
using Microsoft.EntityFrameworkCore;

namespace ConsoleUI;

public class GameSetup
{
    public static void NewGame(GameEngine gameEngine)
    {
        gameEngine.DeleteCardWithValueToAvoid(GameConfiguration.PromptForCardValueToAvoid());
        
        var numPlayers = GameConfiguration.PromptForNumberOfPlayers();
        gameEngine.GameState.PlayersMaxAmount = numPlayers;

        GameConfiguration.CreatePlayers(numPlayers, gameEngine);

        gameEngine.CheckFirstCardInGame();
        
        var gameController = new GameController(gameEngine);
        gameEngine.GameState.IsGameStarted = 1;
        
        gameController.Run();
    }
    
    public static void LoadGameJson(GameEngine gameEngine)
    {
        var jsonFolderPath = @"C:\Users\lasim\RiderProjects\icd0008-23f\Uno1\DAL\JsonSaves/";

        var savedGames = Directory.GetFiles(jsonFolderPath, "*.json");
        if (savedGames.Length == 0)
        {
            Console.WriteLine("No saved games found.");
            return;
        }

        int selectedGameIndex = 0; // Select by default first element

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

        JsonRepository.LoadFromJson(jsonString, gameEngine);
        
        // Start the selected game
        var gameController = new GameController(gameEngine);
        gameController.Run();
    }
    
    public static void LoadGameDb(GameEngine gameEngine)
    {
        var context = DbRepository.GetContext();
        context.Database.Migrate();
        
        var savedGames = context.GameStates.ToList();
        if (savedGames.Count == 0)
        {
            Console.WriteLine("No saved games found in the database.");
            return;
        }

        int selectedGameIndex = 0; // Select by default first element

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
        
        // Load the selected game
        var selectedGameState = savedGames[selectedGameIndex];
        DbRepository.LoadFromDb(selectedGameState.Id, context, gameEngine);
        
        // Start the selected game
        var gameController = new GameController(gameEngine);
        gameController.Run();
    }
    
}