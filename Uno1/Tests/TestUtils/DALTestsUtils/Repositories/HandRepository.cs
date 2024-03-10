using DAL;
using DAL.DbEntities;
using Tests.TestUtils.DALTestsUtils.Contracts;

namespace Tests.TestUtils.DALTestsUtils.Repositories;

public class HandRepository(AppDbContext dbContext) : IHandRepository
{
    public Hand CreateEntityWithId(int id)
    {
        return new Hand()
        {
            Id = 0,
            CardColor = 0,
            CardValue = 0,
            PlayerId = 0,
            GameStateId = id
        };
    }
    
    public async Task AddAsync(Hand handCard)
    {
        await dbContext.Hands.AddAsync(handCard);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Hand handCard)
    {
        dbContext.Hands.Update(handCard);
        await dbContext.SaveChangesAsync();
    }
    
    public async Task DeleteAsync(int id)
    {
        var handCard = dbContext.Hands.SingleOrDefault(handCard => handCard.GameStateId == id)!;
        dbContext.Hands.Remove(handCard);
        await dbContext.SaveChangesAsync();
    }

    public Hand? GetById(int id)
    {
        return dbContext.Hands.SingleOrDefault(handCard => handCard.GameStateId == id);
    }

    public IEnumerable<Hand> GetAllAsync()
    {
        return dbContext.Hands.ToList();
    }
    
    public async Task DeleteAllAsync()
    {
        dbContext.Hands.RemoveRange(dbContext.Hands);
        await dbContext.SaveChangesAsync();
    }
}