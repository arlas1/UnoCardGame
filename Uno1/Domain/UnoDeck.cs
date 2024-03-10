using Microsoft.IdentityModel.Tokens;

namespace Domain;

public class UnoDeck
{
    private UnoCard[]? _deck = new UnoCard[108];
    public List<UnoCard> Cards { get; private set; } = new();
        
    public UnoCard[] SerializedCards
    {
        get => Cards.ToArray();
        set => Cards = value.ToList();
    }
    
    public void Create()
    {
        Cards.Clear();
        UnoCard.Color[] colors = { UnoCard.Color.Red, UnoCard.Color.Blue, UnoCard.Color.Green, UnoCard.Color.Yellow };

        foreach (var color in colors)
        {
            AddCardToDeck(new UnoCard(color, UnoCard.Value.Zero));

            for (var j = 1; j <= 9; j++)
            {
                AddCardToDeck(new UnoCard(color, (UnoCard.Value)j));
                AddCardToDeck(new UnoCard(color, (UnoCard.Value)j));
            }

            UnoCard.Value[] values = { UnoCard.Value.DrawTwo, UnoCard.Value.Skip, UnoCard.Value.Reverse };
            foreach (var value in values)
            {
                AddCardToDeck(new UnoCard(color, value));
                AddCardToDeck(new UnoCard(color, value));
            }
        }

        for (var i = 0; i < 4; i++)
        {
            AddCardToDeck(new UnoCard(UnoCard.Color.Wild, UnoCard.Value.Wild));
            AddCardToDeck(new UnoCard(UnoCard.Color.Wild, UnoCard.Value.WildFour));
        }
    }
    
    public void AddCardToDeck(UnoCard card)
    {
        _deck = _deck!.Append(card).ToArray();
        Cards.Add(card);
    }
    
    public void RemoveCardsWithValue(UnoCard.Value? valueToRemove)
    {
        Cards.RemoveAll(card => card.CardValue == valueToRemove);
        _deck = Cards.ToArray();
    }
    
    public bool IsEmpty()
    {
        return Cards.Count == 0;
    }
    
    public void Shuffle()
    {
        _deck = Cards.ToArray();
        var n = _deck.Length;
        Random random = new Random();

        for (var i = 0; i < _deck.Length; i++)
        {
            var randomValue = random.Next(n - 1);
            (_deck[i], _deck[randomValue]) = (_deck[randomValue], _deck[i]);
        }

        Cards = _deck.ToList();
    }
    
    public UnoCard DrawCard()
    {
        if (Cards.IsNullOrEmpty())
        {
            throw new InvalidOperationException("Cannot draw a card since there are no cards in the deck.");
        }

        var card = Cards.Last();
        Cards.RemoveAt(Cards.Count - 1);
        return card;
    }
    
    public void Clear()
    {
        Cards.Clear();
        _deck = Cards.ToArray();
    }


}