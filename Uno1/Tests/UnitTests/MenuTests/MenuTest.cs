using System.Diagnostics.CodeAnalysis;
using Tests.TestUtils;

namespace Tests.UnitTests.MenuTests;

[SuppressMessage("ReSharper", "SuggestVarOrType_SimpleTypes")]
public class MenuTest
{
    [Fact]
    public void MenuSimulation_RunAndSelectOptionNewGame_NewGameMethodCalled()
    {
        // Arrange
        var newGameMethodCalled = false;
        Action newGameMethod = () => newGameMethodCalled = true;
        Action loadGameMethod = () => { };

        MenuUtils.RedirectConsoleOutputToStringWriter();
        
        // Act
        using (StringReader input = new StringReader("enter\n"))
        {
            Console.SetIn(input);
            MenuUtils.MenuSimulationRun(newGameMethod, loadGameMethod);
        }

        // Assert
        Assert.True(newGameMethodCalled);
    }

    [Fact]
    public void MenuSimulation_RunAndSelectOptionLoadGame_LoadGameMethodCalled()
    {
        // Arrange
        var loadGameMethodCalled = false;
        Action newGameMethod = () => { };
        Action loadGameMethod = () => loadGameMethodCalled = true;

        MenuUtils.RedirectConsoleOutputToStringWriter();


        // Act
        using (StringReader input = new StringReader("down\nenter\n"))
        {
            Console.SetIn(input);
            MenuUtils.MenuSimulationRun(newGameMethod, loadGameMethod);
        }

        // Assert
        Assert.True(loadGameMethodCalled);
    }

    [Fact]
    public void MenuSimulation_RunAndSelectOptionExit_ExitMessageDisplayed()
    {
        // Arrange
        Action newGameMethod = () => { };
        Action loadGameMethod = () => { };
        var expectedOutput = "Have a good day!";

        var writer = MenuUtils.RedirectConsoleOutputToStringWriter();

        // Act
        using (StringReader input = new StringReader("down\ndown\nenter"))
        {
            Console.SetIn(input);
            MenuUtils.MenuSimulationRun(newGameMethod, loadGameMethod);
        }

        // Assert
        var output = writer.ToString();
        Assert.Contains(expectedOutput, output);
    }
    
}