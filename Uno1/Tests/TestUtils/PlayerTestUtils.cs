using Domain;

namespace Tests.TestUtils;

public static class PlayerTestUtils
{
    public static Player CreatePlayerHuman(int id, string name)
    {
        return new Player(id, name, Player.PlayerType.Human);
    }
    
    public static Player CreatePlayerAi(int id, string name)
    {
        return new Player(id, name, Player.PlayerType.Human);
    }

    public static void LoadHandWithAmountOfNumericCards(int cardsAmount, Player player)
    {
        for (var i = 0; i < cardsAmount; i++)
        {
            player.Hand.Add(i % 2 == 0 
                ? new UnoCard(UnoCard.Color.Red, UnoCard.Value.Zero)
                : new UnoCard(UnoCard.Color.Blue, UnoCard.Value.One)
            );
        }
    }
    
    public static void LoadHandWithAmountOfDrawTwoCards(int cardsAmount, Player player)
    {
        for (var i = 0; i < cardsAmount; i++)
        {
            player.Hand.Add(i % 2 == 0 
                ? new UnoCard(UnoCard.Color.Red, UnoCard.Value.DrawTwo)
                : new UnoCard(UnoCard.Color.Blue, UnoCard.Value.DrawTwo)
            );
        }
    }
    
    public static void LoadHandWithAmountOfSkipCards(int cardsAmount, Player player)
    {
        for (var i = 0; i < cardsAmount; i++)
        {
            player.Hand.Add(i % 2 == 0 
                ? new UnoCard(UnoCard.Color.Red, UnoCard.Value.Skip)
                : new UnoCard(UnoCard.Color.Blue, UnoCard.Value.Skip)
            );
        }
    }
    
    public static void LoadHandWithAmountOfReverseCards(int cardsAmount, Player player)
    {
        for (var i = 0; i < cardsAmount; i++)
        {
            player.Hand.Add(i % 2 == 0 
                ? new UnoCard(UnoCard.Color.Red, UnoCard.Value.Reverse)
                : new UnoCard(UnoCard.Color.Blue, UnoCard.Value.Reverse)
            );
        }
    }
    
    public static void LoadHandWithAmountOfWildCards(int cardsAmount, Player player)
    {
        for (var i = 0; i < cardsAmount; i++)
        {
            player.Hand.Add(i % 2 == 0 
                ? new UnoCard(UnoCard.Color.Red, UnoCard.Value.Wild)
                : new UnoCard(UnoCard.Color.Blue, UnoCard.Value.Wild)
            );
        }
    }
    
    public static void LoadHandWithAmountOfWildFourCards(int cardsAmount, Player player)
    {
        for (var i = 0; i < cardsAmount; i++)
        {
            player.Hand.Add(i % 2 == 0 
                ? new UnoCard(UnoCard.Color.Red, UnoCard.Value.WildFour)
                : new UnoCard(UnoCard.Color.Blue, UnoCard.Value.WildFour)
            );
        }
    }
}