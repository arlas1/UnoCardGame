using DAL;
using DAL.DbEntities;

namespace Tests.TestUtils.DALTestsUtils.GameStateEntityTestUtils;

public class GameStateRepository(AppDbContext dbContext) : IGameStateRepository
{
    public GameState CreateEntityWithId(int id)
    {
        return new GameState()
        {
           Id = 0,
           GameDirection = 0,
           CurrentPlayerIndex = 0,
           IsColorChosen = 0,
           SelectedCardIndex = 0,
           CardColorChoice = 0,
           MaxCardAmount = 0,
           PlayersMaxAmount = 0,
           IsGameStarted = 0,
           IsGameEnded = 0,
           WinnerId = id,
           ConsoleSaved = 0,
        };
    }
    
    public async Task AddAsync(GameState gameState)
    {
        await dbContext.GameStates.AddAsync(gameState);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(GameState gameState)
    {
        dbContext.GameStates.Update(gameState);
        await dbContext.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(int id)
    {
        var gameState = dbContext.GameStates.SingleOrDefault(gameState => gameState.WinnerId == id)!;
        dbContext.GameStates.Remove(gameState);
        await dbContext.SaveChangesAsync();
    }

    public GameState? GetById(int id)
    {
        return dbContext.GameStates.SingleOrDefault(gameState => gameState.WinnerId == id);
    }

    public IEnumerable<GameState> GetAllAsync()
    {
        return dbContext.GameStates.ToList();
    }
    
    public async Task DeleteAllAsync()
    {
        dbContext.GameStates.RemoveRange(dbContext.GameStates);
        await dbContext.SaveChangesAsync();
    }
}