using Domain;
using UnoGameEngine;

namespace ConsoleUI;

public static class ConsoleVisualization
{
    public static void DisplayGameHeader(GameEngine gameEngine)
    {
        if (gameEngine.GameState.UnoDeck.IsEmpty())
        {
            gameEngine.GameState.UnoDeck.Create();
            gameEngine.GameState.UnoDeck.Shuffle();
        }

        Console.Clear();
        
        if (gameEngine.GameState.IsColorChosen)
        {
            Console.WriteLine("=======================");
            Console.WriteLine($"Wild card color: {gameEngine.GameState.CardColorChoice}");
        }

        Console.WriteLine("=======================");
        Console.WriteLine("Game direction: " + (gameEngine.GameState.GameDirection ? "Counterclockwise" : "Clockwise"));
        Console.WriteLine("=======================");
        Console.WriteLine("Cards in deck left: " + gameEngine.GameState.UnoDeck.Cards.Count);
        Console.WriteLine("=======================");
        Console.WriteLine("Top card --> " + gameEngine.GameState.StockPile.Last() + " <--");
        Console.WriteLine("=======================");

    }
    
    
    public static void DisplayPlayerHand(IReadOnlyList<UnoCard> currentPlayerHand, GameEngine gameEngine)
    {
        Console.WriteLine($"{gameEngine.GameState.PlayersList[gameEngine.GameState.CurrentPlayerIndex].Name}'s hand:");
        gameEngine.GameState.SelectedCardIndex = 0;
        for (var i = 0; i < currentPlayerHand.Count; i++)
        {
            if (i == gameEngine.GameState.SelectedCardIndex)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
            }

            Console.WriteLine($"{i + 1}. {currentPlayerHand[i]}");

            Console.ResetColor();
        }

        if (gameEngine.GameState.SelectedCardIndex == currentPlayerHand.Count)
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