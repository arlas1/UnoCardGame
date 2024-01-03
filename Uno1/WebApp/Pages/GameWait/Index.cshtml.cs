using DAL;
using DAL.DbEntities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using WebApp.GamesManager;

namespace WebApp.Pages.GameWait;

public class IndexModel(AppDbContext context) : PageModel
{
    public IList<Player> Players { get;set; } = default!;
        
    [BindProperty(SupportsGet = true)]
    public int GameId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int? IsBoss { get; set; }

    [BindProperty(SupportsGet = true)]
    public int MaxAmount { get; set; }
        
    [BindProperty(SupportsGet = true)]
    public string? Nickname { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public int? PlayerId { get; set; }
    
    [BindProperty(SupportsGet = true)]
    public string? Command { get; set; }
    
    [BindProperty(SupportsGet = true)] 
    public Domain.Player.PlayerType PlayerType { get; set; }
    
    public int? IsGameStarted { get; set; }

    public int PlayersToStart;


    public async Task<RedirectToPageResult> OnGet()
    {
        if (Command == "checkGameStart")
        {
            var gameManager = new GameManager(context);
            if (gameManager.CheckGameStart(GameId))
            {
                return RedirectToPage($"/GamePlay/Index", new { GameId, PlayerId, IsBoss = 0 });
            }
        }
        
        Players = await context.Players.Where(player => player.GameStateId == GameId).ToListAsync();
        
        var gameState = context.GameStates.SingleOrDefault(state => state.Id == GameId)!;
        IsGameStarted = gameState.IsGameStarted;

        PlayersToStart = MaxAmount - Players.Count;
        return null!;
    }

    public async Task<IActionResult> OnPost()
    {
        if (PlayerType == Domain.Player.PlayerType.Human)
        {
            Players = await context.Players.Where(player => player.GameStateId == GameId).ToListAsync();
        
            var gameManager = new GameManager(context);
            var data = await gameManager.JoinTheGame(GameId, Nickname!, PlayerType);
        
            PlayerId = data.playerId;
            MaxAmount = data.maxAmount;
        
            PlayersToStart = MaxAmount - Players.Count;
        }
        else if (PlayerType == Domain.Player.PlayerType.Ai)
        {
            var gameManager = new GameManager(context);
            await gameManager.JoinTheGame(GameId, Nickname!, PlayerType);
            return RedirectToPage($"/Dashboard/Index");

        }

        return Page();
    }

}