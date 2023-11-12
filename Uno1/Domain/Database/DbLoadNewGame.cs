using Domain;
using Domain.Database;
using Microsoft.EntityFrameworkCore;
using GameState = Domain.GameState;

namespace Domain.Database;

public static class DbLoadNewGame
{
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
    
    
    public static string? LoadNewGameDb()
    {
        var context = GetContext();
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

                Console.WriteLine($"{i + 1}. Game ID: {savedGames[i].Id}");

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
        DbOptions.LoadFromDb(selectedGameState.Id);

        // Continue the game from the loaded state
        Game.StartTheGame(Domain.GameState.PlayersList.Count);

        return null;
    }

    
}