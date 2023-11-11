namespace Domain.Database;

public class Hand
{
    public int Id { get; set; } 
    
    public int CardColor { get; set; } 
    public int CardValue { get; set; } 
    
    public int PlayerId { get; set; } 
    public int GameStateId { get; set; } 

}