using System.Collections.Generic;
using System.Threading.Tasks;
using DAL.DbEntities;

namespace Tests.TestUtils.DALTestsUtils.PlayerEntityUtils;

public interface IPlayerRepository
{
    Task AddPlayerAsync(Player player);
    Task UpdatePlayerAsync(Player player);
    Task DeletePlayerAsync(int playerId);
    Player? GetPlayerById(int id);
    List<Player> GetAllPlayersAsync();
    Task DeleteAllPlayersAsync();
}