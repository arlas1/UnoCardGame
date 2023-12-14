using System.Text.Json;

namespace Domain;

public class GameState
{
    public bool GameDirection { get; set; }
    public int CurrentPlayerIndex { get; set; } = 0;
    
    public UnoDeck UnoDeck = new();
    
    public List<UnoCard> StockPile = new();

    public List<Player> PlayersList = new();
    
    public UnoCard.Color CardColorChoice = default;
    public bool IsColorChosen { get; set; } = false;
    public int SelectedCardIndex { get; set; } = -1;
    
    // For the game start only. Property does not go into db.
    public int RepositoryChoice { get; set; }
}