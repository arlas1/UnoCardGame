namespace Domain;

public static class GameState
{
    public static bool GameDirection { get; set; }
    public static int CurrentPlayerIndex { get; set; } = 0;
    
    public static UnoDeck UnoDeck = new();
    
    public static List<UnoCard> StockPile = new();

    public static List<Player> PlayersList = new();
    
    public static UnoCard.Color CardColorChoice = default;
    public static bool IsColorChosen { get; set; } = false;
    public static int SelectedCardIndex { get; set; } = -1; // Initialize to -1

}