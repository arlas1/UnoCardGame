
namespace ConsoleApp;
public static class ProgramMenus
{
    public static Menu GetOptionsMenu(GameOptions gameOptions) =>
        new Menu("Options", new List<MenuItem>()
        {
            
        });
    
    
    public static Menu GetMainMenu(GameOptions gameOptions, Menu optionsMenu, Func<string?> newGameMethod, Func<string?> loadGameMethod) => 
        new Menu(">> U N O <<", new List<MenuItem>()
        {
            new MenuItem()
            {
                Shortcut = "s",
                MenuLabel = "Start a new game ",
                MenuLabelFunction = () => "Start a new game ",
                MethodToRun = newGameMethod
            },
            new MenuItem()
            {
                Shortcut = "l",
                MenuLabel = "Load game",
                MethodToRun = loadGameMethod
            }
        });
}