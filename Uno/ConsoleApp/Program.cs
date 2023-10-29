using System;
using System.Collections.Generic;

namespace ConsoleApp
{
    class Program
    {
        static UnoDeck _unoDeck = new UnoDeck();

        static void Main(string[] args)
        {
            var gameOptions = new GameOptions();
            IGameRepository gameRepository = new GameRepositoryFileSystem();
            var mainMenu = ProgramMenus.GetMainMenu(gameOptions, ProgramMenus.GetOptionsMenu(gameOptions), NewGame,
                LoadGame);

            // ================== MAIN =====================
            mainMenu.Run();

            // ================ THE END ==================
            return;
        }

        // ================== NEW GAME =====================
        static string? NewGame()
        {
            int numPlayers = PromptForNumberOfPlayers();
            if (numPlayers < 2 || numPlayers > 7)
            {
                Console.WriteLine("Invalid number of players. Please choose between 2 and 7.");
                return null;
            }
                
            _unoDeck.Reset();
            _unoDeck.Shuffle();

            List<UnoCard> stockPile = new List<UnoCard>();
            List<List<UnoCard>> playerHands = new List<List<UnoCard>>();
            string[] playerIds = new string[numPlayers];

            for (int i = 0; i < numPlayers; i++)
            {
                playerHands.Add(new List<UnoCard>());
                playerIds[i] = (i + 1).ToString(); // for start in the future
            }
           
            UnoCard topCard;
    
            do
            {
                topCard = _unoDeck.DrawCard();

                if (topCard.CardValue == UnoCard.Value.Wild || topCard.CardValue == UnoCard.Value.DrawTwo || topCard.CardValue == UnoCard.Value.WildFour)
                {
                    // Invalid starting card, draw another
                    continue;
                }

                if (topCard.CardValue == UnoCard.Value.Skip)
                {
                    var message = $"Player {playerIds[0]} was skipped.";
                    Console.WriteLine(message);
                    GameDirectionCheck(playerIds, true);
                }

                if (topCard.CardValue == UnoCard.Value.Reverse)
                {
                    var message = $"{playerIds[0]} The game direction reversed.";
                    Console.WriteLine(message);
                    GameDirectionCheck(playerIds, false);
                    playerIds = playerIds.Reverse().ToArray();
                }

                stockPile.Add(topCard);
            }
            while (topCard.CardValue == UnoCard.Value.Wild || topCard.CardValue == UnoCard.Value.DrawTwo || topCard.CardValue == UnoCard.Value.Skip || topCard.CardValue == UnoCard.Value.Reverse);

            //UnoCard initialCard = _unoDeck.DrawCard();
            //stockPile.Add(initialCard);

            
            // Deal initial cards to all players
            for (int i = 0; i < 7; i++)
            {
                foreach (var playerHand in playerHands)
                {
                    playerHand.Add(_unoDeck.DrawCard());
                }
            }
            
            int currentPlayerIndex = 0;

            while (true)
            {
                Console.Clear();

                Console.WriteLine("=======================");
                Console.WriteLine("Cards in deck left: " + UnoDeck.cardsInDeck);
                Console.WriteLine("=======================");
                Console.WriteLine("Top card --> " + stockPile.Last() + " <--");
                Console.WriteLine("=======================");

                List<UnoCard> currentPlayerHand = playerHands[currentPlayerIndex];

                Console.WriteLine($"Player {currentPlayerIndex + 1}'s hand:");
                for (int i = 0; i < currentPlayerHand.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {currentPlayerHand[i]}");
                }

                Console.WriteLine("=======================");

                Console.WriteLine($"Enter the number of the card you want to play (or 0 to draw a card): ");
                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    continue;
                }

                if (choice == 0)
                {
                    UnoCard drawnCard = _unoDeck.DrawCard();
                    Console.WriteLine($"Player {currentPlayerIndex + 1} drew a card: {drawnCard}");
                    currentPlayerHand.Add(drawnCard);
                }
                else if (choice >= 1 && choice <= currentPlayerHand.Count)
                {
                    UnoCard selectedCard = currentPlayerHand[choice - 1];

                    if (Game.IsValidCardPlay(selectedCard, stockPile.Last()))
                    {
                        currentPlayerHand.RemoveAt(choice - 1);
                        stockPile.Add(selectedCard);

                        //SubmitPlayerCard(Game._playerIds[currentPlayerIndex], selectedCard, selectedCard.CardColor);
                        if (currentPlayerHand.Count == 0)
                        {
                            Console.WriteLine($"Player {currentPlayerIndex + 1} wins! Congratulations!");
                            break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid card play. Try again.");
                    }
                }

                currentPlayerIndex = (currentPlayerIndex + 1) % numPlayers; // Rotate to the next player
            }

            return null;
        }


