using System.Text.Json;

namespace Domain;

public class JsonRepository
{
    public static void SaveIntoJson()
    {
        var jsonOptions = new JsonSerializerOptions()
        {
            WriteIndented = true,
            AllowTrailingCommas = true,
        };

        var gameStateCopy = GameState.GetGameStateCopy();
        var jsonString = JsonSerializer.Serialize(gameStateCopy, jsonOptions);

        const string jsonFolderPath = @"C:\Users\lasim\RiderProjects\icd0008-23f\Uno1\DAL\JsonSaves/";
            
        // Get the count of existing JSON files in the folder
        var fileIndex = Directory.GetFiles(jsonFolderPath, "*.json").Length + 1;

        var filePath = Path.Combine(jsonFolderPath, $"{fileIndex}.json");

        File.WriteAllText(filePath, jsonString);
    }
    
    
    public static GameStateCopy LoadFromJson(string jsonString)
    {
        return JsonSerializer.Deserialize<GameStateCopy>(jsonString)!;
    }
    
}

