using Menu;

namespace Domain;

public static class Game
{
    // Asking Players amount
    public static int PromptForNumberOfPlayers()
    {
        int numPlayers;

        while (true)
        {
            Console.WriteLine("Enter amount of players (2-7). 2 if Enter is pressed: ");
            var input = Console.ReadLine()!;

            if (string.IsNullOrWhiteSpace(input))
            {
                // User pressed Enter; use the default value
                numPlayers = 2;
                Console.Clear();
                break;
            }

            if (int.TryParse(input, out numPlayers) && numPlayers is >= 2 and <= 7)
            {
                Console.Clear();
                break; // Valid input, exit the loop
            }

            Console.Clear();
        }

        return numPlayers;
    }

    
    // Create players with user input names
    public static void CreatePlayers(int numPlayers)
    {
        var players = new List<Player>();

        for (var i = 0; i < numPlayers; i++)
        {
            Console.WriteLine($"Enter player's {i + 1} nickname (Within 20 characters):");
            Console.WriteLine("Enter / wrong input --> nickname = Player's number");

            var playerName = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(playerName) || playerName.Length > 20)
            {
                playerName = $"Player {i + 1}";
                Console.Clear();
            }

            Console.Clear();

            string playerTypeInput;

            while (true)
            {
                Console.WriteLine($"Enter player's {playerName} type (Human/Ai) [h/a]:");
                Console.WriteLine("If Enter is pressed - Human");
                playerTypeInput = Console.ReadLine()!.ToLower();

                if (playerTypeInput == "h" || playerTypeInput == "a" || string.IsNullOrWhiteSpace(playerTypeInput))
                {
                    Console.Clear();
                    break;
                }

                Console.Clear();
            }

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

    
    // Check first stockpile card for type (Wild, Reverse, Skip, +2, +4)
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

    
    // First Level of card placement control
    private static bool IsValidCardPlay(UnoCard card)
    {
        if (GameState.StockPile.Last().CardColor == UnoCard.Color.Wild && GameState.StockPile.Last().CardValue == UnoCard.Value.Wild)
        {
            return card.CardColor == GameState.CardColorChoice;
        }
        
        return (card.CardColor == GameState.StockPile.Last().CardColor ||
                card.CardValue == GameState.StockPile.Last().CardValue ||
                UnoCard.Color.Wild == GameState.StockPile.Last().CardColor ||
                card.CardColor == UnoCard.Color.Wild);
    }

    
    // For the main game loop to switch the players
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
                GameState.CurrentPlayerIndex = (pId - 2 + numPlayers) % numPlayers;            }
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
                GameState.CurrentPlayerIndex = (pId - 1 + numPlayers) % numPlayers;            }
        }
    }
    
    
    // Display game state with every player hand 
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
    
    
    // Main part of the game, loop
    public static void StartTheGame(int numPlayers)
    {
        bool exitGame = false;

        while (!exitGame)
        {
            DisplayGameHeader();
            
            var currentPlayerHand = GameState.PlayersList[GameState.CurrentPlayerIndex].Hand;

            FirstDisplay(currentPlayerHand);
                
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
                        GameState.SelectedCardIndex = (GameState.SelectedCardIndex - 1 + currentPlayerHand.Count + 2) % (currentPlayerHand.Count + 2);
                        break;
                    case ConsoleKey.DownArrow:
                        GameState.SelectedCardIndex = (GameState.SelectedCardIndex + 1) % (currentPlayerHand.Count + 1);
                        break;
                }

                Console.Clear();
                DisplayGameHeader();

                Console.WriteLine($"Player {GameState.CurrentPlayerIndex + 1}'s hand:");
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


            } while (key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.RightArrow && key.Key != ConsoleKey.LeftArrow);

            if (key.Key == ConsoleKey.RightArrow)
            {
                JsonOptions.SaveIntoJson();
                Menu.Menu.RunMenu(NewOrLoadGame.NewGame, NewOrLoadGame.LoadGameJson);
                
                exitGame = true; // Exit the game loop
            } 
            if (key.Key == ConsoleKey.LeftArrow)
            {
                Menu.Menu.RunMenu(NewOrLoadGame.NewGame, NewOrLoadGame.LoadGameJson);
                
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
                            Console.WriteLine($"{GameState.PlayersList[GameState.CurrentPlayerIndex + 1].Name} wins! Congratulations!");
                            GameState.IsColorChosen = false;
                            exitGame = true; // Set the flag to exit the game loop
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
    
    
    // Apply card logic after it being placed
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

    
    // Handle wild card being placed
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
    private static void FirstDisplay(IReadOnlyList<UnoCard> currentPlayerHand)
    {
        Console.WriteLine($"Player {GameState.CurrentPlayerIndex + 1}'s hand:");
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
    
}