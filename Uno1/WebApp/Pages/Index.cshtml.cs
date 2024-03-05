using DAL;
using DAL.DbEntities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;


namespace WebApp.Pages;

public class IndexModel(AppDbContext context) : PageModel
{
    public IList<GameState> GameStates { get;set; } = default!;


    private async Task OnGetAsync()
    {
        GameStates = await context.GameStates.ToListAsync();
    }
}