        public static void SubmitPlayerCard(string pid, UnoCard card, UnoCard.Color declaredColor)
        {
            Game.CheckPlayerTurn(pid);

            List<UnoCard> pHand = Game.GetPlayerHand(pid);

            if (!Game.ValidCardPlay(card))
            {
                if (card.CardColor != Game._validColor)
                {
                    string message =
                        $"Invalid player move, expected color: {Game._validColor} but got color {card.CardColor}";
                    throw new InvalidColorSubmissionException(message, card.CardColor, Game._validColor);
                }

                if (card.CardValue != Game._validValue)
                {
                    string message =
                        $"Invalid player move, expected value: {Game._validValue} but got value {card.CardValue}";
                    throw new InvalidValueSubmissionException(message, card.CardValue, Game._validValue);
                }
            }

            pHand.Remove(card);

            if (Game.HasEmptyHand(Game._playerIds[Game._currentPlayer]))
            {
                var message = $"{Game._playerIds[Game._currentPlayer]} won the game. Congratulations";
                Console.WriteLine(message);
                Environment.Exit(0);
            }

            Game._validColor = card.CardColor;
            Game._validValue = card.CardValue;
            Game._stockPile.Add(card);

            if (card.CardColor == UnoCard.Color.Wild)
            {
                Game._validColor = declaredColor;
            }

            if (card.CardValue == UnoCard.Value.DrawTwo)
            {
                string nextPlayer = Game.GetNextPlayer();
                var message = $"{Game._playerIds[Game._currentPlayer]} + 2 cards.";
                Console.WriteLine(message);
                Game.GetPlayerHand(nextPlayer).Add(Game._deck.DrawCard());
                Game.GetPlayerHand(nextPlayer).Add(Game._deck.DrawCard());
            }

            if (card.CardValue == UnoCard.Value.WildFour)
            {
                string nextPlayer = Game.GetNextPlayer();
                var message = $"{Game._playerIds[Game._currentPlayer]} + 4 cards.";
                Console.WriteLine(message);
                Game.GetPlayerHand(nextPlayer).Add(Game._deck.DrawCard());
                Game.GetPlayerHand(nextPlayer).Add(Game._deck.DrawCard());
                Game.GetPlayerHand(nextPlayer).Add(Game._deck.DrawCard());
                Game.GetPlayerHand(nextPlayer).Add(Game._deck.DrawCard());
            }

            if (card.CardValue == UnoCard.Value.Skip)
            {
                var message = $"{Game._playerIds[Game._currentPlayer]} was skipped.";
                Console.WriteLine(message);
                Game.GameDirectionCheck();
            }

            if (card.CardValue == UnoCard.Value.Reverse)
            {
                var message = $"{pid} changed game direction.";
                Console.WriteLine(message);
                Game._gameDirection = !Game._gameDirection;

                if (Game._gameDirection)
                {
                    Game._currentPlayer = (Game._currentPlayer - 2 + Game._playerIds.Length) % Game._playerIds.Length;
                }
                else
                {
                    Game._currentPlayer = (Game._currentPlayer + 2) % Game._playerIds.Length;
                }
            }

            Game.GameDirectionCheck();
        }

        static void GameDirectionCheck(string[] playerIds, bool initialDirection)
        {
            bool gameDirection = initialDirection;
            int currentPlayer = 0;
    
            // Implement the logic to change game direction and current player

            if (gameDirection)
            {
                currentPlayer = (currentPlayer - 1 + playerIds.Length) % playerIds.Length;
            }
            else
            {
                currentPlayer = (currentPlayer + 1) % playerIds.Length;
            }

            // Now the currentPlayer and game direction have been updated
        }
        
        static int PromptForNumberOfPlayers()
        {
            int numPlayers = 2; // Default to 2 players
            Console.WriteLine("Enter amount of players (2-7). 2 if enter pressed: ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 2 && choice <= 7)
            {
                numPlayers = choice;
            }

            return numPlayers;
        }

        
        static string? LoadGame()
        {
            //Console.WriteLine("Saved games");
            //Console.WriteLine(string.Join("\n", GameRepositoryFileSystem.GetSaveGames()));
            return null;
        }
    }
}

