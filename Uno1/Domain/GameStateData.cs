namespace Domain;

public class GameStateData
{
    //public int GameId { get; set; }
    public bool GameDirection { get; set; }
    public int CurrentPlayerIndex { get; set; }
    public UnoDeck UnoDeck { get; set; }
    public List<UnoCard> StockPile { get; set; }
    public List<Player> PlayersList { get; set; }
    public UnoCard.Color CardColorChoice { get; set; }
    public bool IsColorChosen { get; set; }
    public int SelectedCardIndex { get; set; }
    
    public UnoCard[] SerializedUnoDeck
    {
        get => UnoDeck.SerializedCards;
        set => UnoDeck.SerializedCards = value;
    }
}