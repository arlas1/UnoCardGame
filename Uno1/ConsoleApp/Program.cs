using DAL;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace ConsoleApp;

public static class Program
{
    // private static string ConnectionString =
    //     "Server=barrel.itcollege.ee;User Id=student;Password=Student.Pass.1;Database=student_<@arlasi@>;MultipleActiveResultSets=true";


    private static void Main()
    {
        
        // var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
        //     .UseSqlServer(ConnectionString)
        //     .EnableDetailedErrors()
        //     .EnableSensitiveDataLogging()
        //     .Options;
        // using var db = new AppDbContext(contextOptions);
        // db.Database.Migrate();
        // Console.WriteLine(db);
         //Start the menu
        Menu.Menu.RunMenu(NewOrLoadGame.NewGame, NewOrLoadGame.LoadGameJson);
        
        return;
    }
}