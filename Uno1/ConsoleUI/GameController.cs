using DAL;
using Domain;
using UnoGameEngine;

namespace ConsoleUI;

public class GameController
{
    private readonly GameEngine _gameEngine;
    
    private Player _currentPlayer = null!;
    private int _currentPlayerId;
    private List<UnoCard> _currentPlayerHand = null!;

    public GameController(GameEngine gameEngine)
    {
        _gameEngine = gameEngine;
    }
    
    public void Run() 
    {
        var gameEnded = false;
        
        while (!gameEnded)
        {
            UpdateCurrentPlayerData();
            
            if (_currentPlayer.Type == Player.PlayerType.Ai)
            {
                var validCards = _currentPlayer.Hand
                    .Where(_gameEngine.IsValidCardPlay)
                    .ToList();

                if (validCards.Count > 0)
                {
                    var selectedCard = AiChoseRandomCard(validCards);
                    
                    var aiPlayerWon = CheckDidThePlayerWin();
                    if (aiPlayerWon)
                    {
                        break;
                    }

                    Console.WriteLine($"{_currentPlayer.Name} placed {selectedCard}");
                    SubmitAiPlayerMove(selectedCard);
                }
                else
                {
                    PlayerTakeCardFromDeck();
                }
                
                // Next player turn
                _gameEngine.GetNextPlayerId(_currentPlayerId);
            }
            
            else if (_currentPlayer.Type == Player.PlayerType.Human)
            {
                ConsoleVisualization.DisplayGameHeader(_gameEngine);
                ConsoleVisualization.DisplayPlayerHand(_currentPlayerHand, _gameEngine);
                ConsoleKeyInfo key;

                do
                {
                    while (Console.KeyAvailable)
                    {
                        Console.ReadKey(true);
                    }

                    key = Console.ReadKey();

                    // Move between the cards in hand
                    switch (key.Key)
                    {
                        case ConsoleKey.UpArrow:
                            if (_gameEngine.GameState.SelectedCardIndex == 0)
                            {
                                _gameEngine.GameState.SelectedCardIndex = _currentPlayerHand.Count;
                            }
                            else
                            {
                                _gameEngine.GameState.SelectedCardIndex =
                                    (_gameEngine.GameState.SelectedCardIndex - 1) % (_currentPlayerHand.Count + 1);
                            }

                            break;
                        case ConsoleKey.DownArrow:
                            _gameEngine.GameState.SelectedCardIndex =
                                (_gameEngine.GameState.SelectedCardIndex + 1) % (_currentPlayerHand.Count + 1);
                            break;
                    }

                    Console.Clear();
                    ConsoleVisualization.DisplayGameHeader(_gameEngine);
                    ConsoleVisualization.DisplayPlayerHand(_currentPlayerHand, _gameEngine);
                    
                } while (key.Key != ConsoleKey.Enter &&
                         key.Key != ConsoleKey.RightArrow &&
                         key.Key != ConsoleKey.LeftArrow);
                
                if (key.Key == ConsoleKey.RightArrow)
                {
                    SaveAndExit();
                    gameEnded = true;
                }
                
                if (key.Key == ConsoleKey.LeftArrow)
                {
                    ExitWithoutSaving();
                    gameEnded = true;
                }
                
                // Player have chosen (to take) a card
                else
                {
                    var isValidMove = false;
                    var takeACardOption = _currentPlayerHand.Count;
                    
                    if (_gameEngine.GameState.SelectedCardIndex == takeACardOption)
                    {
                        PlayerTakeCardFromDeck();
                        isValidMove = true;
                    }
                    else
                    {
                        var selectedCard = _currentPlayerHand[_gameEngine.GameState.SelectedCardIndex];
                        if (_gameEngine.IsValidCardPlay(selectedCard))
                        {
                            PlaceCardOnStockPile(selectedCard);

                            var humanPlayerWon = CheckDidThePlayerWin();
                            if (humanPlayerWon)
                            {
                                break;
                            }

                            SubmitHumanPlayerMove(selectedCard);
                            isValidMove = true;
                        }
                    }
                    
                    if (!isValidMove)
                    {
                        Console.Clear();
                        continue;
                    }

                    // Next player turn
                    _gameEngine.GetNextPlayerId(_currentPlayerId);
                }
            }
        }
    }
    
    private void UpdateCurrentPlayerData()
    {
        _currentPlayerHand = _gameEngine.GameState.PlayersList[_gameEngine.GameState.CurrentPlayerIndex].Hand;
        _currentPlayer = _gameEngine.GameState.PlayersList[_gameEngine.GameState.CurrentPlayerIndex];
        _currentPlayerId = _gameEngine.GameState.CurrentPlayerIndex;
    }

    private UnoCard AiChoseRandomCard(IReadOnlyList<UnoCard> validCards)
    {
        var randomIndex = new Random().Next(0, validCards.Count);
        var selectedCard = validCards[randomIndex];
                    
        PlaceCardOnStockPile(selectedCard);

        return selectedCard;
    }

    private void PlaceCardOnStockPile(UnoCard selectedCard)
    {
        _currentPlayer.Hand.Remove(selectedCard);
        _gameEngine.GameState.StockPile.Add(selectedCard);
    }

    private void PlayerTakeCardFromDeck()
    {
        _currentPlayerHand.Add(_gameEngine.GameState.UnoDeck.DrawCard());
    }

    private void SubmitAiPlayerMove(UnoCard selectedCard)
    {
        if (selectedCard.CardValue != UnoCard.Value.Wild)
        {
            _gameEngine.SubmitPlayerCard(_currentPlayerId, selectedCard);
        }
        else
        {
            GameConfiguration.PromptForWildCardColorAi(_gameEngine);
        }

        if (selectedCard.CardColor == _gameEngine.GameState.CardColorChoice)
        {
            _gameEngine.GameState.IsColorChosen = false;
        }
    }
    
    private void SubmitHumanPlayerMove(UnoCard selectedCard)
    {
        if (selectedCard.CardValue != UnoCard.Value.Wild)
        {
            _gameEngine.SubmitPlayerCard(_currentPlayerId, selectedCard);
        }
        else
        {
            GameConfiguration.PromptForWildCardColorHuman(_gameEngine);
        }
        
        if (selectedCard.CardColor == _gameEngine.GameState.CardColorChoice)
        {
            _gameEngine.GameState.IsColorChosen = false;
        }
    }
    
    private bool CheckDidThePlayerWin()
    {
        if (_gameEngine.GameState.PlayersList[_currentPlayerId].Hand.Count == 0)
        {
            Console.WriteLine(
                $"{_gameEngine.GameState.PlayersList[_gameEngine.GameState.CurrentPlayerIndex].Name} wins! Congratulations!");
            _gameEngine.GameState.IsColorChosen = false;
            return true;
        }

        return false;
    }

    private void SaveAndExit()
    {
        var gameEngine = new GameEngine();

        GameConfiguration.PromptForRepositoryType(gameEngine);

        switch (gameEngine.GameState.RepositoryChoice)
        {
            case 1:
                JsonRepository.SaveIntoJson(_gameEngine);
                Menu.Menu.Run(() => GameSetup.NewGame(gameEngine), () => GameSetup.LoadGameJson(gameEngine));
                break;
            case 2:
                DbRepository.SaveIntoDb(_gameEngine);
                Menu.Menu.Run(() => GameSetup.NewGame(gameEngine), () => GameSetup.LoadGameDb(gameEngine));
                break;
        }

    }
    
    private static void ExitWithoutSaving()
    {
        GameConfiguration.Start();
    }
    
}