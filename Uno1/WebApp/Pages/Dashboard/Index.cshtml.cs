using DAL;
using DAL.DbEntities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.GamesManager;


namespace WebApp.Pages.Dashboard;

public class IndexModel(AppDbContext context) : PageModel
{
    public IList<GameState> GameStates { get;set; } = default!;
    public IList<Player> Players { get;set; } = default!;
    public Dictionary<int, Domain.GameState> Games { get; set; } = new();

    
    [BindProperty] 
    public string? Nickname { get; set; }
    
    [BindProperty(SupportsGet = true)] 
    public Domain.Player.PlayerType PlayerType { get; set; }
    
    [BindProperty(SupportsGet = true)] 
    public int GameId { get; set; }
    
    [BindProperty(SupportsGet = true)] 
    public int PlayerId { get; set; }
    
    [BindProperty(SupportsGet = true)] 
    public string? Command { get; set; }

    [BindProperty(SupportsGet = true)]
    public string? WinnerMessage { get; set; } = default;

    
    
    public async Task OnGet()
    {
        GameStates = await context.GameStates.ToListAsync();
        Players = await context.Players.ToListAsync();

        foreach (var gameState in GameStates)
        {
            Games[gameState.Id] = new Domain.GameState
            {
                IsGameStarted = gameState.IsGameStarted,
                IsGameEnded = gameState.IsGameEnded,
                PlayersMaxAmount = gameState.PlayersMaxAmount,
                IsConsoleSaved = gameState.ConsoleSaved == 1 ? 1 : 0
            };

            foreach (var player in Players)
            {
                if (player.GameStateId == gameState.Id)
                {
                    var playerToAdd = new Domain.Player(player.Id, player.Name, (Domain.Player.PlayerType) player.Type);
                    Games[gameState.Id].PlayersList.Add(playerToAdd);
                }
            }
        }
        
    }

    public async Task OnPost()
    {
        GameStates = await context.GameStates.ToListAsync();
        Players = await context.Players.ToListAsync();

        foreach (var gameState in GameStates)
        {
            Games[gameState.Id] = new Domain.GameState
            {
                IsGameStarted = gameState.IsGameStarted,
                PlayersMaxAmount = gameState.PlayersMaxAmount
            };

            foreach (var player in Players)
            {
                if (player.GameStateId == gameState.Id)
                {
                    var playerToAdd = new Domain.Player(player.Id, player.Name, Domain.Player.PlayerType.Human);
                    Games[gameState.Id].PlayersList.Add(playerToAdd);
                }
            }
        }
        
        var gameManager = new GameManager(context);
        
        switch (Command)
        {
            case "leave":
                gameManager.LeaveTheGame(GameId, PlayerId);
                break;
            case "delete":
                gameManager.DeleteTheGame(GameId);
                break;
        }
        
    }
}