using Microsoft.EntityFrameworkCore;

namespace Domain.Database;


public class AppDbContext : DbContext
{
    public DbSet<GameState> GameStates { get; set; } = default!;
    public DbSet<Player> Players { get; set; } = default!;
    public DbSet<Hand> Hands { get; set; } = default!;
    public DbSet<StockPile> StockPiles { get; set; } = default!;
    public DbSet<UnoDeck> UnoDecks { get; set; } = default!;
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}