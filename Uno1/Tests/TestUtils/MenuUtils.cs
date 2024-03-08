namespace Tests.TestUtils;

public static class MenuUtils
{
    public static void SimulateMenuInteraction(Action newGameMethod, Action loadGameMethod)
    {
        string[] menuOptions = { "Start a new game", "Load game", "Exit" };
        var selectedIndex = 0;
        string? userInput;

        do
        {
            Console.WriteLine(">>  Welcome to UNO!  <<");
            Console.WriteLine("=======================");

            for (var i = 0; i < menuOptions.Length; i++)
            {
                if (i == selectedIndex)
                {
                    Console.Write("> ");
                }
                else
                {
                    Console.Write("  ");
                }

                Console.WriteLine($"{menuOptions[i]}");
            }
            Console.WriteLine("=======================");

            userInput = Console.ReadLine();
            switch (userInput)
            {
                case "up":
                    selectedIndex = (selectedIndex - 1 + menuOptions.Length) % menuOptions.Length;
                    break;
                case "down":
                    selectedIndex = (selectedIndex + 1) % menuOptions.Length;
                    break;
                case "enter":
                    break; 
            }
        }
        while (userInput != "enter");

        switch (selectedIndex)
        {
            case 0:
                newGameMethod();
                break;
            case 1:
                loadGameMethod();
                break;
            case 2:
                Console.WriteLine("Have a good day!");
                break;
        }
    }

    public static StringWriter RedirectConsoleOutputToStringWriter()
    {
        var writer = new StringWriter();
        Console.SetOut(writer);
        return writer;
    }
}