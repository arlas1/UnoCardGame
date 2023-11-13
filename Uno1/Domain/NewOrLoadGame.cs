namespace Domain;

public static class NewOrLoadGame
{
    public static string? NewGame()
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

        return null!;
    }

    public static string? LoadGameJson()
    {
        var jsonFolderPath = @"C:\Users\lasim\RiderProjects\icd0008-23f\Uno1\Domain\Database\JsonSaves/";

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

                Console.WriteLine($"{i + 1}. {Path.GetFileNameWithoutExtension(savedGames[i])}");

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
        GameState.LoadFromJson(jsonString);

        // Continue the game from the loaded state
        Game.StartTheGame(GameState.PlayersList.Count);

        return null!;
    }
}