namespace Domain;

public class Player
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public PlayerType Type { get; set; }
    public List<UnoCard> Hand { get; set; }

    public Player( int id, string? name, PlayerType type)
    {
        Id = id;
        Name = name;
        Type = type;
        Hand = new List<UnoCard>();
    }
    

    public enum PlayerType
    {
        Human,
        Ai
    }
}