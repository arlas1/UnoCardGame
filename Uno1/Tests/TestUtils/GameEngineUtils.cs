using Domain;
using UnoGameEngine;

namespace Tests.TestUtils;

public static class GameEngineUtils
{
    public static GameEngine CreateWithColorChoice(UnoCard.Color color)
    {
        return new GameEngine
        {
            GameState =
            {
                CardColorChoice = color
            }
        };
    }

    public static GameEngine CreateWithRequiredGameDirection(bool direction)
    {
        return new GameEngine 
        { 
            GameState =
            {
                GameDirection = direction,
            }
        };
    }
    
    public static GameEngine CreateWithAllPropertiesSet()
    {
        var sampleGameEngine = new GameEngine 
        { 
            GameState =
            {
                GameDirection = false,
                CurrentPlayerIndex = 0,
                UnoDeck = new UnoDeck(),
                StockPile = [],
                PlayersList = [],
                CardColorChoice = 0,
                IsColorChosen = false,
                SelectedCardIndex = 0
            }
        };
        
        sampleGameEngine.GameState.UnoDeck.Clear();
        sampleGameEngine.GameState.UnoDeck.Create();

        return sampleGameEngine;
    }
    
    public static GameEngine CreateWithUnoDeckWith5LastCardsToAvoid()
    {
        var gameEngine = new GameEngine();
        
        gameEngine.GameState.UnoDeck.Clear();
        gameEngine.GameState.UnoDeck.Create();

        gameEngine.GameState.UnoDeck.Cards[^1] = new UnoCard(UnoCard.Color.Red, UnoCard.Value.Skip);
        gameEngine.GameState.UnoDeck.Cards[^2] = new UnoCard(UnoCard.Color.Red, UnoCard.Value.Reverse);
        gameEngine.GameState.UnoDeck.Cards[^3] = new UnoCard(UnoCard.Color.Red, UnoCard.Value.DrawTwo);
        gameEngine.GameState.UnoDeck.Cards[^4] = new UnoCard(UnoCard.Color.Red, UnoCard.Value.Wild);
        gameEngine.GameState.UnoDeck.Cards[^5] = new UnoCard(UnoCard.Color.Red, UnoCard.Value.WildFour);
        gameEngine.GameState.UnoDeck.Cards[^6] = new UnoCard(UnoCard.Color.Red, UnoCard.Value.Zero);
        
        return gameEngine;
    }
    
    public static bool NoCardsWithValue(GameEngine gameEngine, UnoCard.Value? valueToAvoid)
    {
        return gameEngine.GameState.UnoDeck.Cards.All(card => card.CardValue != valueToAvoid);
    }
    
    public static void LoadAmountOfHumanPlayers(int amount, GameEngine gameEngine)
    {
        for (int i = 0; i < amount; i++)
        {
            gameEngine.GameState.PlayersList.Add(PlayerUtils.CreateHumanType(i,"p" + i));
        }
    }
    
    // public static void LoadAmountOfAiPlayers(int amount, GameEngine gameEngine)
    // {
    //     for (int i = 0; i < amount; i++)
    //     {
    //         gameEngine.GameState.PlayersList.Add(SamplePlayer.CreateAiType(0,"p" + i));
    //     }
    // }
}