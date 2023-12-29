namespace DAL.DbEntities;

public class Player
{
    public int Id { get; set; }
    
    public string? Name { get; set; }
    public int Type { get; set; }
    public int Role { get; set; }
    
    public int GameStateId { get; set; }
}