using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;


namespace ConsoleApp;


public class GameRepositoryFileSystem : IGameRepository
{
    // TODO: figure out system dependent location - maybe Path.GetTempPath()
    private const string SaveLocation = "Users\\lasim\\RiderProjects\\firstProject\\Domain";

    public void Save(Guid id, GameState state)
    {
        var content = JsonSerializer.Serialize(state, JsonHelpers.JsonSerializerOptions);

        var fileName = Path.ChangeExtension(id.ToString(), ".json");

        if (!Path.Exists(SaveLocation))
        {
            Directory.CreateDirectory(SaveLocation);
        }

        File.WriteAllText(Path.Combine(SaveLocation, fileName), content);
    }

    public List<string> GetSaveGames()
    {
        return Directory.EnumerateFiles(SaveLocation).ToList();
    }
}