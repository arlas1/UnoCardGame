using System.Text;
using Domain;
using UnoGameEngine;

namespace ConsoleUI;

public static class GameConfiguration
{
    public static void Start()
    {
        var gameEngine = new GameEngine();
        
        PromptForRepositoryType(gameEngine);
        
        switch (gameEngine.GameState.RepositoryChoice)
        {
            case 1:
                Menu.Menu.Run(() => GameSetup.NewGame(gameEngine), () => GameSetup.LoadGameJson(gameEngine));
                break;
            case 2:
                Menu.Menu.Run(() => GameSetup.NewGame(gameEngine), () => GameSetup.LoadGameDb(gameEngine));
                break;
        }
    }
    
    public static void PromptForRepositoryType(GameEngine gameEngine)
    {
        Console.Clear();
        int choice;
        var input = string.Empty;
        
        while (true)
        {
            Console.Write("Which file system to use? Json/Sqlite [1/2]. Press Enter for json: ");
            Console.Write(input);

            var key = Console.ReadKey(intercept: true);
            
            if (key.Key == ConsoleKey.Enter)
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    choice = 1;
                    Console.Clear();
                    break;
                }

                if (int.TryParse(input, out choice) && choice is >= 1 and <= 2)
                {
                    Console.Clear();
                    break;
                }
            }
            else if (key.Key == ConsoleKey.Backspace && input.Length > 0)
            {
                input = input[..^1];
                Console.Write("\b \b"); 
            }
            else if (char.IsDigit(key.KeyChar) && input.Length < 1)
            {
                var enteredDigit = int.Parse(key.KeyChar.ToString());
                if (enteredDigit is >= 1 and <= 2)
                {
                    input += key.KeyChar;
                    Console.Write(key.KeyChar);
                }
            }

