using Domain;

namespace ConsoleApp;

public static class Program
{
    private static void Main()
    {
        switch (Game.RepositoryChoice())
        {
            case 1:
                //Start the menu with JSON repository
                Menu.Menu.RunMenu(GameSetupLoader.NewGame, GameSetupLoader.LoadGameJson);
                break;
            case 2:
                //Start the menu with SQLite repository
                Menu.Menu.RunMenu(GameSetupLoader.NewGame, GameSetupLoader.LoadGameDb);
                break;
        }
    }
    

}