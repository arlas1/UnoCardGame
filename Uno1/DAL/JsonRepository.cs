using System.Text.Json;
using Domain;
using UnoGameEngine;

namespace DAL;

public class JsonRepository
{
    public static void SaveIntoJson(GameEngine gameEngine)
    {
        var jsonOptions = new JsonSerializerOptions()
        {
            WriteIndented = true,
            AllowTrailingCommas = true,
        };

        var gameStateCopy = gameEngine.GetGameStateCopy();
        var jsonString = JsonSerializer.Serialize(gameStateCopy, jsonOptions);

        const string jsonFolderPath = @"C:\Users\lasim\RiderProjects\icd0008-23f\Uno1\DAL\JsonSaves/";
            
        // Get the count of existing JSON files in the folder
        var fileIndex = Directory.GetFiles(jsonFolderPath, "*.json").Length + 1;

        var filePath = Path.Combine(jsonFolderPath, $"{fileIndex}.json");

        File.WriteAllText(filePath, jsonString);
    }
    
    public static void LoadFromJson(string jsonString, GameEngine gameEngine)
    {
        var gameStateCopy = JsonSerializer.Deserialize<GameStateCopy>(jsonString)!;

        gameEngine.GameState.GameDirection = gameStateCopy.GameDirection;
        gameEngine.GameState.CurrentPlayerIndex = gameStateCopy.CurrentPlayerIndex;
        gameEngine.GameState.UnoDeck = gameStateCopy.UnoDeck!;
        gameEngine.GameState.StockPile = gameStateCopy.StockPile!;
        gameEngine.GameState.PlayersList = gameStateCopy.PlayersList!;
        gameEngine.GameState.CardColorChoice = gameStateCopy.CardColorChoice;
        gameEngine.GameState.IsColorChosen = gameStateCopy.IsColorChosen;
        gameEngine.GameState.SelectedCardIndex = gameStateCopy.SelectedCardIndex;
    }
}

