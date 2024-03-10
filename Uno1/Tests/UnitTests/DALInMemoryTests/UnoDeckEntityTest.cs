using DAL;
using FluentAssert;
using FluentAssertions;
using Tests.TestUtils.DALTestsUtils.UnoDeckEntityTestUtils;

namespace Tests.UnitTests.DALInMemoryTests;

public class UnoDeckEntityTest
{
    private readonly AppDbContext _dbContext;
    private const int SampleId = 0;
    
    public UnoDeckEntityTest()
    {
        _dbContext = new AppDbContext();
        _dbContext.Database.EnsureCreated();
    }
    
    [Fact]
    public async void UnoDeckEntity_AddDeckCard_CardAddedSuccessfully()
    {
        // Arrange
        var unoDeckRepository = new UnoDeckRepository(_dbContext);
        var sampleDeckCard = unoDeckRepository.CreateEntityWithId(SampleId);

        // Act
        await unoDeckRepository.AddAsync(sampleDeckCard);

        // Assert
        var addedDeckCards = unoDeckRepository.GetAllAsync();
        addedDeckCards.Should()
                    .ContainSingle()
                    .Which
                    .Should()
                    .BeEquivalentTo(sampleDeckCard);
        
        // Clean up 
        await unoDeckRepository.DeleteAllAsync();
    }
    
    [Fact]
    public async void UnoDeckEntity_AddAndRetrieveDeckCard_CardRetrievedSuccessfully()
    {
        // Arrange
        var unoDeckRepository = new UnoDeckRepository(_dbContext);
        var sampleDeckCard = unoDeckRepository.CreateEntityWithId(SampleId);

        // Act
        await unoDeckRepository.AddAsync(sampleDeckCard);
        var addedDeckCards = unoDeckRepository.GetById(SampleId);

        // Assert
        addedDeckCards.Should().NotBeNull();
        addedDeckCards.Should().BeEquivalentTo(sampleDeckCard);
        // Clean up 
        await unoDeckRepository.DeleteAllAsync();
    }
    
    [Theory]
    [InlineData( 1, 1)]
    [InlineData( 2, 2)]
    public async void UnoDeckEntity_AddUpdateAndRetrieveDeckCard_CardUpdatedAndRetrievedSuccessfully(
        int newColor,
        int newValue)
    {
        // Arrange
        var unoDeckRepository = new UnoDeckRepository(_dbContext);
        var sampleDeckCard = unoDeckRepository.CreateEntityWithId(SampleId);
        
        var sampleCardColor = sampleDeckCard.CardColor; // Is 0
        var sampleCardValue = sampleDeckCard.CardValue; // Is 0
        
        // Act
        await unoDeckRepository.AddAsync(sampleDeckCard);
        
        var addedDeckCard = unoDeckRepository.GetById(SampleId);
        addedDeckCard!.CardColor = newColor;
        addedDeckCard!.CardValue = newValue;
        
        await unoDeckRepository.UpdateAsync(addedDeckCard);
        
        // Assert
        var updatedDeckCard = unoDeckRepository.GetById(SampleId);
        
        updatedDeckCard!.CardColor.ShouldNotBeEqualTo(sampleCardColor);
        updatedDeckCard.CardValue.ShouldBeEqualTo(newColor);
        
        updatedDeckCard!.CardValue.ShouldNotBeEqualTo(sampleCardValue);
        updatedDeckCard.CardValue.ShouldBeEqualTo(newValue);

        // Clean up 
        await unoDeckRepository.DeleteAllAsync();
    }
    
    [Fact]
    public async void UnoDeckEntity_AddAndDeleteHandDeckCard_HandCardDeletedSuccessfully()
    {
        // Arrange
        var unoDeckRepository = new UnoDeckRepository(_dbContext);
        var sampleDeckCard = unoDeckRepository.CreateEntityWithId(SampleId);
        
        // Act
        await unoDeckRepository.AddAsync(sampleDeckCard);
        await unoDeckRepository.DeleteAsync(SampleId);

        // Assert
        var listWithAllDeckCards = unoDeckRepository.GetAllAsync();
        listWithAllDeckCards.Should().BeEmpty();
    }
}