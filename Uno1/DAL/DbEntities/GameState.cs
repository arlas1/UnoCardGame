﻿namespace DAL.DbEntities;

public class GameState
{
    public int Id { get; set; }
    public int GameDirection { get; set; }
    public int CurrentPlayerIndex { get; set; } 
    public int IsColorChosen { get; set; }
    public int SelectedCardIndex { get; set; }
    public int CardColorChoice { get; set; }
    public int MaxCardAmount { get; set; }
    public int PlayersMaxAmount { get; set; }
    public int IsGameStarted { get; set; }
    public int IsGameEnded { get; set; }
    public int WinnerId { get; set; }
    public int ConsoleSaved { get; set; }
}