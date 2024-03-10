namespace Tests.TestUtils.DALTestsUtils;

public interface IBaseRepository<TEntity>
{
    TEntity CreateEntityWithId(int id);
    
    Task AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(int id);
    TEntity? GetById(int id);
    IEnumerable<TEntity> GetAllAsync();
    Task DeleteAllAsync();
}