using System.Text;

namespace Domain;

public static class Game
{
    
    public static int PromptForNumberOfPlayers()
    {
        return (int)GetValidatedInput(0, 1, "");
    }
    
    
    public static void CreatePlayers(int numPlayers)
    {
        var players = new List<Player>();

        for (var i = 0; i < numPlayers; i++)
        {
            var playerName = (string)GetValidatedInput(i, 2, "");
            var playerTypeInput = (string)GetValidatedInput(i, 3, playerName);
            
            
            var playerType = playerTypeInput == "a" ? Player.PlayerType.Ai : Player.PlayerType.Human;
            var player = new Player(i, playerName, playerType);
            
            
            for (var j = 0; j < 7; j++)
            {
                var drawnCard = GameState.UnoDeck.DrawCard();
                player.Hand.Add(drawnCard);
            }

            players.Add(player);
        }

        GameState.PlayersList = players;
    }
    
    
    public static void CheckFirstCard(UnoDeck unoDeck, List<UnoCard> stockPile)
    {
        var isValid = false;

        while (!isValid)
        {
            var initialCard = unoDeck.DrawCard();

            // Invalid card types for the start
            if (initialCard.CardValue is
                UnoCard.Value.Wild or
                UnoCard.Value.DrawTwo or
                UnoCard.Value.WildFour or
                UnoCard.Value.Skip or
                UnoCard.Value.Reverse)
            {
                isValid = false;
            }
            else
            {
                stockPile.Add(initialCard);
                break;
            }
        }
    }
    
    // Main loop of the game
    public static void StartTheGame(int numPlayers)
    {
        var exitGame = false;

        while (!exitGame)
        {
            DisplayGameHeader();

            var currentPlayerHand = GameState.PlayersList[GameState.CurrentPlayerIndex].Hand;

            DisplayPlayerHand(currentPlayerHand);

            ConsoleKeyInfo key;

            do
            {

                while (Console.KeyAvailable)
                {
                    Console.ReadKey(true);
                }

                key = Console.ReadKey();

                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        GameState.SelectedCardIndex = (GameState.SelectedCardIndex - 1 + currentPlayerHand.Count + 2) %
                                                      (currentPlayerHand.Count + 2);
                        break;
                    case ConsoleKey.DownArrow:
                        GameState.SelectedCardIndex = (GameState.SelectedCardIndex + 1) % (currentPlayerHand.Count + 1);
                        break;
                }

                Console.Clear();
                DisplayGameHeader();

                Console.WriteLine($"{GameState.PlayersList[GameState.CurrentPlayerIndex].Name}'s hand:");
                for (var i = 0; i < currentPlayerHand.Count; i++)
                {
                    if (i == GameState.SelectedCardIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }

                    Console.WriteLine($"{i + 1}. {currentPlayerHand[i]}");

                    Console.ResetColor();
                }

                if (GameState.SelectedCardIndex == currentPlayerHand.Count)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }

                Console.WriteLine($"{currentPlayerHand.Count + 1}. -> draw a card <-");

                Console.ResetColor();

                Console.WriteLine("=======================");
                Console.WriteLine("Press RIGHT ARROW to SAVE and EXIT to the main menu.");
                Console.WriteLine("                        OR");
                Console.WriteLine("Press LEFT ARROW to EXIT without saving game state.");


            } while (key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.RightArrow &&
                     key.Key != ConsoleKey.LeftArrow);

            if (key.Key == ConsoleKey.RightArrow)
            {
                // JsonOptions.SaveIntoJson();
                // Menu.Menu.RunMenu(NewOrLoadGame.NewGame, NewOrLoadGame.LoadGameJson);

                DbRepository.SaveIntoDb();
                Menu.Menu.RunMenu(GameSetupLoader.NewGame, GameSetupLoader.LoadGameDb);

                exitGame = true; // Exit the game loop
            }

            if (key.Key == ConsoleKey.LeftArrow)
            {
                // Menu.Menu.RunMenu(NewOrLoadGame.NewGame, NewOrLoadGame.LoadGameJson);

                Menu.Menu.RunMenu(GameSetupLoader.NewGame, GameSetupLoader.LoadGameDb);

                exitGame = true; // Exit the game loop
            }

