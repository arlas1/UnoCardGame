namespace Domain.Database;

public class GameState
{
    public int Id { get; set; }
    
    public int GameDirection { get; set; } // bool df - false
    public int CurrentPlayerIndex { get; set; } 
    public int IsColorChosen { get; set; } // bool df - false
    public int SelectedCardIndex { get; set; }
}