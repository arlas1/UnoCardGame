using DAL;
using Domain;
using UnoGameEngine;

namespace ConsoleUI;

public class GameController
{
    
    private GameEngine _gameEngine;

    public GameController(GameEngine gameEngine)
    {
        _gameEngine = gameEngine;
    }
    public void Run(int numPlayers)
    {
        var exitGame = false;
        
        while (!exitGame)
        {
            var currentPlayerHand = _gameEngine.GameState.PlayersList[_gameEngine.GameState.CurrentPlayerIndex].Hand;
            var currentPlayer = _gameEngine.GameState.PlayersList[_gameEngine.GameState.CurrentPlayerIndex];
            var playerId = _gameEngine.GameState.CurrentPlayerIndex;
            
            if (currentPlayer.Type == Player.PlayerType.Ai)
            {
                var validCards = currentPlayer.Hand
                    .Where(_gameEngine.IsValidCardPlay)
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
                    selectedCard = _gameEngine.GameState.UnoDeck.DrawCard();
                    currentPlayer.Hand.Add(selectedCard);
                }

                currentPlayer.Hand.Remove(selectedCard);
                _gameEngine.GameState.StockPile.Add(selectedCard);
                
                if (_gameEngine.GameState.PlayersList[playerId].Hand.Count == 0)
                {
                    Console.WriteLine(
                        $"{_gameEngine.GameState.PlayersList[_gameEngine.GameState.CurrentPlayerIndex + 1].Name} wins! Congratulations!");
                    _gameEngine.GameState.IsColorChosen = false;
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

                if (selectedCard.CardValue != UnoCard.Value.Wild)
                {
                    _gameEngine.SubmitPlayerCard(selectedCard, playerId, numPlayers);
                }
                else
                {
                    GameConfiguration.PromptForWildCardColorAi(_gameEngine);
                }
                

                if (selectedCard.CardColor == _gameEngine.GameState.CardColorChoice)
                {
                    _gameEngine.GameState.IsColorChosen = false;
                }
                
                // Player switch + exclusive control for skip card
                _gameEngine.GetNextPlayerId(playerId, numPlayers);
                
            }
            else
            {
                ConsoleVisualization.DisplayGameHeader(_gameEngine);
                
                ConsoleVisualization.DisplayPlayerHand(currentPlayerHand, _gameEngine);

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
                            if (_gameEngine.GameState.SelectedCardIndex == 0)
                            {
                                // If at the first card, move to the last card
                                _gameEngine.GameState.SelectedCardIndex = currentPlayerHand.Count;
                            }
                            else
                            {
                                // Move up normally
                                _gameEngine.GameState.SelectedCardIndex =
                                    (_gameEngine.GameState.SelectedCardIndex - 1) % (currentPlayerHand.Count + 1);
                            }

                            break;
                        case ConsoleKey.DownArrow:
                            _gameEngine.GameState.SelectedCardIndex =
                                (_gameEngine.GameState.SelectedCardIndex + 1) % (currentPlayerHand.Count + 1);
                            break;
                    }

                    Console.Clear();
                    ConsoleVisualization.DisplayGameHeader(_gameEngine);

                    Console.WriteLine($"{_gameEngine.GameState.PlayersList[_gameEngine.GameState.CurrentPlayerIndex].Name}'s hand:");
                    for (var i = 0; i < currentPlayerHand.Count; i++)
                    {
                        if (i == _gameEngine.GameState.SelectedCardIndex)
                        {
                            Console.BackgroundColor = ConsoleColor.Gray;
                            Console.ForegroundColor = ConsoleColor.Black;
                        }

                        Console.WriteLine($"{i + 1}. {currentPlayerHand[i]}");

                        Console.ResetColor();
                    }

                    if (_gameEngine.GameState.SelectedCardIndex == currentPlayerHand.Count)
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
                    var gameEngine = new GameEngine();

                    GameConfiguration.PromptForRepositoryType(gameEngine);
                    
                    switch (gameEngine.GameState.RepositoryChoice)
                    {
                        case 1:
                            JsonRepository.SaveIntoJson(_gameEngine);
                            Menu.Menu.RunMenu(() => GameSetup.NewGame(gameEngine), () => GameSetup.LoadGameJson(gameEngine));
                            break;
                        case 2:
                            DbRepository.SaveIntoDb(_gameEngine);
                            Menu.Menu.RunMenu(() => GameSetup.NewGame(gameEngine), () => GameSetup.LoadGameDb(gameEngine));
                            break;
                    }
                    
                    exitGame = true; // Exit the game loop
                }

                if (key.Key == ConsoleKey.LeftArrow)
                {
                    var gameEngine = new GameEngine();

                    GameConfiguration.PromptForRepositoryType(gameEngine);

                    switch (gameEngine.GameState.RepositoryChoice)
                    {
                        case 1:
                            Menu.Menu.RunMenu(() => GameSetup.NewGame(gameEngine), () => GameSetup.LoadGameJson(gameEngine));
                            break;
                        case 2:
                            Menu.Menu.RunMenu(() => GameSetup.NewGame(gameEngine), () => GameSetup.LoadGameDb(gameEngine));
                            break;
                    }

                    exitGame = true; // Exit the game loop
                }

                else
                {
                    var isValid = false;
                    
                    if (_gameEngine.GameState.SelectedCardIndex == currentPlayerHand.Count)
                    {
                        currentPlayerHand.Add(_gameEngine.GameState.UnoDeck.DrawCard());
                        isValid = true;
                    }
                    else
                    {
                        var selectedCard = currentPlayerHand[_gameEngine.GameState.SelectedCardIndex];
                        if (_gameEngine.IsValidCardPlay(selectedCard))
                        {
                            currentPlayerHand.RemoveAt(_gameEngine.GameState.SelectedCardIndex);
                            _gameEngine.GameState.StockPile.Add(selectedCard);

                            if (_gameEngine.GameState.PlayersList[playerId].Hand.Count == 0)
                            {
                                Console.WriteLine(
                                    $"{_gameEngine.GameState.PlayersList[_gameEngine.GameState.CurrentPlayerIndex + 1].Name} wins! Congratulations!");
                                _gameEngine.GameState.IsColorChosen = false;
                                break;
                            }

                            if (selectedCard.CardValue != UnoCard.Value.Wild)
                            {
                                _gameEngine.SubmitPlayerCard(selectedCard, playerId, numPlayers);
                            }
                            else
                            {
                                GameConfiguration.PromptForWildCardColorHuman(_gameEngine);
                            }

                            if (selectedCard.CardColor == _gameEngine.GameState.CardColorChoice)
                            {
                                _gameEngine.GameState.IsColorChosen = false;
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
                    _gameEngine.GetNextPlayerId(playerId, numPlayers);
                }
            }
        }
    }
    
}