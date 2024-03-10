using DAL;
using Microsoft.EntityFrameworkCore;

namespace Tests.UnitTests.DALInMemoryTests;

public class AppDbContextTest
{
    [Fact]
    public void AppDbContext_InitializeWithDefaultConstructor_InstantiatedSuccessfully()
    {
        // Arrange & Act
        var dbContext = new AppDbContext();
        
        // Assert
        Assert.NotNull(dbContext);
    }
    
    [Fact]
    public void AppDbContext_GetContextWithOptions_InstantiatedSuccessfully()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("TestDatabase")
            .Options;
        
        // Act
        var dbContext = new AppDbContext(options);
        
        // Assert
        Assert.NotNull(dbContext);
    }
    
    [Fact]
    public void AppDbContext_PropertiesAfterContextInitialization_PropertiesAreInitializedSuccessfully()
    {
        // Arrange
        var dbContext = new AppDbContext();
    
        // Assert
        Assert.NotNull(dbContext.GameStates);
        Assert.NotNull(dbContext.Players);
        Assert.NotNull(dbContext.Hands);
        Assert.NotNull(dbContext.StockPiles);
        Assert.NotNull(dbContext.UnoDecks);
    }
}