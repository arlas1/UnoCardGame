using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;

namespace Domain.Database;


public class AppDbContext : DbContext
{
    public DbSet<GameState> GameStates { get; set; } = default!;
    public DbSet<Player> Players { get; set; } = default!;
    public DbSet<Hand> Hands { get; set; } = default!;
    public DbSet<StockPile> StockPiles { get; set; } = default!;
    public DbSet<UnoDeck> UnoDecks { get; set; } = default!;
    
    public AppDbContext()
    {
    }
    
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        
        var dbFilePath = @"C:\Users\lasim\RiderProjects\icd0008-23f\Uno1\Domain\Database\UnoDb.db"; // Replace with your actual file path
        var connectionString = $"Data Source={dbFilePath};";
        base.OnConfiguring(optionsBuilder);
            
        // Configure SQLite
        optionsBuilder.UseSqlite(connectionString);
    }
}