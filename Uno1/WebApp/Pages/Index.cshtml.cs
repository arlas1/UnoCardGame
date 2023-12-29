using DAL;
using DAL.DbEntities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

// using WebApp.GameHub;

namespace WebApp.Pages;

public class IndexModel : PageModel
{
    private readonly AppDbContext _context;
    public IList<GameState> GameStates { get;set; } = default!;
    
    
    public IndexModel(AppDbContext context)
    {
        _context = context;
    }
    
    
    public async Task OnGetAsync()
    {
        GameStates = await _context.GameStates.ToListAsync();
    }
}