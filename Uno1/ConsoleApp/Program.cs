using DAL;
using Domain;
using Domain.Database;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp;

public static class Program
{
    private static void Main()
    {
        
         // var connectionString =
         //     "Server=barrel.itcollege.ee;User Id=student;Password=Student.Pass.1;Database=student_arlasi;MultipleActiveResultSets=true";
         //
         // var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
         //     .UseSqlServer(connectionString)
         //     .EnableDetailedErrors()
         //     .EnableSensitiveDataLogging()
         //     .Options;
         //
         // using (var db = new AppDbContext(contextOptions))
         // {
         //     db.Database.Migrate();
         // }
         
         
         //Start the menu with json db
         Menu.Menu.RunMenu(NewOrLoadGame.NewGame, NewOrLoadGame.LoadGameJson);
         
         //Start the menu with Entity.framework db
         //Menu.Menu.RunMenu(NewOrLoadGame.NewGame, DbLoadNewGame.LoadNewGameDb);


        
        return;
    }
    
}