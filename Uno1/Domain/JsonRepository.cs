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

        var gameStateData = GameState.GetGameStateData();
        var jsonString = JsonSerializer.Serialize(gameStateData, jsonOptions);

        var jsonFolderPath = @"C:\Users\lasim\RiderProjects\icd0008-23f\Uno1\DAL\JsonSaves/";
            
        // Get the count of existing JSON files in the folder
        int fileIndex = Directory.GetFiles(jsonFolderPath, "*.json").Length + 1;

        var filePath = Path.Combine(jsonFolderPath, $"{fileIndex}.json");

        File.WriteAllText(filePath, jsonString);
    }
    
    
    public static GameStateData LoadFromJson(string jsonString)
    {
        return JsonSerializer.Deserialize<GameStateData>(jsonString)!;
    }
    
}

