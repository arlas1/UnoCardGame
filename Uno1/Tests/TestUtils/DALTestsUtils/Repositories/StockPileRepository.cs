using DAL;
using DAL.DbEntities;
using Tests.TestUtils.DALTestsUtils.Contracts;

namespace Tests.TestUtils.DALTestsUtils.Repositories;

public class StockPileRepository(AppDbContext dbContext) : IStockPileRepository
{
    public StockPile CreateEntityWithId(int id)
    {
        return new StockPile()
        {
            Id = 0,
            CardColor = 0,
            CardValue = 0,
            GameStateId = id
        };
    }
    
    public async Task AddAsync(StockPile card)
    {
        await dbContext.StockPiles.AddAsync(card);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(StockPile card)
    {
        dbContext.StockPiles.Update(card);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var card = dbContext.StockPiles.SingleOrDefault(card => card.GameStateId == id)!;
        dbContext.StockPiles.Remove(card);
        await dbContext.SaveChangesAsync();
    }

    public StockPile? GetById(int id)
    {
        return dbContext.StockPiles.SingleOrDefault(card => card.GameStateId == id);
    }

    public IEnumerable<StockPile> GetAllAsync()
    {
        return dbContext.StockPiles.ToList();
    }

    public async Task DeleteAllAsync()
    {
        dbContext.StockPiles.RemoveRange(dbContext.StockPiles);
        await dbContext.SaveChangesAsync();
    }
}