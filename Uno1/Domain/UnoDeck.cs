namespace Domain;


public class UnoDeck
{
    private static UnoCard[]? Deck;
    public int CardsInDeck;

    public UnoDeck()
    {
        Deck = new UnoCard[108];
        CardsInDeck = 0;
    }

    public void Create()
    {
        CardsInDeck = 0;
        UnoCard.Color[] colors = { UnoCard.Color.Red, UnoCard.Color.Blue, UnoCard.Color.Green, UnoCard.Color.Yellow };

        foreach (var color in colors)
        {
            Deck![CardsInDeck++] = new UnoCard(color, UnoCard.Value.Zero);

            for (var j = 1; j <= 9; j++)
            {
                Deck[CardsInDeck++] = new UnoCard(color, (UnoCard.Value)j);
                Deck[CardsInDeck++] = new UnoCard(color, (UnoCard.Value)j);
            }

            UnoCard.Value[] values = { UnoCard.Value.DrawTwo, UnoCard.Value.Skip, UnoCard.Value.Reverse };
            foreach (var value in values)
            {
                Deck[CardsInDeck++] = new UnoCard(color, value);
                Deck[CardsInDeck++] = new UnoCard(color, value);
            }
        }

        for (var i = 0; i < 4; i++) // Four Wild and WildFour cards
        {
            Deck![CardsInDeck++] = new UnoCard(UnoCard.Color.Wild, UnoCard.Value.Wild);
            Deck[CardsInDeck++] = new UnoCard(UnoCard.Color.Wild, UnoCard.Value.WildFour);
        }
    }

    public bool IsEmpty()
    {
        return CardsInDeck == 0;
    }

    public void Shuffle()
    {
        var n = Deck!.Length;
        Random random = new Random();

        for (var i = 0; i < Deck.Length; i++)
        {
            var randomValue = random.Next(n - 1);
            (Deck[i], Deck[randomValue]) = (Deck[randomValue], Deck[i]);
        }
    }

    public UnoCard DrawCard()
    {
        if (IsEmpty())
        {
            throw new InvalidOperationException("Cannot draw a card since there are no cards in the deck.");
        }
        return Deck![--CardsInDeck];
    }
}