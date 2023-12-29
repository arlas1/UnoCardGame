namespace DAL.DbEntities;

public class GameState
{
    public int Id { get; set; }
    public int GameDirection { get; set; } // bool df - false
    public int CurrentPlayerIndex { get; set; } 
    public int IsColorChosen { get; set; } // bool df - false
    public int SelectedCardIndex { get; set; }

    public int CardColorChoice { get; set; }
    public int MaxCardAmount { get; set; }
    public int PlayersMaxAmount { get; set; }
    public int IsGameStarted { get; set; }
    public int IsGameEnded { get; set; }
    public int WinnerId { get; set; }
}