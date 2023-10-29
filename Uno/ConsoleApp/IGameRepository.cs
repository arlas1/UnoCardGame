using System;
using System.Collections.Generic;


namespace ConsoleApp;

public interface IGameRepository
{
    void Save(Guid id, GameState state);
    List<String> GetSaveGames();
}