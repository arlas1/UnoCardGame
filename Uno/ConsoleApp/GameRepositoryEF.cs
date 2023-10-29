using System;
using System.Collections.Generic;


namespace ConsoleApp;

public class GameRepositoryEF : IGameRepository
{
    public void Save(Guid id, GameState state)
    {
        throw new NotImplementedException();
    }

    public List<string> GetSaveGames()
    {
        throw new NotImplementedException();
    }
}