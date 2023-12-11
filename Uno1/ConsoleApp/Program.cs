using ConsoleUI;
using Domain;

namespace ConsoleApp;

public static class Program
{
    private static void Main()
    {
        GameConfiguration.PromptForRepositoryType();

        switch (GameState.RepositoryChoice)
        {
            case 1:
                Menu.Menu.RunMenu(GameSetup.NewGame, GameSetup.LoadGameJson);
                break;
            case 2:
                Menu.Menu.RunMenu(GameSetup.NewGame, GameSetup.LoadGameDb);
                break;
        }
    }
}