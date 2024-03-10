using DAL;
using DAL.DbEntities;
using Tests.TestUtils.DALTestsUtils.Contracts;

namespace Tests.TestUtils.DALTestsUtils.Repositories;

public class UnoDeckRepository(AppDbContext dbContext) : IUnoDeckRepository
{
    public UnoDeck CreateEntityWithId(int id)
    {
        return new UnoDeck()
        {
            Id = 0,
            CardColor = 0,
            CardValue = 0,
            GameStateId = id
        };
    }
    
    public async Task AddAsync(UnoDeck deckCard)
    {
        await dbContext.UnoDecks.AddAsync(deckCard);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(UnoDeck deckCard)
    {
        dbContext.UnoDecks.Update(deckCard);
        await dbContext.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(int id)
    {
        var deckCard = dbContext.UnoDecks.SingleOrDefault(card => card.GameStateId == id)!;
        dbContext.UnoDecks.Remove(deckCard);
        await dbContext.SaveChangesAsync();
    }

    public UnoDeck? GetById(int id)
    {
        return dbContext.UnoDecks.SingleOrDefault(card => card.GameStateId == id);
    }

    public IEnumerable<UnoDeck> GetAllAsync()
    {
        return dbContext.UnoDecks.ToList();
    }
    
    public async Task DeleteAllAsync()
    {
        dbContext.UnoDecks.RemoveRange(dbContext.UnoDecks);
        await dbContext.SaveChangesAsync();
    }
}