            else
            {
                var pId = GameState.CurrentPlayerIndex;

                var isValid = false;

                // Card placing or drawing
                if (GameState.SelectedCardIndex == currentPlayerHand.Count)
                {
                    currentPlayerHand.Add(GameState.UnoDeck.DrawCard());
                    isValid = true;
                }
                else
                {
                    var selectedCard = currentPlayerHand[GameState.SelectedCardIndex];
                    if (IsValidCardPlay(selectedCard))
                    {
                        currentPlayerHand.RemoveAt(GameState.SelectedCardIndex);
                        GameState.StockPile.Add(selectedCard);

                        if (GameState.PlayersList[pId].Hand.Count == 0)
                        {
                            Console.WriteLine(
                                $"{GameState.PlayersList[GameState.CurrentPlayerIndex + 1].Name} wins! Congratulations!");
                            GameState.IsColorChosen = false;
                            break;
                        }

                        SubmitPlayerCard(selectedCard, pId, numPlayers);

                        if (selectedCard.CardColor == GameState.CardColorChoice)
                        {
                            GameState.IsColorChosen = false;
                        }

                        isValid = true;
                    }
                }

                if (!isValid)
                {
                    Console.Clear();
                    continue;
                }

                // Player switch + exclusive control for skip card
                GetNextPlayerId(pId, numPlayers);
            }
        }
    }

    
    private static bool IsValidCardPlay(UnoCard card)
    {
        if (GameState.StockPile.Last().CardColor == UnoCard.Color.Wild &&
            GameState.StockPile.Last().CardValue == UnoCard.Value.Wild)
        {
            return card.CardColor == GameState.CardColorChoice;
        }

        return (card.CardColor == GameState.StockPile.Last().CardColor ||
                card.CardValue == GameState.StockPile.Last().CardValue ||
                UnoCard.Color.Wild == GameState.StockPile.Last().CardColor ||
                card.CardColor == UnoCard.Color.Wild);
    }

    
    private static void GetNextPlayerId(int pId, int numPlayers)
    {
        if ((GameState.StockPile.Last().CardValue == UnoCard.Value.Skip))
        {
            if (!GameState.GameDirection)
            {
                // Move forward if skip
                GameState.CurrentPlayerIndex = (pId + 2) % numPlayers;

            }
            else
            {
                // Move backward if skip
                GameState.CurrentPlayerIndex = (pId - 2 + numPlayers) % numPlayers;
            }
        }
        else
        {
            if (!GameState.GameDirection)
            {
                // Move forward
                GameState.CurrentPlayerIndex = (pId + 1) % numPlayers;
            }
            else
            {
                // Move backward
                GameState.CurrentPlayerIndex = (pId - 1 + numPlayers) % numPlayers;
            }
        }
    }

    
    private static void DisplayGameHeader()
    {
        if (GameState.UnoDeck.IsEmpty())
        {
            GameState.UnoDeck.Create();
            GameState.UnoDeck.Shuffle();
        }

        Console.Clear();
        if (GameState.IsColorChosen)
        {
            Console.WriteLine("=======================");
            Console.WriteLine($"Wild card color: {GameState.CardColorChoice}");
        }

        Console.WriteLine("=======================");
        Console.WriteLine("Game direction: " + (GameState.GameDirection ? "Counterclockwise" : "Clockwise"));
        Console.WriteLine("=======================");
        Console.WriteLine("Cards in deck left: " + GameState.UnoDeck.Cards.Count);
        Console.WriteLine("=======================");
        Console.WriteLine("Top card --> " + GameState.StockPile.Last() + " <--");
        Console.WriteLine("=======================");

    }

    
    // Apply card logic after placing it
    private static void SubmitPlayerCard(UnoCard card, int pId, int numPlayers)
    {

        if (card.CardValue == UnoCard.Value.Reverse)
        {
            GameState.GameDirection = !GameState.GameDirection;

        }

        if (card is { CardColor: UnoCard.Color.Wild, CardValue: UnoCard.Value.Wild })
        {
            HandleWildCard();
        }

        if (card.CardValue == UnoCard.Value.DrawTwo)
        {
            if (!GameState.GameDirection)
            {
                GameState.PlayersList[(pId + 1) % numPlayers].Hand.Add(GameState.UnoDeck.DrawCard());
                GameState.PlayersList[(pId + 1) % numPlayers].Hand.Add(GameState.UnoDeck.DrawCard());

            }
            else
            {
                GameState.PlayersList[(pId - 1) % numPlayers].Hand.Add(GameState.UnoDeck.DrawCard());
                GameState.PlayersList[(pId - 1) % numPlayers].Hand.Add(GameState.UnoDeck.DrawCard());
            }
        }

        if (card is { CardColor: UnoCard.Color.Wild, CardValue: UnoCard.Value.WildFour })
        {
            if (!GameState.GameDirection)
            {
                GameState.PlayersList[(pId + 1) % numPlayers].Hand.Add(GameState.UnoDeck.DrawCard());
                GameState.PlayersList[(pId + 1) % numPlayers].Hand.Add(GameState.UnoDeck.DrawCard());
                GameState.PlayersList[(pId + 1) % numPlayers].Hand.Add(GameState.UnoDeck.DrawCard());
                GameState.PlayersList[(pId + 1) % numPlayers].Hand.Add(GameState.UnoDeck.DrawCard());
            }
            else
            {
                GameState.PlayersList[(pId - 1) % numPlayers].Hand.Add(GameState.UnoDeck.DrawCard());
                GameState.PlayersList[(pId - 1) % numPlayers].Hand.Add(GameState.UnoDeck.DrawCard());
                GameState.PlayersList[(pId - 1) % numPlayers].Hand.Add(GameState.UnoDeck.DrawCard());
                GameState.PlayersList[(pId - 1) % numPlayers].Hand.Add(GameState.UnoDeck.DrawCard());
            }
        }
    }

    
    private static void HandleWildCard()
    {
        var selectedIndex = 0;

        ConsoleKeyInfo key;

        do
        {
            Console.Clear();
            Console.WriteLine("Choose the color of the Wild card:");

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

        GameState.CardColorChoice = (UnoCard.Color)selectedIndex;
        GameState.IsColorChosen = true;
    }
    
    
    // Weird first display
    private static void DisplayPlayerHand(IReadOnlyList<UnoCard> currentPlayerHand)
    {
        Console.WriteLine($"{GameState.PlayersList[GameState.CurrentPlayerIndex].Name}'s hand:");
        GameState.SelectedCardIndex = 0;
        for (var i = 0; i < currentPlayerHand.Count; i++)
        {
            if (i == GameState.SelectedCardIndex)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
            }

            Console.WriteLine($"{i + 1}. {currentPlayerHand[i]}");

            Console.ResetColor();
        }

        if (GameState.SelectedCardIndex == currentPlayerHand.Count)
        {
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
        }

        Console.WriteLine($"{currentPlayerHand.Count + 1}. -> draw a card <-");

        Console.ResetColor();

        Console.WriteLine("=======================");
        Console.WriteLine("Press RIGHT ARROW to SAVE and EXIT to the main menu.");
        Console.WriteLine("                        OR");
        Console.WriteLine("Press LEFT ARROW to EXIT without saving game state.");

    }
    
    
    private static object GetValidatedInput(int playerIndex, int caseOfUsage, string playerNameInput)
    {
        object playerInput = null!;

        switch (caseOfUsage)
        {
            // Amount of players
            case 1:
            {
                int numPlayers;
                var input = string.Empty;

                while (true)
                {
                    Console.Write("Enter amount of players [2-7]. Press Enter for 2: ");
                    Console.Write(input);

                    var key = Console.ReadKey(intercept: true);

                    // Enter pressed
                    if (key.Key == ConsoleKey.Enter)
                    {
                        if (string.IsNullOrWhiteSpace(input))
                        {
                            numPlayers = 2;
                            Console.Clear();
                            break;
                        }

                        if (int.TryParse(input, out numPlayers) && numPlayers is >= 2 and <= 7)
                        {
                            Console.Clear();
                            break; // Valid input, exit the loop
                        }
                    }
                    else if (key.Key == ConsoleKey.Backspace && input.Length > 0)
                    {
                        // Handle backspace to delete the last character
                        input = input[..^1];
                        Console.Write("\b \b"); // Move the cursor back and overwrite the character with a space
                    }
                    else if (char.IsDigit(key.KeyChar) && input.Length < 1)
                    {
                        // Allow only the first digit within the specified range
                        var enteredDigit = int.Parse(key.KeyChar.ToString());
                        if (enteredDigit is >= 2 and <= 7)
                        {
                            input += key.KeyChar;
                            Console.Write(key.KeyChar);
                        }
                    }

                    Console.Clear();
                }

                playerInput = numPlayers;
                break;
            }
            // Players nickname
            case 2:
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
                        // Handle backspace to delete the last character
                        playerNameBuilder.Remove(playerNameBuilder.Length - 1, 1);
                        Console.Write("\b \b"); // Move the cursor back and overwrite the character with a space
                    }
                    else if (!char.IsControl(key.KeyChar) && playerNameBuilder.Length < 20)
                    {
                        // Allow only printable characters and limit the input to 20 characters
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

                playerInput = playerName;
                break;
            }
            // Players type
            case 3:
            {
                string input = string.Empty;

                while (true)
                {
                    Console.WriteLine($"Enter Player's {playerNameInput} type (Human/Ai) [h/a]. Press Enter for human:");
                    Console.Write(input);

                    var key = Console.ReadKey(intercept: true);

                    // Enter pressed
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
                            playerInput = input.Trim().ToLower(); // Valid input, exit the loop
                            Console.Clear();
                            break;
                        }
                    }
                    else if (key.Key == ConsoleKey.Backspace && input.Length > 0)
                    {
                        // Handle backspace to delete the last character
                        input = input[..^1];
                        Console.Write("\b \b"); // Move the cursor back and overwrite the character with a space
                    }
                    else if (key.KeyChar is 'h' or 'a' && input.Length < 1)
                    {
                        // Allow only the first character (h or a)
                        input += key.KeyChar;
                        Console.Write(key.KeyChar);
                    }

                    Console.Clear();
                }

                break;
            }
        }
        
        return playerInput;
    }
    
}