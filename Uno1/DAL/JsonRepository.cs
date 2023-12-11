using System.Text.Json;
using Domain;

namespace DAL;

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
    
    public static void LoadFromJson(string jsonString)
    {
        var gameStateCopy = JsonSerializer.Deserialize<GameStateCopy>(jsonString)!;

        GameState.GameDirection = gameStateCopy.GameDirection;
        GameState.CurrentPlayerIndex = gameStateCopy.CurrentPlayerIndex;
        GameState.UnoDeck = gameStateCopy.UnoDeck!;
        GameState.StockPile = gameStateCopy.StockPile!;
        GameState.PlayersList = gameStateCopy.PlayersList!;
        GameState.CardColorChoice = gameStateCopy.CardColorChoice;
        GameState.IsColorChosen = gameStateCopy.IsColorChosen;
        GameState.SelectedCardIndex = gameStateCopy.SelectedCardIndex;
    }
    
}

