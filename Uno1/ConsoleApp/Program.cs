using Domain;

namespace ConsoleApp;

public static class Program
{
    private static void Main()
    {
         //Start the menu with JSON repository
         // Menu.Menu.RunMenu(NewOrLoadGame.NewGame, NewOrLoadGame.LoadGameJson);
         
         //Start the menu with SQLite repository
         Menu.Menu.RunMenu(GameSetupLoader.NewGame, GameSetupLoader.LoadGameDb);
    }
}