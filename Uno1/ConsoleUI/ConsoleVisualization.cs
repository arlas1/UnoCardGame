using Domain;

namespace ConsoleUI;

public static class ConsoleVisualization
{
    
    public static void DisplayGameHeader()
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
    
    
    public static void DisplayPlayerHand(IReadOnlyList<UnoCard> currentPlayerHand)
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
    
}