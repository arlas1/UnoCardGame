using DAL;
using FluentAssert;
using FluentAssertions;
using Tests.TestUtils.DALTestsUtils.HandEntityTestUtils;

namespace Tests.UnitTests.DALInMemoryTests;

public class HandEntityTest
{
    private readonly AppDbContext _dbContext;
    private const int SampleId = 0;
    
    public HandEntityTest()
    {
        _dbContext = new AppDbContext();
        _dbContext.Database.EnsureCreated();
    }
    
    [Fact]
    public async void HandEntity_AddHandCard_CardAddedSuccessfully()
    {
        // Arrange
        var handRepository = new HandRepository(_dbContext);
        var sampleHandCard = handRepository.CreateEntityWithId(SampleId);

        // Act
        await handRepository.AddAsync(sampleHandCard);

        // Assert
        var addedHandCards = handRepository.GetAllAsync();
        addedHandCards.Should()
                    .ContainSingle()
                    .Which
                    .Should()
                    .BeEquivalentTo(sampleHandCard);
        
        // Clean up 
        await handRepository.DeleteAllAsync();
    }
    
    [Fact]
    public async void HandEntity_AddAndRetrieveHandCard_CardRetrievedSuccessfully()
    {
        // Arrange
        var handRepository = new HandRepository(_dbContext);
        var sampleHandCard = handRepository.CreateEntityWithId(SampleId);

        // Act
        await handRepository.AddAsync(sampleHandCard);
        var addedHandCards = handRepository.GetById(SampleId);

        // Assert
        addedHandCards.Should().NotBeNull();
        addedHandCards.Should().BeEquivalentTo(sampleHandCard);
        // Clean up 
        await handRepository.DeleteAllAsync();
    }
    
    [Theory]
    [InlineData( 1, 1, 1)]
    [InlineData( 2, 2, 1)]
    public async void HandEntity_AddUpdateAndRetrieveHandCard_CardUpdatedAndRetrievedSuccessfully(
        int newColor,
        int newValue,
        int newPlayerId)
    {
        // Arrange
        var handRepository = new HandRepository(_dbContext);
        var sampleHandCard = handRepository.CreateEntityWithId(SampleId);
        
        var sampleCardColor = sampleHandCard.CardColor; // Is 0
        var sampleCardValue = sampleHandCard.CardValue; // Is 0
        var sampleCardPlayerId = sampleHandCard.PlayerId; // Is 0
        
        // Act
        await handRepository.AddAsync(sampleHandCard);
        
        var addedHandCard = handRepository.GetById(SampleId);
        addedHandCard!.CardColor = newColor;
        addedHandCard!.CardValue = newValue;
        addedHandCard!.PlayerId = newPlayerId;
        
        await handRepository.UpdateAsync(addedHandCard);
        
        // Assert
        var updatedHandCard = handRepository.GetById(SampleId);
        
        updatedHandCard!.CardColor.ShouldNotBeEqualTo(sampleCardColor);
        updatedHandCard.CardValue.ShouldBeEqualTo(newColor);
        
        updatedHandCard!.CardValue.ShouldNotBeEqualTo(sampleCardValue);
        updatedHandCard.CardValue.ShouldBeEqualTo(newValue);
        
        updatedHandCard!.PlayerId.ShouldNotBeEqualTo(sampleCardPlayerId);
        updatedHandCard.PlayerId.ShouldBeEqualTo(newPlayerId);

        // Clean up 
        await handRepository.DeleteAllAsync();
    }
    
    [Fact]
    public async void HandEntity_AddAndDeleteHandCard_CardDeletedSuccessfully()
    {
        // Arrange
        var handRepository = new HandRepository(_dbContext);
        var sampleHandCard = handRepository.CreateEntityWithId(SampleId);
        
        // Act
        await handRepository.AddAsync(sampleHandCard);
        await handRepository.DeleteAsync(SampleId);

        // Assert
        var listWithAllHandCards = handRepository.GetAllAsync();
        listWithAllHandCards.Should().BeEmpty();
    }
}