using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using DAL.DbEntities;

namespace Tests.TestUtils.DALTestsUtils.PlayerEntityUtils;

public class PlayerRepository(AppDbContext dbContext) : IPlayerRepository
{
    public async Task AddPlayerAsync(Player player)
    {
        await dbContext.Players.AddAsync(player);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdatePlayerAsync(Player player)
    {
        dbContext.Players.Update(player);
        await dbContext.SaveChangesAsync();
    }
    
    public async Task DeletePlayerAsync(int id)
    {
        var player = dbContext.Players.SingleOrDefault(player => player.GameStateId == id)!;
        dbContext.Players.Remove(player);
        await dbContext.SaveChangesAsync();
    }

    public Player? GetPlayerById(int id)
    {
        return dbContext.Players.SingleOrDefault(player => player.GameStateId == id);
    }

    public List<Player> GetAllPlayersAsync()
    {
        return dbContext.Players.ToList();
    }
    
    public async Task DeleteAllPlayersAsync()
    {
        dbContext.Players.RemoveRange(dbContext.Players);
        await dbContext.SaveChangesAsync();
    }
}
