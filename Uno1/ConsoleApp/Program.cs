using ConsoleUI;
using UnoGameEngine;

namespace ConsoleApp;

public static class Program
{
    private static void Main()
    {
        var gameEngine = new GameEngine();

        GameConfiguration.PromptForRepositoryType(gameEngine);

        switch (gameEngine.GameState.RepositoryChoice)
        {
            case 1:
                Menu.Menu.RunMenu(() => GameSetup.NewGame(gameEngine), () => GameSetup.LoadGameJson(gameEngine));
                break;
            case 2:
                Menu.Menu.RunMenu(() => GameSetup.NewGame(gameEngine), () => GameSetup.LoadGameDb(gameEngine));
                break;
        }
    }
}