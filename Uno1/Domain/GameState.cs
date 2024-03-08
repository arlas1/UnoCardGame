namespace Domain;

public class GameState
{
    public UnoDeck UnoDeck = new();
    public List<UnoCard> StockPile = new();
    public List<Player> PlayersList = new();
    public UnoCard.Color CardColorChoice = default;
    public int MaxCardsAmount { get; set; }
    public int CurrentPlayerIndex { get; set; } = 0;
    public int SelectedCardIndex { get; set; } = 0;
    public int MaxPlayersAmount { get; set; }
    public bool GameDirection { get; set; }
    public bool IsColorChosen { get; set; } = false;
    public int IsGameStarted { get; set; } = 0;
    public int IsGameEnded { get; set; } = 0;
    public int IsConsoleSaved { get; set; }
    
    // For the game start only. Property does not go into db.
    public int RepositoryChoice { get; set; }
}