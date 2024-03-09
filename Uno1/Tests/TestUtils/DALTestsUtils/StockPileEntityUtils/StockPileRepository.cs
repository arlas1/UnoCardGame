using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using DAL.DbEntities;

namespace Tests.TestUtils.DALTestsUtils.StockPileEntityUtils;

public class StockPileRepository(AppDbContext dbContext) : IStockPileRepository
{
    public async Task AddStockPileCardAsync(StockPile card)
    {
        await dbContext.StockPile.AddAsync(card);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateStockPileCardAsync(StockPile card)
    {
        dbContext.StockPile.Update(card);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteStockPileCardAsync(int id)
    {
        var card = dbContext.StockPile.SingleOrDefault(card => card.GameStateId == id)!;
        dbContext.StockPile.Remove(card);
        await dbContext.SaveChangesAsync();
    }

    public StockPile? GetStockPileById(int id)
    {
        return dbContext.StockPile.SingleOrDefault(card => card.GameStateId == id);
    }

    public List<StockPile> GetAllStockPileCardsAsync()
    {
        return dbContext.StockPile.ToList();
    }

    public async Task DeleteAllStockPileCardsAsync()
    {
        dbContext.StockPile.RemoveRange(dbContext.StockPile);
        await dbContext.SaveChangesAsync();
    }
}