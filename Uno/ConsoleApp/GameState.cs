using System;
using System.Collections.Generic;

namespace ConsoleApp;

public class GameState
{
    public Guid Id { get; set; } = Guid.NewGuid();
        
    public List<UnoCard> DeckOfCardsInPlay { get; set; } = new List<UnoCard>();
    public List<UnoCard> DeckOfCardsGraveyard { get; set; } = new List<UnoCard>();
    public UnoCard? TrumpCard { get; set; }
    public int ActivePlayerNo { get; set; } = 0;
}