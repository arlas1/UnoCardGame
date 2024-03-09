using DAL;
using FluentAssert;
using FluentAssertions;
using Tests.TestUtils.DALTestsUtils;
using Tests.TestUtils.DALTestsUtils.PlayerEntityUtils;
using Tests.TestUtils.DALTestsUtils.StockPileEntityUtils;
using Xunit;

namespace Tests.UnitTests.DALTests;

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
        var sampleCard = EntityTestUtils.CreateStockPileCardEntityWithId(SampleId);
        var stockPileRepository = new StockPileRepository(_dbContext);

        // Act
        await stockPileRepository.AddStockPileCardAsync(sampleCard);

        // Assert
        var addedCards = stockPileRepository.GetAllStockPileCardsAsync();
        addedCards.Should()
                    .ContainSingle()
                    .Which
                    .Should()
                    .BeEquivalentTo(sampleCard);
        
        // Clean up 
        await stockPileRepository.DeleteAllStockPileCardsAsync();
    }
    
    [Fact]
    public async void StockPileEntity_AddAndRetrieveCard_CardRetrievedSuccessfully()
    {
        // Arrange
        var sampleCard = EntityTestUtils.CreateStockPileCardEntityWithId(SampleId);
        var stockPileRepository = new StockPileRepository(_dbContext);

        // Act
        await stockPileRepository.AddStockPileCardAsync(sampleCard);
        var addedCard = stockPileRepository.GetStockPileById(SampleId);

        // Assert
        addedCard.Should().NotBeNull();
        addedCard.Should().BeEquivalentTo(sampleCard);
        // Clean up 
        await stockPileRepository.DeleteAllStockPileCardsAsync();
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
        var sampleCard = EntityTestUtils.CreateStockPileCardEntityWithId(SampleId);
        
        var sampleCardColor = sampleCard.CardColor; // Is 0
        var sampleCardValue = sampleCard.CardValue; // Is 0
        
        // Act
        await stockPileRepository.AddStockPileCardAsync(sampleCard);
        
        var addedPlayer = stockPileRepository.GetStockPileById(SampleId);
        addedPlayer!.CardColor = newColor;
        addedPlayer!.CardValue = newValue;
        
        await stockPileRepository.UpdateStockPileCardAsync(addedPlayer);
        
        // Assert
        var updatedPlayer = stockPileRepository.GetStockPileById(SampleId);
        
        updatedPlayer!.CardColor.ShouldNotBeEqualTo(sampleCardColor);
        updatedPlayer.CardValue.ShouldBeEqualTo(newColor);
        
        updatedPlayer!.CardValue.ShouldNotBeEqualTo(sampleCardValue);
        updatedPlayer.CardValue.ShouldBeEqualTo(newValue);

        // Clean up 
        await stockPileRepository.DeleteAllStockPileCardsAsync();
    }
    
    [Fact]
    public async void StockPileEntity_AddAndDeleteCard_CardDeletedSuccessfully()
    {
        // Arrange
        var sampleCard = EntityTestUtils.CreateStockPileCardEntityWithId(SampleId);
        var stockPileRepository = new StockPileRepository(_dbContext);
        
        // Act
        await stockPileRepository.AddStockPileCardAsync(sampleCard);
        await stockPileRepository.DeleteStockPileCardAsync(SampleId);

        // Assert
        var listWithAllStockPileCards = stockPileRepository.GetAllStockPileCardsAsync();
        listWithAllStockPileCards.Should().BeEmpty();
    }
}