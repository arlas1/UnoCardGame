using System;
using System.Collections.Generic;

namespace ConsoleApp
{
 public class Game
    {
        public static int _currentPlayer;
        public static string[] _playerIds = null!;
        public static UnoDeck _deck = null!;

        private static List<List<UnoCard>> _playerHand = null!;
        public static List<UnoCard> _stockPile = null!;

        public static UnoCard.Color _validColor;
        public static UnoCard.Value _validValue;

        public static bool _gameDirection;

        public Game(string[] pids)
        {
            _deck = new UnoDeck();
            _deck.Shuffle();
            _stockPile = new List<UnoCard>();

            _playerIds = pids;
            _currentPlayer = 0;
            _gameDirection = false; // false - clockwise, true - counterclockwise

            _playerHand = new List<List<UnoCard>>();

            //for (int i = 0; i < pids.Length; i++)
            //{
            //    List<UnoCard> hand = new List<UnoCard>(_deck.DrawCard(7));
            //    _playerHand.Add(hand);
            //}
        }

        public void Start(Game game)
        {
            UnoCard card = _deck.DrawCard();
            _validColor = card.CardColor;
            _validValue = card.CardValue;

            if (card.CardValue == UnoCard.Value.Wild)
            {
                Start(game);
            }

            if (card.CardValue == UnoCard.Value.DrawTwo || card.CardValue == UnoCard.Value.WildFour)
            {
                Start(game);
            }

            if (card.CardValue == UnoCard.Value.Skip)
            {
                var message = $"Player {_playerIds[_currentPlayer]} was skipped.";
                Console.WriteLine(message);
                GameDirectionCheck();
            }

            if (card.CardValue == UnoCard.Value.Reverse)
            {
                var message = $"{_playerIds[_currentPlayer]} The game direction reversed.";
                Console.WriteLine(message);
                _gameDirection ^= true;
                _currentPlayer = _playerIds.Length - 1;
            }

            _stockPile.Add(card);
        }

        public UnoCard GetTopCard()
        {
            return new UnoCard(_validColor, _validValue);
        }

        public bool IsGameOver()
        {
            foreach (var player in _playerIds)
            {
                if (HasEmptyHand(player))
                {
                    return true;
                }
            }

            return false;
        }

        public string GetCurrentPlayer()
        {
            return _playerIds[_currentPlayer];
        }

        public string GetPreviousPlayer(int i)
        {
            int index = _currentPlayer - 1;

            if (index == -1)
            {
                index = _playerIds.Length - 1;
            }

            return _playerIds[index];
        }

        public static List<UnoCard> GetPlayerHand(string pid)
        {
            var index = Array.IndexOf(_playerIds, pid);
            return _playerHand[index];
        }

        public int GetPlayerHandSize(string pid)
        {
            return GetPlayerHand(pid).Count;
        }

        public UnoCard GetPlayerCard(string pid, int choice)
        {
            List<UnoCard> hand = GetPlayerHand(pid);
            return hand[choice];
        }

        public static bool HasEmptyHand(string pid)
        {
            return GetPlayerHand(pid).Count == 0;
        }

        public static bool ValidCardPlay(UnoCard card)
        {
            return card.CardColor == _validColor || card.CardValue == _validValue;
        }

        public static void CheckPlayerTurn(string pid)
        {
            if (_playerIds[_currentPlayer] != pid)
            {
                throw new InvalidPlayerTurnException($"It's not {pid} turn.", pid);
            }
        }

        public void SubmitDraw(string pid)
        {
            CheckPlayerTurn(pid);

            if (_deck.IsEmpty())
            {
                _deck.ReplaceDeckWith(_stockPile);
                _deck.Shuffle();
            }

            GetPlayerHand(pid).Add(_deck.DrawCard());
            GameDirectionCheck();
        }

        public static void GameDirectionCheck()
        {
            if (!_gameDirection)
            {
                _currentPlayer = (_currentPlayer + 1) % _playerIds.Length;
            }

            if (_gameDirection)
            {
                _currentPlayer = (_currentPlayer - 1) % _playerIds.Length;
                if (_currentPlayer == -1)
                {
                    _currentPlayer = _playerIds.Length - 1;
                }
            }
        }

        public void SetCardColor(UnoCard.Color color)
        {
            _validColor = color;
        }


        public static string GetNextPlayer()
        {
            int nextPlayerIndex = _gameDirection ? (_currentPlayer - 1 + _playerIds.Length) % _playerIds.Length : (_currentPlayer + 1) % _playerIds.Length;
            return _playerIds[nextPlayerIndex];
        }

        public static bool IsValidCardPlay(UnoCard card, UnoCard topCard)
        {
            return card.CardColor == topCard.CardColor || card.CardValue == topCard.CardValue || card.CardColor == UnoCard.Color.Wild;
        }
    }   
}
