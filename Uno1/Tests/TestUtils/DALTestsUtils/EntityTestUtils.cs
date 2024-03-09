using DAL.DbEntities;

namespace Tests.TestUtils.DALTestsUtils;

public static class EntityTestUtils
{
    public static Player CreatePlayerEntityWithId(int id)
    {
        return new Player()
        {
            Id = 0,
            Name = "p1",
            Type = 0,
            Role = 0,
            GameStateId = id
        };
    }
    
    public static StockPile CreateStockPileCardEntityWithId(int id)
    {
        return new StockPile()
        {
            Id = 0,
            CardColor = 0,
            CardValue = 0,
            GameStateId = id
        };
    }
}