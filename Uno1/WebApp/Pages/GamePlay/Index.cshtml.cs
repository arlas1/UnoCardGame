using DAL.DbEntities;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.GamesManager;
using GameState = DAL.DbEntities.GameState;
using Player = DAL.DbEntities.Player;
using UnoDeck = DAL.DbEntities.UnoDeck;

namespace WebApp.Pages.GamePlay;

public class IndexModel(DAL.AppDbContext context) : PageModel
{
    public GameState GameState { get;set; } = default!;
    public IList<Player> Players { get;set; } = default!;
    public IList<UnoDeck> UnoDeck { get;set; } = default!;
    public IList<StockPile> StockPile { get;set; } = default!;
    public IList<Hand> AllHandCards { get;set; } = default!;
    public IDictionary<int, IList<Hand>> PlayersHands { get; set; } = new Dictionary<int, IList<Hand>>();
    
    [BindProperty(SupportsGet = true)]
    public int GameId { get; set; }
        
    [BindProperty(SupportsGet = true)]
    public int PlayerId { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public int? IsBoss { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public string? Command { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public UnoCard.Color Color { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public UnoCard.Value Value { get; set; }
    
    public string? Direction { get; set; }
    public int? CurrentPlayerIndex { get; set; }
    public int IsColorChosen { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public UnoCard.Color ChosenColor { get; set; }

    
    public async Task OnGet()
    {
        GameState = context.GameStates.SingleOrDefault(state => state.Id == GameId)!;
        Players = await context.Players.Where(player => player.GameStateId == GameId).ToListAsync();
        UnoDeck = await context.UnoDecks.Where(deck => deck.GameStateId == GameId).ToListAsync();
        StockPile = await context.StockPiles.Where(pile => pile.GameStateId == GameId).ToListAsync();
        AllHandCards = await context.Hands.Where(hand => hand.GameStateId == GameId).ToListAsync();

        foreach (var player in Players)
        {
            foreach (var card in AllHandCards)
            {
                if (player.Id == card.PlayerId)
                {
                    if (!PlayersHands.ContainsKey(player.Id))
                    {
                        PlayersHands[player.Id] = new List<Hand>();
                    }

                    PlayersHands[player.Id].Add(card);
                }
            }
        }

        Direction = GameState.GameDirection == 0 ? "Clockwise" : "Counterclockwise";
        IsColorChosen = GameState.IsColorChosen;
        CurrentPlayerIndex = GameState.CurrentPlayerIndex;
    }

    public async Task<IActionResult> OnPost()
    {
        
        var gameManager = new GameManager(context);
        var players = await context.Players.Where(player => player.GameStateId == GameId)
            .OrderBy(player => player.Id)
            .ToListAsync();
        var gameState1 = context.GameStates.SingleOrDefault(state => state.Id == GameId)!;
        CurrentPlayerIndex = gameState1.CurrentPlayerIndex;
        
        var currentPlayerId = players.IndexOf(players.FirstOrDefault(player => player.Id == PlayerId)!);
        
        switch (Command)
        {
            case "updateState":
            {
                var game = await gameManager.CheckForWinner(GameId, PlayerId);
                if (game.hasWinner)
                {
                    var winnerMessage = $"{players.SingleOrDefault(player => player.Id == gameState1.WinnerId)!.Name} won!";
                
                    return RedirectToPage($"/Dashboard/Index", new { WinnerMessage = winnerMessage });
                }
                break;
            }
            case "start":
            {
                gameManager.StartTheGame(GameId);
                break;
            }
            case "drawCard":
            {
                if (currentPlayerId == CurrentPlayerIndex)
                {
                    await gameManager.DrawCard(GameId, PlayerId);
                }
                break;
            }
            case "playCard":
            {
                if (currentPlayerId == CurrentPlayerIndex)
                {
                    var playedCard = new UnoCard(Color, Value);
                    if (await gameManager.ValidateCard(GameId, playedCard, null))
                    {
                        await gameManager.PlayCardHuman(GameId, PlayerId, playedCard);
                        
                        var game = await gameManager.CheckForWinner(GameId, PlayerId);
                        if (game.hasWinner && PlayerId == game.playerId)
                        {
                            var winnerMessage = $"{players.SingleOrDefault(player => player.Id == gameState1.WinnerId)!.Name} won!";
                
                            return RedirectToPage($"/Dashboard/Index", new { WinnerMessage = winnerMessage });
                        }
                    }
                }
                break;
            }
            case "choseColor":
            {
                if (currentPlayerId == CurrentPlayerIndex)
                {
                    var playedCard = new UnoCard(Color, Value);
                    if (await gameManager.ValidateCard(GameId, playedCard, ChosenColor))
                    {
                        await gameManager.PlayCardHuman(GameId, PlayerId, playedCard);

                    }
                }
                break;
            }
        }
        GameState = context.GameStates.SingleOrDefault(state => state.Id == GameId)!;
        if (GameState.IsGameEnded == 1)
        {
            var winnerMessage = $"{players.SingleOrDefault(player => player.Id == GameState.WinnerId)!.Name} won!";
                
            return RedirectToPage($"/Dashboard/Index", new { WinnerMessage = winnerMessage }); 
        }
        Players = await context.Players.Where(player => player.GameStateId == GameId).ToListAsync();
        UnoDeck = await context.UnoDecks.Where(deck => deck.GameStateId == GameId).ToListAsync();
        StockPile = await context.StockPiles.Where(pile => pile.GameStateId == GameId).ToListAsync();
        AllHandCards = await context.Hands.Where(hand => hand.GameStateId == GameId).ToListAsync();
        
        foreach (var player in Players)
        {
            foreach (var card in AllHandCards)
            {
                if (player.Id == card.PlayerId)
                {
                    if (!PlayersHands.ContainsKey(player.Id))
                    {
                        PlayersHands[player.Id] = new List<Hand>();
                    }

                    PlayersHands[player.Id].Add(card);
                }
            }
        }
        
        Direction = GameState.GameDirection == 0 ? "Clockwise" : "Counterclockwise";
        IsColorChosen = GameState.IsColorChosen;
        CurrentPlayerIndex = GameState.CurrentPlayerIndex;
        ChosenColor = (UnoCard.Color)GameState.CardColorChoice;
        return Page();
    }
    public string GetCardColor(int cardColor)
    {
        return ((UnoCard.Color)cardColor).ToString();
    }

    public string GetCardValue(int cardValue)
    {
        return ((UnoCard.Value)cardValue).ToString();
    }

    public IActionResult Winner(bool hasWinner)
    {
        return RedirectToPage($"/Dashboard/Index");
    }
}