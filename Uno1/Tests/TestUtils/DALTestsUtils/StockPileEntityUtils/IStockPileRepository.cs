using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.DbEntities;

namespace Tests.TestUtils.DALTestsUtils.StockPileEntityUtils;

public interface IStockPileRepository
{
    Task AddStockPileCardAsync(StockPile player);
    Task UpdateStockPileCardAsync(StockPile player);
    Task DeleteStockPileCardAsync(int playerId);
    StockPile? GetStockPileById(int id);
    List<StockPile> GetAllStockPileCardsAsync();
    Task DeleteAllStockPileCardsAsync();
}