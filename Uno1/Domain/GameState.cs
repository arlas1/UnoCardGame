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
    public int CardsMaxAmount { get; set; }
    public int PlayersMaxAmount { get; set; }
    public int IsGameStarted { get; set; } = 0;
    public int IsGameEnded { get; set; } = 0;
    
    // For the game start only. Property does not go into db.
    public int RepositoryChoice { get; set; }
}