            Console.Clear();
        }

        gameEngine.GameState.RepositoryChoice = choice;
    }
    
    public static int PromptForNumberOfPlayers()
    {
        int playerInput;
        var input = string.Empty;

        while (true)
        {
            Console.Write("Enter amount of players [2-7]. Press Enter for 2: ");
            Console.Write(input);

            var key = Console.ReadKey(intercept: true);

            if (key.Key == ConsoleKey.Enter)
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    playerInput = 2;
                    Console.Clear();
                    break;
                }

                if (int.TryParse(input, out playerInput) && playerInput is >= 2 and <= 7)
                {
                    Console.Clear();
                    break;
                }
            }
            else if (key.Key == ConsoleKey.Backspace && input.Length > 0)
            {
                input = input[..^1];
                Console.Write("\b \b"); 
            }
            else if (char.IsDigit(key.KeyChar) && input.Length < 1)
            {
                var enteredDigit = int.Parse(key.KeyChar.ToString());
                if (enteredDigit is >= 2 and <= 7)
                {
                    input += key.KeyChar;
                    Console.Write(key.KeyChar);
                }
            }

            Console.Clear();
        }

        return playerInput;
    }
    
    public static void CreatePlayers(int numPlayers, GameEngine gameEngine)
    {
        var players = new List<Player>();
        var cardsPerPlayer = PromptForInitialCardsAmountPerPlayer();

        for (var i = 0; i < numPlayers; i++)
        {
            var playerName = PromptForPlayerName(i);
            var playerTypeInput = PromptForPlayerType(playerName);
            
            var playerType = playerTypeInput == "a" ? Player.PlayerType.Ai : Player.PlayerType.Human;
            var player = new Player(i, playerName, playerType);
            
            for (var j = 0; j < cardsPerPlayer; j++)
            {
                var drawnCard = gameEngine.GameState.UnoDeck.DrawCard();
                player.Hand.Add(drawnCard);
            }

            players.Add(player);
        }

        gameEngine.GameState.PlayersList = players;
    }
    
    public static void PromptForWildCardColorAi(GameEngine gameEngine)
    {
        var currentPlayer = gameEngine.GameState.PlayersList[gameEngine.GameState.CurrentPlayerIndex];
        
        if (currentPlayer.Type == Player.PlayerType.Ai)
        {
            var randomColor = (UnoCard.Color)new Random().Next(0, 4);
            Console.WriteLine($"{currentPlayer.Name} placed Wild card. Chose color: {randomColor}");
            gameEngine.GameState.CardColorChoice = randomColor;
        }
        
        gameEngine.GameState.IsColorChosen = true;
    }
    
    public static void PromptForWildCardColorHuman(GameEngine gameEngine)
    {
        var selectedIndex = 0;
        ConsoleKeyInfo key;
    
        do
        {
            Console.Clear();
            Console.WriteLine("Choose the color of the Wild card: ");
    
            for (int i = 0; i < 4; i++)
            {
                if (i == selectedIndex)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }
    
                Console.WriteLine($"{i + 1}. {((UnoCard.Color)i).ToString()}");
    
                Console.ResetColor();
            }
    
            key = Console.ReadKey();
    
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = (selectedIndex - 1 + 4) % 4;
                    break;
                case ConsoleKey.DownArrow:
                    selectedIndex = (selectedIndex + 1) % 4;
                    break;
            }
        } while (key.Key != ConsoleKey.Enter);
    
        Console.Clear();
    
        gameEngine.GameState.CardColorChoice = (UnoCard.Color)selectedIndex;
        gameEngine.GameState.IsColorChosen = true;
    }
    
    public static UnoCard.Value PromptForCardValueToAvoid()
    {
        Console.Clear();
        var options = new List<UnoCard.Value> 
        { 
            UnoCard.Value.Reverse,
            UnoCard.Value.Skip,
            UnoCard.Value.DrawTwo,
            UnoCard.Value.WildFour,
            UnoCard.Value.Wild
        };

        var selectedIndex = 0;
        ConsoleKeyInfo key;

        do
        {
            Console.Clear();
            Console.WriteLine("Choose which card value to avoid in the deck: ");

            for (var i = 0; i < options.Count; i++)
            {
                if (i == selectedIndex)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }

                Console.WriteLine($"{i + 1}. {options[i]}");
                Console.ResetColor();
            }

            key = Console.ReadKey();

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = (selectedIndex - 1 + options.Count) % options.Count;
                    break;
                case ConsoleKey.DownArrow:
                    selectedIndex = (selectedIndex + 1) % options.Count;
                    break;
                case ConsoleKey.Enter:
                    Console.Clear();
                    return options[selectedIndex];
            }
        } while (key.Key != ConsoleKey.Enter);

        return default;
    }
    
    private static string PromptForPlayerName(int playerIndex)
    {
        Console.Write($"Enter Player {playerIndex + 1} nickname (Within 20 characters): ");

        var playerNameBuilder = new StringBuilder();

        while (true)
        {
            var key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Enter)
            {
                break;
            }

            if (key.Key == ConsoleKey.Backspace && playerNameBuilder.Length > 0)
            {
                playerNameBuilder.Remove(playerNameBuilder.Length - 1, 1);
                Console.Write("\b \b");
            }
            else if (!char.IsControl(key.KeyChar) && playerNameBuilder.Length < 20)
            {
                playerNameBuilder.Append(key.KeyChar);
                Console.Write(key.KeyChar);
            }

        }

        var playerName = playerNameBuilder.ToString().Trim();
        if (string.IsNullOrWhiteSpace(playerName))
        {
            playerName = $"Player {playerIndex + 1}";
        }

        Console.Clear();

        return playerName;
    }

    private static string PromptForPlayerType(string playerNameInput)
    {
        string playerInput;
        var input = string.Empty;

        while (true)
        {
            Console.WriteLine($"Enter Player's {playerNameInput} type (Human/Ai) [h/a]. Press Enter for human: ");
            Console.Write(input);

            var key = Console.ReadKey(intercept: true);

            if (key.Key == ConsoleKey.Enter)
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    playerInput = "h";
                    Console.Clear();
                    break;
                }
                    
                if (input.Contains('h') || input.Contains('a'))
                {
                    playerInput = input.Trim().ToLower();
                    Console.Clear();
                    break;
                }
            }
            else if (key.Key == ConsoleKey.Backspace && input.Length > 0)
            {
                        
                input = input[..^1];
                Console.Write("\b \b");
            }
            else if (key.KeyChar is 'h' or 'a' && input.Length < 1)
            {
                input += key.KeyChar;
                Console.Write(key.KeyChar);
            }

            Console.Clear();
        }

        return playerInput;
    }
    
    private static int PromptForInitialCardsAmountPerPlayer()
    {
        int choice;
        var input = string.Empty;
        
        while (true)
        {
            Console.Write("Enter initial cards amount per player (2-7). Press Enter for 7: ");
            Console.Write(input);

            var key = Console.ReadKey(intercept: true);
            
            if (key.Key == ConsoleKey.Enter)
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    choice = 7;
                    Console.Clear();
                    break;
                }

                if (int.TryParse(input, out choice) && choice is >= 2 and <= 7)
                {
                    Console.Clear();
                    break;
                }
            }
            else if (key.Key == ConsoleKey.Backspace && input.Length > 0)
            {
                input = input[..^1];
                Console.Write("\b \b");
            }
            else if (char.IsDigit(key.KeyChar) && input.Length < 1)
            {
                var enteredDigit = int.Parse(key.KeyChar.ToString());
                if (enteredDigit is >= 2 and <= 7)
                {
                    input += key.KeyChar;
                    Console.Write(key.KeyChar);
                }
            }

            Console.Clear();
        }

        return choice;
    }
    
}