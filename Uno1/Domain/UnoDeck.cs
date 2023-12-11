
namespace Domain
{
    public class UnoDeck
    {
        private UnoCard[]? Deck = new UnoCard[108];
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

            for (var i = 0; i < 4; i++) // Four Wild and WildFour cards
            {
                AddCardToDeck(new UnoCard(UnoCard.Color.Wild, UnoCard.Value.Wild));
                AddCardToDeck(new UnoCard(UnoCard.Color.Wild, UnoCard.Value.WildFour));
            }

            var valueToAvoid = GameConfiguration.PromptForValueToAvoid();
        
            // Remove cards with the specified value
            Cards.RemoveAll(card => card.CardValue == valueToAvoid);

        }
        
        public void AddCardToDeck(UnoCard card)
        {
            Deck = Deck!.Append(card).ToArray();
            Cards.Add(card);
        }

        
        public bool IsEmpty()
        {
            return Cards.Count == 0;
        }

        
        public void Shuffle()
        {
            Deck = Cards.ToArray();
            var n = Deck.Length;
            Random random = new Random();

            for (var i = 0; i < Deck.Length; i++)
            {
                var randomValue = random.Next(n - 1);
                (Deck[i], Deck[randomValue]) = (Deck[randomValue], Deck[i]);
            }

            Cards = Deck.ToList();
        }

        
        public UnoCard DrawCard()
        {
            if (IsEmpty())
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
            Deck = Cards.ToArray();
        }
    }
}
