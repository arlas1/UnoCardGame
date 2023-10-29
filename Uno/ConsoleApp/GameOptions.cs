namespace ConsoleApp;

public class GameOptions
{
    // max hand size during gameplay
    public int HandSize { get; set; } = 7;
    
    // discard 2-3-4-5 from initial deck
    public bool UseSmallCards { get; set; } = false;
}