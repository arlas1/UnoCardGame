using DAL.DbEntities;
using Microsoft.EntityFrameworkCore;

namespace DAL;


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
        
        const string dbFilePath = @"C:\Users\lasim\RiderProjects\icd0008-23f\Uno1\DAL\UnoDb.db";
        const string connectionString = $"Data Source={dbFilePath};";
        base.OnConfiguring(optionsBuilder);
            
        // Configure SQLite
        optionsBuilder.UseSqlite(connectionString);
    }
}