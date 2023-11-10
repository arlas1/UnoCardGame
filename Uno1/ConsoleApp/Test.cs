/*
using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Domain;


namespace ConsoleApp;

public static class Test
{
    public static void Serialize()
    {
        var jsonOptions = new JsonSerializerOptions()
        {
            WriteIndented = true,
            AllowTrailingCommas = true,

        };


        var unoCard1 = new UnoCard(UnoCard.Color.Red, UnoCard.Value.One);
        var unoCard2 = new UnoCard(UnoCard.Color.Blue, UnoCard.Value.DrawTwo);
        var unoCard3 = new UnoCard(UnoCard.Color.Yellow, UnoCard.Value.Reverse);

        var playerId = 1;
        var name = "Artur";
        var playerType = Player.PlayerType.Human;
        var hand = new List<UnoCard>{unoCard1, unoCard2, unoCard3 };

        var w = new Player(playerId, name, playerType)
            {
                Id = playerId,
                Name = name,
                Type = playerType,
                Hand = hand
                
            };
    public static void StartTheGame(int numPlayers)
    {
        while (true)
        {
            
            DisplayGameHeader();

            var currentPlayerHand = GameState.PlayersList[GameState.CurrentPlayerIndex].Hand;

            FirstDisplay(currentPlayerHand);

            ConsoleKeyInfo key;

            do
            {
                key = Console.ReadKey();

                switch (key.Key)
                {
                    case ConsoleKey.UpArrow:
                        GameState.SelectedCardIndex = (GameState.SelectedCardIndex - 1 + currentPlayerHand.Count + 2) % (currentPlayerHand.Count + 2);
                        break;
                    case ConsoleKey.DownArrow:
                        GameState.SelectedCardIndex = (GameState.SelectedCardIndex + 1) % (currentPlayerHand.Count + 2);
                        break;
                }

                Console.Clear();
                DisplayGameHeader();

                Console.WriteLine($"Player {GameState.CurrentPlayerIndex + 1}'s hand:");
                for (var i = 0; i < currentPlayerHand.Count; i++)
                {
                    if (i == GameState.SelectedCardIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.Gray;
                        Console.ForegroundColor = ConsoleColor.Black;
                    }

                    Console.WriteLine($"{i + 1}. {currentPlayerHand[i]}");

                    Console.ResetColor();
                }

                if (GameState.SelectedCardIndex == currentPlayerHand.Count)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;
                }

                Console.WriteLine($"{currentPlayerHand.Count + 1}. -> draw a card <-");
                
                Console.ResetColor();
                
                Console.WriteLine("=======================");
                Console.WriteLine("Press ESCAPE to SAVE the game state and EXIT to the main menu.");

            } while (key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.RightArrow);
            
            if (key.Key == ConsoleKey.RightArrow)
            {
                JsonOptions.SaveIntoJson();
                Menu.Menu.RunMenu(NewOrLoadGame.NewGame, NewOrLoadGame.LoadGame);
            }

            var pId = GameState.CurrentPlayerIndex;

            var isValid = false;

            // Card placing or drawing
            if (GameState.SelectedCardIndex == currentPlayerHand.Count)
            {
                currentPlayerHand.Add(GameState.UnoDeck.DrawCard());
                isValid = true;
            }
            else
            {
                var selectedCard = currentPlayerHand[GameState.SelectedCardIndex];
                if (IsValidCardPlay(selectedCard))
                {
                    currentPlayerHand.RemoveAt(GameState.SelectedCardIndex);
                    GameState.StockPile.Add(selectedCard);

                    if (GameState.PlayersList[pId].Hand.Count == 0)
                    {
                        Console.WriteLine($"{GameState.PlayersList[GameState.CurrentPlayerIndex + 1].Name} wins! Congratulations!");
                        GameState.IsColorChosen = false;
                        break;
                    }

                    SubmitPlayerCard(selectedCard, pId, numPlayers);

                    if (selectedCard.CardColor == GameState.CardColorChoice)
                    {
                        GameState.IsColorChosen = false;
                    }
                    isValid = true;
                }
            }
            if (!isValid)
            {
                Console.Clear();
                continue;
            }

            // Player switch + exclusive control for skip card
            GetNextPlayerId(pId, numPlayers);
            
        }
    }
        
        Console.WriteLine(JsonSerializer.Serialize(w, jsonOptions));
    }
    
    
    public static string DeSerialize()
    {
        return "asd";
    }
}
*/