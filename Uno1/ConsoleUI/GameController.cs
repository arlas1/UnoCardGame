using DAL;
using Domain;

namespace ConsoleUI;

public static class GameController
{
    public static void StartTheGame(int numPlayers)
    {
        var exitGame = false;
        
        while (!exitGame)
        {
            var currentPlayerHand = GameState.PlayersList[GameState.CurrentPlayerIndex].Hand;
            var currentPlayer = GameState.PlayersList[GameState.CurrentPlayerIndex];
            var playerId = GameState.CurrentPlayerIndex;
            
            if (currentPlayer.Type == Player.PlayerType.Ai)
            {
                var validCards = currentPlayer.Hand
                    .Where(GameEngine.GameEngine.IsValidCardPlay)
                    .ToList();

                UnoCard selectedCard;

                if (validCards.Count > 0)
                {
                    // Choose a random valid card
                    var randomIndex = new Random().Next(0, validCards.Count);
                    selectedCard = validCards[randomIndex];
                }
                else
                {
                    // If no valid cards, draw a card
                    selectedCard = GameState.UnoDeck.DrawCard();
                    currentPlayer.Hand.Add(selectedCard);
                }

                currentPlayer.Hand.Remove(selectedCard);
                GameState.StockPile.Add(selectedCard);
                
                if (GameState.PlayersList[playerId].Hand.Count == 0)
                {
                    Console.WriteLine(
                        $"{GameState.PlayersList[GameState.CurrentPlayerIndex + 1].Name} wins! Congratulations!");
                    GameState.IsColorChosen = false;
                    break;
                }
                // var allPlayersAi = GameState.PlayersList.All(player => player.Type == Player.PlayerType.Ai);

                // if (currentPlayer.Type == Player.PlayerType.Ai)
                // {
                //     if (selectedCard is { CardColor: UnoCard.Color.Wild, CardValue: UnoCard.Value.Wild })
                //     {
                //         // Task 2: Randomly pick one of 4 colors for Wild card
                //         var randomColor = (UnoCard.Color)new Random().Next(0, 4);
                //         Console.WriteLine($"{currentPlayer.Name} placed Wild card. Chose color: {randomColor}");
                //         GameState.CardColorChoice = randomColor;
                //     }
                //     else
                //     {
                //         Console.WriteLine($"{currentPlayer.Name} placed {selectedCard}");
                //     }
                // }
                Console.WriteLine($"{currentPlayer.Name} placed {selectedCard}");
                GameEngine.GameEngine.SubmitPlayerCard(selectedCard, playerId, numPlayers);
                

                if (selectedCard.CardColor == GameState.CardColorChoice)
                {
                    GameState.IsColorChosen = false;
                }
                
                // Player switch + exclusive control for skip card
                GameEngine.GameEngine.GetNextPlayerId(playerId, numPlayers);
                
            }
            else
            {
                ConsoleVisualization.DisplayGameHeader();
                
                ConsoleVisualization.DisplayPlayerHand(currentPlayerHand);

                ConsoleKeyInfo key;

                do
                {

                    while (Console.KeyAvailable)
                    {
                        Console.ReadKey(true);
                    }

                    key = Console.ReadKey();

                    switch (key.Key)
                    {
                        case ConsoleKey.UpArrow:
                            if (GameState.SelectedCardIndex == 0)
                            {
                                // If at the first card, move to the last card
                                GameState.SelectedCardIndex = currentPlayerHand.Count;
                            }
                            else
                            {
                                // Move up normally
                                GameState.SelectedCardIndex =
                                    (GameState.SelectedCardIndex - 1) % (currentPlayerHand.Count + 1);
                            }

                            break;
                        case ConsoleKey.DownArrow:
                            GameState.SelectedCardIndex =
                                (GameState.SelectedCardIndex + 1) % (currentPlayerHand.Count + 1);
                            break;
                    }

                    Console.Clear();
                    ConsoleVisualization.DisplayGameHeader();

                    Console.WriteLine($"{GameState.PlayersList[GameState.CurrentPlayerIndex].Name}'s hand:");
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
                    Console.WriteLine("Press RIGHT ARROW to SAVE and EXIT to the main menu.");
                    Console.WriteLine("                        OR");
                    Console.WriteLine("Press LEFT ARROW to EXIT without saving game state.");


                } while (key.Key != ConsoleKey.Enter && key.Key != ConsoleKey.RightArrow &&
                         key.Key != ConsoleKey.LeftArrow);

                if (key.Key == ConsoleKey.RightArrow)
                {
                    if (GameState.RepositoryChoice == 1)
                    {
                        JsonRepository.SaveIntoJson();
                        Menu.Menu.RunMenu(GameSetup.NewGame, GameSetup.LoadGameJson);
                    }
                    else
                    {
                        DbRepository.SaveIntoDb();
                        Menu.Menu.RunMenu(GameSetup.NewGame, GameSetup.LoadGameDb);
                    }


                    exitGame = true; // Exit the game loop
                }

                if (key.Key == ConsoleKey.LeftArrow)
                {

                    Menu.Menu.RunMenu(GameSetup.NewGame, GameSetup.LoadGameDb);

                    exitGame = true; // Exit the game loop
                }

                else
                {
                    var isValid = false;
                    
                    if (GameState.SelectedCardIndex == currentPlayerHand.Count)
                    {
                        currentPlayerHand.Add(GameState.UnoDeck.DrawCard());
                        isValid = true;
                    }
                    else
                    {
                        var selectedCard = currentPlayerHand[GameState.SelectedCardIndex];
                        if (GameEngine.GameEngine.IsValidCardPlay(selectedCard))
                        {
                            currentPlayerHand.RemoveAt(GameState.SelectedCardIndex);
                            GameState.StockPile.Add(selectedCard);

                            if (GameState.PlayersList[playerId].Hand.Count == 0)
                            {
                                Console.WriteLine(
                                    $"{GameState.PlayersList[GameState.CurrentPlayerIndex + 1].Name} wins! Congratulations!");
                                GameState.IsColorChosen = false;
                                break;
                            }

                            GameEngine.GameEngine.SubmitPlayerCard(selectedCard, playerId, numPlayers);

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
                    GameEngine.GameEngine.GetNextPlayerId(playerId, numPlayers);
                }
            }
        }
    }
    
}