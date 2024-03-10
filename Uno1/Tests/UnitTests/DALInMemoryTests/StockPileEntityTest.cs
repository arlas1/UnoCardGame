using DAL;
using FluentAssert;
using FluentAssertions;
using Tests.TestUtils.DALTestsUtils.Repositories;

namespace Tests.UnitTests.DALInMemoryTests;

public class StockPileEntityTest
{
    private readonly AppDbContext _dbContext;
    private const int SampleId = 0;
    
    public StockPileEntityTest()
    {
        _dbContext = new AppDbContext();
        _dbContext.Database.EnsureCreated();
    }
    
    [Fact]
    public async void StockPileEntity_AddCard_CardAddedSuccessfully()
    {
        // Arrange
        var stockPileRepository = new StockPileRepository(_dbContext);
        var sampleCard = stockPileRepository.CreateEntityWithId(SampleId);

        // Act
        await stockPileRepository.AddAsync(sampleCard);

        // Assert
        var addedCards = stockPileRepository.GetAllAsync();
        addedCards.Should()
                    .ContainSingle()
                    .Which
                    .Should()
                    .BeEquivalentTo(sampleCard);
        
        // Clean up 
        await stockPileRepository.DeleteAllAsync();
    }
    
    [Fact]
    public async void StockPileEntity_AddAndRetrieveCard_CardRetrievedSuccessfully()
    {
        // Arrange
        var stockPileRepository = new StockPileRepository(_dbContext);
        var sampleCard = stockPileRepository.CreateEntityWithId(SampleId);
        
        // Act
        await stockPileRepository.AddAsync(sampleCard);
        var addedCard = stockPileRepository.GetById(SampleId);

        // Assert
        addedCard.Should().NotBeNull();
        addedCard.Should().BeEquivalentTo(sampleCard);
        // Clean up 
        await stockPileRepository.DeleteAllAsync();
    }
    
    [Theory]
    [InlineData( 1, 1)]
    [InlineData( 2, 2)]
    public async void StockPileEntity_AddUpdateAndRetrieveCard_CardUpdatedAndRetrievedSuccessfully(
        int newColor,
        int newValue)
    {
        // Arrange
        var stockPileRepository = new StockPileRepository(_dbContext);
        var sampleCard = stockPileRepository.CreateEntityWithId(SampleId);
        
        var sampleCardColor = sampleCard.CardColor; // Is 0
        var sampleCardValue = sampleCard.CardValue; // Is 0
        
        // Act
        await stockPileRepository.AddAsync(sampleCard);
        
        var addedPlayer = stockPileRepository.GetById(SampleId);
        addedPlayer!.CardColor = newColor;
        addedPlayer!.CardValue = newValue;
        
        await stockPileRepository.UpdateAsync(addedPlayer);
        
        // Assert
        var updatedPlayer = stockPileRepository.GetById(SampleId);
        
        updatedPlayer!.CardColor.ShouldNotBeEqualTo(sampleCardColor);
        updatedPlayer.CardValue.ShouldBeEqualTo(newColor);
        
        updatedPlayer!.CardValue.ShouldNotBeEqualTo(sampleCardValue);
        updatedPlayer.CardValue.ShouldBeEqualTo(newValue);

        // Clean up 
        await stockPileRepository.DeleteAllAsync();
    }
    
    [Fact]
    public async void StockPileEntity_AddAndDeleteCard_CardDeletedSuccessfully()
    {
        // Arrange
        var stockPileRepository = new StockPileRepository(_dbContext);
        var sampleCard = stockPileRepository.CreateEntityWithId(SampleId);
        
        // Act
        await stockPileRepository.AddAsync(sampleCard);
        await stockPileRepository.DeleteAsync(SampleId);

        // Assert
        var listWithAllStockPileCards = stockPileRepository.GetAllAsync();
        listWithAllStockPileCards.Should().BeEmpty();
    }
}