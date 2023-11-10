namespace Menu;

public static class Menu
{
    public static void RunMenu(Func<string?> newGameMethod, Func<string?> loadGameMethod)
    {
        string[] menuOptions = { "Start a new game", "Load game", "Exit" };
        var selectedIndex = 0;

        ConsoleKeyInfo key;

        do
        {
            Console.Clear();
            //Console.WriteLine(">> U N O <<");
            Console.WriteLine(">>  Welcome to UNO!  <<");
            Console.WriteLine("=======================");

            for (var i = 0; i < menuOptions.Length; i++)
            {
                if (i == selectedIndex)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }

                Console.WriteLine($"{i + 1}. {menuOptions[i]}");

                Console.ResetColor();
            }
            
            Console.WriteLine("=======================");

            key = Console.ReadKey();

            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = (selectedIndex - 1 + menuOptions.Length) % menuOptions.Length;
                    break;
                case ConsoleKey.DownArrow:
                    selectedIndex = (selectedIndex + 1) % menuOptions.Length;
                    break;
            }
        }
        while (key.Key != ConsoleKey.Enter);

        Console.Clear();

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
}