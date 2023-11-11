using Microsoft.EntityFrameworkCore;

namespace DAL;


public class AppDbContext : DbContext
{
    public DbSet<Domain.Database.GameState> GameStates { get; set; } = default!;
    public DbSet<Domain.Database.Player> Players { get; set; } = default!;
    public DbSet<Domain.Database.Hand> Hands { get; set; } = default!;
    public DbSet<Domain.Database.StockPile> StockPiles { get; set; } = default!;
    public DbSet<Domain.Database.UnoDeck> UnoDecks { get; set; } = default!;
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}