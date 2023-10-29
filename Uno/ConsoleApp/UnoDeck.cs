using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp
{
    public class UnoDeck
    {
        private UnoCard[] _cards;
        public static int cardsInDeck;

        public UnoDeck()
        {
            _cards = new UnoCard[108];
            cardsInDeck = 0; 
        }

        public void Reset()
        {
            cardsInDeck = 0;
            UnoCard.Color[] colors = { UnoCard.Color.Red, UnoCard.Color.Blue, UnoCard.Color.Green, UnoCard.Color.Yellow };

            foreach (UnoCard.Color color in colors)
            {
                _cards[cardsInDeck++] = new UnoCard(color, UnoCard.Value.Zero);

                for (int j = 1; j <= 9; j++)
                {
                    _cards[cardsInDeck++] = new UnoCard(color, (UnoCard.Value)j);
                    _cards[cardsInDeck++] = new UnoCard(color, (UnoCard.Value)j);
                }

                UnoCard.Value[] values = { UnoCard.Value.DrawTwo, UnoCard.Value.Skip, UnoCard.Value.Reverse };
                foreach (UnoCard.Value value in values)
                {
                    _cards[cardsInDeck++] = new UnoCard(color, value);
                    _cards[cardsInDeck++] = new UnoCard(color, value);
                }
            }

            for (int i = 0; i < 4; i++) // Four Wild and WildFour cards
            {
                _cards[cardsInDeck++] = new UnoCard(UnoCard.Color.Wild, UnoCard.Value.Wild);
                _cards[cardsInDeck++] = new UnoCard(UnoCard.Color.Wild, UnoCard.Value.WildFour);
            }
        }
        


        public void ReplaceDeckWith(List<UnoCard> newCards)
        {
            _cards = newCards.ToArray();
            cardsInDeck = _cards.Length;
        }

        public bool IsEmpty()
        {
            return cardsInDeck == 0;
        }

        public void Shuffle()
        {
            int n = _cards.Length;
            Random random = new Random();

            for (int i = 0; i < _cards.Length; i++)
            {
                int randomValue = random.Next(n - 1);
                (_cards[i], _cards[randomValue]) = (_cards[randomValue], _cards[i]);
            }
        }

        public UnoCard DrawCard()
        {
            if (IsEmpty())
            {
                throw new InvalidOperationException("Cannot draw a card since there are no cards in the deck.");
            }
            return _cards[--cardsInDeck];
        }

        public UnoCard[] DrawCard(int n)
        {
            if (n < 0)
            {
                throw new ArgumentException("Must draw a positive number of cards but tried to draw " + n + " _cards.");
            }

            if (n > cardsInDeck)
            {
                throw new ArgumentException("Cannot draw " + n + " cards since there are only " + cardsInDeck + " _cards.");
            }

            UnoCard[] drawnCards = new UnoCard[n];

            for (int i = 0; i < n; i++)
            {
                drawnCards[i] = _cards[--cardsInDeck];
            }

            return drawnCards;
        }
        
        public List<UnoCard> GetAllCards()
        {
            return _cards.Take(cardsInDeck).ToList();
        }
    }
}
