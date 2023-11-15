using Domain;

namespace ConsoleApp;

public static class Program
{
    private static void Main()
    {
        // var dbFilePath = @"C:\Program Files\SQLiteStudio\UnoDb.db"; // Replace with your actual file path
         // var connectionString = $"Data Source={dbFilePath};";
         //
         // var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
         //     .UseSqlite(connectionString)
         //     .EnableDetailedErrors()
         //     .EnableSensitiveDataLogging()
         //     .Options;
         // using var db = new AppDbContext(contextOptions);
         // db.Database.Migrate();
         // Console.WriteLine(db);
         
         //Start the menu with JSON
         // Menu.Menu.RunMenu(NewOrLoadGame.NewGame, NewOrLoadGame.LoadGameJson);
         
         //Start the menu with DB
         Menu.Menu.RunMenu(GameSetupLoader.NewGame, GameSetupLoader.LoadGameDb);
    }
}