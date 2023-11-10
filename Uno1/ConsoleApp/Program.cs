using Domain;

namespace ConsoleApp;

public static class Program
{
    private static void Main()
    {
        // Start the menu
        Menu.Menu.RunMenu(NewOrLoadGame.NewGame, NewOrLoadGame.LoadGame);
        return;
    }
}