using System.Text.Json;
using Domain;
using UnoGameEngine;

namespace DAL;

public static class JsonRepository
{
    public static void SaveIntoJson(GameEngine gameEngine)
    {
        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            AllowTrailingCommas = true
        };

        var gameStateCopy = gameEngine.GetGameStateCopy();
        var jsonString = JsonSerializer.Serialize(gameStateCopy, jsonOptions);
        
        var jsonFolderPath = GetPathForTheJsonSaves();
    
        var fileAmount = Directory.GetFiles(jsonFolderPath, "*.json").Length + 1;
        var filePath = Path.Combine(jsonFolderPath, $"{fileAmount}.json");

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

    public static string GetPathForTheJsonSaves()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var solutionDirectory = Directory.GetParent(currentDirectory)?.Parent?.Parent?.FullName;
        return Path.Combine(solutionDirectory!, "..", "DAL", "JsonSaves");
    }

}

