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
        base.OnConfiguring(optionsBuilder);
        
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseInMemoryDatabase("TestDatabase");
        }
        else
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-FEPAJ2M\\MSSQLSERVER01;Database=Uno;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }
}