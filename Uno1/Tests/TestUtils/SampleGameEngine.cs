using Domain;
using UnoGameEngine;

namespace Tests.TestUtils;

public static class SampleGameEngine
{
    public static GameEngine GetWithoutSamplePlayersData()
    {
        return new GameEngine();
    }
    
    public static GameEngine GetWithUnoDeckWith5LastCardsToAvoid()
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

    public static bool HaveNoCardsWithValueToAvoid(GameEngine gameEngine, UnoCard.Value? valueToAvoid)
    {
        return gameEngine.GameState.UnoDeck.Cards.All(card => card.CardValue != valueToAvoid);
    }
    
    public static void LoadAmountOfHumanPlayers(int playersAmount, GameEngine gameEngine)
    {
        for (int i = 0; i < playersAmount; i++)
        {
            gameEngine.GameState.PlayersList.Add(SamplePlayer.GetHumanType(0,"p" + i));
        }
    }
    
    public static void LoadAmountOfAiPlayers(int playersAmount, GameEngine gameEngine)
    {
        for (int i = 0; i < playersAmount; i++)
        {
            gameEngine.GameState.PlayersList.Add(SamplePlayer.GetAiType(0,"p" + i));
        }
    }
}