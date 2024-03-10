using DAL;
using DAL.DbEntities;
using Tests.TestUtils.DALTestsUtils.Contracts;

namespace Tests.TestUtils.DALTestsUtils.Repositories;

public class PlayerRepository(AppDbContext dbContext) : IPlayerRepository
{
    public Player CreateEntityWithId(int id)
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
    
    public async Task AddAsync(Player player)
    {
        await dbContext.Players.AddAsync(player);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Player player)
    {
        dbContext.Players.Update(player);
        await dbContext.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(int id)
    {
        var player = dbContext.Players.SingleOrDefault(player => player.GameStateId == id)!;
        dbContext.Players.Remove(player);
        await dbContext.SaveChangesAsync();
    }

    public Player? GetById(int id)
    {
        return dbContext.Players.SingleOrDefault(player => player.GameStateId == id);
    }

    public IEnumerable<Player> GetAllAsync()
    {
        return dbContext.Players.ToList();
    }
    
    public async Task DeleteAllAsync()
    {
        dbContext.Players.RemoveRange(dbContext.Players);
        await dbContext.SaveChangesAsync();
    }
}
