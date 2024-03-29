﻿using DAL;
using DAL.DbEntities;
using Domain;
using UnoGameEngine;
using Microsoft.EntityFrameworkCore;
using Player = Domain.Player;
using UnoDeck = Domain.UnoDeck;

namespace WebApp.GamesManager;

public class GameManager(AppDbContext context)
{
    public (int gameId, int playerId) CreateTheGame(string nickname, int playersMaxAmount, int cardsMaxInHand,
                                                    UnoCard.Value? cardValueToAvoid)
    {
        var gameEngine = new GameEngine
        {
            GameState =
            {
                MaxCardsAmount = cardsMaxInHand,
                MaxPlayersAmount = playersMaxAmount,
                IsGameStarted = 0,
                IsGameEnded = 0
            }
        };

        gameEngine.DeleteCardsWithValueToAvoid(cardValueToAvoid);

        var player = new Player(0, nickname, Player.PlayerType.Human)
        {
            Hand = gameEngine.GetOneHand()
        };
        gameEngine.GameState.PlayersList.Add(player);

// ------------------------------------------------------------------------------- //

        context.Database.Migrate();

        var gameStateEntity = new DAL.DbEntities.GameState
        {
            GameDirection = 0,
            CurrentPlayerIndex = 0,
            IsColorChosen = 0,
            SelectedCardIndex = -1,
            CardColorChoice = 4, // 4 is default!, 0,1,2,3 - colors
            MaxCardAmount = gameEngine.GameState.MaxCardsAmount,
            PlayersMaxAmount = gameEngine.GameState.MaxPlayersAmount,
            IsGameStarted = 0,
            IsGameEnded = 0
        };

        context.GameStates.Add(gameStateEntity);
        context.SaveChanges();
        
        foreach (var card in gameEngine.GameState.UnoDeck.SerializedCards)
        {
            var unoDeckEntity = new DAL.DbEntities.UnoDeck
            {
                CardColor = (int)card.CardColor,
                CardValue = (int)card.CardValue,
                GameStateId = gameStateEntity.Id
            };

            context.UnoDecks.Add(unoDeckEntity);
        }

        context.SaveChanges();

        var playerEntity = new DAL.DbEntities.Player
        {
            Name = player.Name,
            Type = (int)player.Type,
            Role = 1,
            GameStateId = gameStateEntity.Id
        };

        context.Players.Add(playerEntity);
        context.SaveChanges();

        foreach (var card in player.Hand)
        {
            var handEntity = new Hand
            {
                CardColor = (int)card.CardColor,
                CardValue = (int)card.CardValue,
                PlayerId = playerEntity.Id,
                GameStateId = gameStateEntity.Id
            };

            context.Hands.Add(handEntity);
        }

        context.SaveChanges();

        return (gameStateEntity.Id, playerEntity.Id);
    }

    public void DeleteTheGame(int gameId)
    {
        var gameState = context.GameStates.SingleOrDefault(gs => gs.Id == gameId);

        if (gameState != null)
        {
            var hands = context.Hands.Where(hand => hand.GameStateId == gameId);
            var stockPiles = context.StockPiles.Where(stockPile => stockPile.GameStateId == gameId);
            var players = context.Players.Where(player => player.GameStateId == gameId);
            var unoDecks = context.UnoDecks.Where(deck => deck.GameStateId == gameId);

            context.Hands.RemoveRange(hands);
            context.StockPiles.RemoveRange(stockPiles);
            context.Players.RemoveRange(players);
            context.UnoDecks.RemoveRange(unoDecks);

            context.GameStates.Remove(gameState);

            context.SaveChanges();
        }

    }

    public async Task<(int playerId, int maxAmount)> JoinTheGame(int gameId, string nickname, Player.PlayerType playerType)
    {
        var gameState = context.GameStates.SingleOrDefault(gs => gs.Id == gameId);

        var unoDeck = context.UnoDecks.Where(sp => sp.GameStateId == gameId);
        var takenCards = unoDeck.Take(gameState!.MaxCardAmount).ToList();

        var players = await context.Players
            .Where(player => player.GameStateId == gameId)
            .ToListAsync();

        // If nickname is empty or null, generate a default nickname
        if (string.IsNullOrWhiteSpace(nickname))
        {
            // Assuming player index is 1-based
            nickname = $"Player {players.Count + 1}";
        }

        var player = new DAL.DbEntities.Player
        {
            Name = nickname,
            Type = (int)playerType,
            Role = 0,
            GameStateId = gameState.Id
        };

        context.Players.Add(player);
        await context.SaveChangesAsync();

        foreach (var card in takenCards)
        {
            var handEntity = new Hand
            {
                CardColor = card.CardColor,
                CardValue = card.CardValue,
                PlayerId = player.Id,
                GameStateId = gameState.Id
            };
            context.Hands.Add(handEntity);
        }

        context.UnoDecks.RemoveRange(takenCards);
        await context.SaveChangesAsync();

        return (player.Id, gameState.PlayersMaxAmount);
    }

    public void LeaveTheGame(int gameId, int playerIndex)
    {
        var gameState = context.GameStates.SingleOrDefault(gs => gs.Id == gameId);

        if (gameState != null)
        {
            var playerToRemove = context.Players.SingleOrDefault(player => player.Id == playerIndex);

            if (playerToRemove != null)
            {
                context.Hands.RemoveRange(context.Hands.Where(hand =>
                    hand.GameStateId == gameId && hand.PlayerId == playerToRemove.Id));
                context.Players.Remove(playerToRemove);

                context.SaveChanges();
            }
        }
    }

    public void StartTheGame(int gameId)
    {
        var gameState = context.GameStates.SingleOrDefault(gs => gs.Id == gameId);
        gameState!.IsGameStarted = 1;
        context.SaveChanges();

        var unoDeck = context.UnoDecks.Where(sp => sp.GameStateId == gameId);
        var gameEngine = new GameEngine
        {
            GameState =
            {
                UnoDeck = new UnoDeck(),
                StockPile = new List<UnoCard>()
            }
        };

        foreach (var card in unoDeck)
        {
            gameEngine.GameState.UnoDeck.AddCardToDeck(new UnoCard((UnoCard.Color)card.CardColor,
                (UnoCard.Value)card.CardValue));
        }

        var cardToDelete = gameEngine.CheckFirstCardInGame();

        var stockPile = context.StockPiles;
        var stockPileEntity = new StockPile
        {
            CardColor = (int)gameEngine.GameState.StockPile.Last().CardColor,
            CardValue = (int)gameEngine.GameState.StockPile.Last().CardValue,
            GameStateId = gameId
        };

        stockPile.Add(stockPileEntity);

        var cardToRemove = context.UnoDecks
            .FirstOrDefault(card => card.GameStateId == gameId
                                    && card.CardColor == (int)cardToDelete.CardColor
                                    && card.CardValue == (int)cardToDelete.CardValue);

        context.UnoDecks.Remove(cardToRemove!);
        context.SaveChanges();

    }

    public bool CheckGameStart(int gameId)
    {
        var gameState = context.GameStates.SingleOrDefault(gs => gs.Id == gameId);

        return gameState!.IsGameStarted == 1;
    }

    public async Task DrawCard(int gameId, int playerId)
    {
        var gameState = context.GameStates.SingleOrDefault(gs => gs.Id == gameId);
        var players = context.Players.Where(player => player.GameStateId == gameId)
            .OrderBy(player => player.Id)
            .ToList();

        var currentPlayerId = players.IndexOf(players.FirstOrDefault(player => player.Id == playerId)!);

        var gameEngine = new GameEngine
        {
            GameState =
            {
                // ReSharper disable once SimplifyConditionalTernaryExpression
                GameDirection = gameState!.GameDirection == 0 ? false : true,
                CurrentPlayerIndex = gameState.CurrentPlayerIndex,
                PlayersList = [],
                StockPile = []
            }
        };
        
        var unoDeck = context.UnoDecks.Where(card => card.GameStateId == gameId);
        if (!unoDeck.Any())
        {
            foreach (var card in gameEngine.GameState.UnoDeck.SerializedCards)
            {
                var unoDeckEntity = new DAL.DbEntities.UnoDeck
                {
                    CardColor = (int)card.CardColor,
                    CardValue = (int)card.CardValue,
                    GameStateId = gameId
                };

                context.UnoDecks.Add(unoDeckEntity);
            }

            await context.SaveChangesAsync();
        }

        foreach (var unused in players)
        {
            gameEngine.GameState.PlayersList.Add(new Player(0, "a", Player.PlayerType.Human));
        }

        var lastStockPileCard = context.StockPiles
            .Where(stockPile => stockPile.GameStateId == gameId)
            .OrderBy(stockPile => stockPile.Id)
            .Last();

        var color = (UnoCard.Color)lastStockPileCard.CardColor;
        var value = (UnoCard.Value)lastStockPileCard.CardValue;

        gameEngine.GameState.StockPile.Add(new UnoCard(color, value));

        gameEngine.GetNextPlayerId(currentPlayerId);

        // ---------------------------------------------------------------------------------- //
        var lastDeckCard = context.UnoDecks
            .Where(deck => deck.GameStateId == gameId)
            .OrderBy(deck => deck.Id) // Add an OrderBy clause here
            .Last();

        var handEntity = new Hand
        {
            CardColor = lastDeckCard.CardColor,
            CardValue = lastDeckCard.CardValue,
            PlayerId = playerId,
            GameStateId = gameId
        };

        context.Hands.Add(handEntity);
        gameState.CurrentPlayerIndex = gameEngine.GameState.CurrentPlayerIndex;
        context.UnoDecks.RemoveRange(context.UnoDecks.Where(card => card.Id == lastDeckCard.Id));
        await context.SaveChangesAsync();

        var currentPlayer = players[gameState.CurrentPlayerIndex];
        if (currentPlayer.Type == (int)Player.PlayerType.Ai)
        {
            await PlayCardAi(gameId, currentPlayer.Id);
        }

    }

    public async Task<bool> ValidateCard(int gameId, UnoCard card, UnoCard.Color? cardColor)
    {
        var gameState = context.GameStates.SingleOrDefault(gs => gs.Id == gameId);

        var gameEngine = new GameEngine
        {
            GameState =
            {
                // ReSharper disable once SimplifyConditionalTernaryExpression
                GameDirection = gameState!.GameDirection == 0 ? false : true,
                CurrentPlayerIndex = gameState.CurrentPlayerIndex,
                PlayersList = [],
                StockPile = [],
                CardColorChoice = cardColor ?? (UnoCard.Color)gameState.CardColorChoice
            }
        };

        var lastStockPileCard = context.StockPiles
            .Where(stockPile => stockPile.GameStateId == gameId)
            .OrderBy(stockPile => stockPile.Id)
            .Last();

        var color = (UnoCard.Color)lastStockPileCard.CardColor;
        var value = (UnoCard.Value)lastStockPileCard.CardValue;

        gameEngine.GameState.StockPile.Add(new UnoCard(color, value));

        var result = gameEngine.IsValidCardPlay(card);

        if (result)
        {
            if (cardColor != null)
            {
                gameState.IsColorChosen = 1;
                gameState.CardColorChoice = (int)cardColor;
                await context.SaveChangesAsync();
            }
            else
            {
                gameState.IsColorChosen = 0;
                gameState.CardColorChoice = 4;
                await context.SaveChangesAsync();
            }
        }

        if (gameState.IsGameEnded == 1)
        {
            result = false;
        }

        return result;
    }
    
    public async Task<(bool hasWinner, int playerId)> CheckForWinner(int gameId, int playerId)
    {
        var gameState = context.GameStates.SingleOrDefault(gs => gs.Id == gameId);

        if (gameState!.IsGameEnded == 1)
        {
            return (true, gameState.WinnerId);
        }

        var playerHandCount = await context.Hands.CountAsync(hand =>
            hand.GameStateId == gameId && hand.PlayerId == playerId);

        if (playerHandCount == 0)
        {
            gameState.IsGameEnded = 1;
            gameState.WinnerId = playerId;
            await context.SaveChangesAsync();
            return (true, playerId);
        }

        return (false, playerId);
    }

    public async Task PlayCardHuman(int gameId, int playerId, UnoCard card)
    {
        // Retrieve the current game state
        var gameState = context.GameStates.SingleOrDefault(gs => gs.Id == gameId);

        // Create a game engine and initialize it with the current game state
        var gameEngine = new GameEngine
        {
            GameState =
            {
                // ReSharper disable once SimplifyConditionalTernaryExpression
                GameDirection = gameState!.GameDirection == 0 ? false : true,
                CurrentPlayerIndex = gameState.CurrentPlayerIndex,
                PlayersList = [],
                UnoDeck = new UnoDeck(),
                StockPile = [],

            }
        };

        // Retrieve necessary data from the database
        var unoDeck = await context.UnoDecks
            .Where(unoCard => unoCard.GameStateId == gameId)
            .ToListAsync();

        var players = await context.Players
            .Where(player => player.GameStateId == gameId)
            .ToListAsync();

        // Update the game engine with the retrieved data
        gameEngine.GameState.UnoDeck.Clear();
        if (unoDeck.Count == 0)
        {
            var gameEngineForAdditionalUnoDeck = new GameEngine();
            foreach (var unoCard in gameEngineForAdditionalUnoDeck.GameState.UnoDeck.SerializedCards)
            {
                var unoDeckEntity = new DAL.DbEntities.UnoDeck
                {
                    CardColor = (int)unoCard.CardColor,
                    CardValue = (int)unoCard.CardValue,
                    GameStateId = gameId
                };

                context.UnoDecks.Add(unoDeckEntity);
            }

            await context.SaveChangesAsync();
        }

        foreach (var unoCardEntity in unoDeck)
        {
            gameEngine.GameState.UnoDeck.AddCardToDeck(new UnoCard(
                (UnoCard.Color)unoCardEntity.CardColor,
                (UnoCard.Value)unoCardEntity.CardValue
            ));
        }

        var plId = 0;
        foreach (var playerEntity in players)
        {
            gameEngine.GameState.PlayersList.Add(new Player(plId, playerEntity.Name, Player.PlayerType.Human));
            plId++;
        }

        // Determine the current player index
        var currentPlayerIndex = players.FindIndex(player => player.Id == playerId);

        // Perform the necessary game logic
        context.StockPiles.Add(new StockPile
        {
            CardColor = (int)card.CardColor,
            CardValue = (int)card.CardValue,
            GameStateId = gameId
        });

        var cardToRemove = context.Hands.FirstOrDefault(hand =>
            hand.GameStateId == gameId &&
            hand.PlayerId == playerId &&
            hand.CardColor == (int)card.CardColor &&
            hand.CardValue == (int)card.CardValue);

        context.Hands.Remove(cardToRemove!);
        await context.SaveChangesAsync();

        gameEngine.SubmitPlayerCard(currentPlayerIndex, card);
        gameEngine.GameState.StockPile.Add(card);

        gameEngine.GetNextPlayerId(currentPlayerIndex);

        // Update the game state with the modified game engine
        gameState.CurrentPlayerIndex = gameEngine.GameState.CurrentPlayerIndex;
        gameState.GameDirection = gameEngine.GameState.GameDirection ? 1 : 0;

        // Update the current player's hand in the database
        var currentPlayer = players[gameState.CurrentPlayerIndex];
        foreach (var handCard in gameEngine.GameState.PlayersList[gameState.CurrentPlayerIndex].Hand)
        {
            context.Hands.Add(new Hand
            {
                CardColor = (int)handCard.CardColor,
                CardValue = (int)handCard.CardValue,
                PlayerId = currentPlayer.Id,
                GameStateId = gameId
            });
            var unoDeckCardToRemove = context.UnoDecks
                .Where(deckCard =>
                    deckCard.GameStateId == gameId &&
                    deckCard.CardColor == (int)handCard.CardColor &&
                    deckCard.CardValue == (int)handCard.CardValue)
                .OrderByDescending(deckCard => deckCard.Id)
                .FirstOrDefault();
            context.UnoDecks.Remove(unoDeckCardToRemove!);
        }

        await context.SaveChangesAsync();

        if (currentPlayer.Type == (int)Player.PlayerType.Ai)
        {
            await PlayCardAi(gameId, currentPlayer.Id);
        }
    }

    private async Task PlayCardAi(int gameId, int playerId)
    {
        var gameState = context.GameStates.SingleOrDefault(gs => gs.Id == gameId);

        if (gameState!.IsGameEnded == 1)
        {
            return;
        }

        var players = await context.Players
            .Where(player => player.GameStateId == gameId)
            .ToListAsync();


        var gameEngine = new GameEngine
        {
            GameState =
            {
                // ReSharper disable once SimplifyConditionalTernaryExpression
                GameDirection = gameState.GameDirection == 0 ? false : true,
                CurrentPlayerIndex = gameState.CurrentPlayerIndex,
                PlayersList = [],
                StockPile = [],
                CardColorChoice = gameState.CardColorChoice == 4 ? default : (UnoCard.Color)gameState.CardColorChoice
            }
        };
        var lastStockPileCard = context.StockPiles
            .Where(stockPile => stockPile.GameStateId == gameId)
            .OrderBy(stockPile => stockPile.Id)
            .Last();
        var color = (UnoCard.Color)lastStockPileCard.CardColor;
        var value = (UnoCard.Value)lastStockPileCard.CardValue;

        gameEngine.GameState.StockPile.Add(new UnoCard(color, value));

        // Retrieve necessary data from the database
        var unoDeck = await context.UnoDecks
            .Where(unoCard => unoCard.GameStateId == gameId)
            .ToListAsync();

        // Update the game engine with the retrieved data
        gameEngine.GameState.UnoDeck.Clear();
        foreach (var unoCardEntity in unoDeck)
        {
            gameEngine.GameState.UnoDeck.AddCardToDeck(new UnoCard(
                (UnoCard.Color)unoCardEntity.CardColor,
                (UnoCard.Value)unoCardEntity.CardValue
            ));
        }

        var currentPlayer = new Player(1, "a", Player.PlayerType.Human);
        var plId = 0;
        foreach (var playerEntity in players)
        {
            gameEngine.GameState.PlayersList.Add(new Player(plId, playerEntity.Name,
                (Player.PlayerType)playerEntity.Type));
            plId++;
            if (playerEntity.Id == playerId)
            {
                currentPlayer.Id = playerEntity.Id;
                currentPlayer.Name = playerEntity.Name;
                currentPlayer.Type = (Player.PlayerType)playerEntity.Type;
            }
        }

        foreach (var card in await context.Hands.Where(card => card.PlayerId == playerId).ToListAsync())
        {
            var cardToAdd = new UnoCard((UnoCard.Color)card.CardColor, (UnoCard.Value)card.CardValue);
            currentPlayer.Hand.Add(cardToAdd);
        }

        var validCards = currentPlayer.Hand
            .Where(card => gameEngine.IsValidCardPlay(card))
            .ToList();

        UnoCard selectedCard;
//------------------------------------------------------------------------------------------------------------------//
        if (validCards.Count > 0)
        {
            // Choose a random valid card
            var randomIndex = new Random().Next(0, validCards.Count);
            selectedCard = validCards[randomIndex];

            // Update database with AI move
            context.StockPiles.Add(new StockPile
            {
                CardColor = (int)selectedCard.CardColor,
                CardValue = (int)selectedCard.CardValue,
                GameStateId = gameId
            });

            var cardToRemove = context.Hands.FirstOrDefault(hand =>
                hand.GameStateId == gameId &&
                hand.PlayerId == playerId &&
                hand.CardColor == (int)selectedCard.CardColor &&
                hand.CardValue == (int)selectedCard.CardValue);

            context.Hands.Remove(cardToRemove!);
            await context.SaveChangesAsync();

            gameEngine.SubmitPlayerCard(gameState.CurrentPlayerIndex, selectedCard);
            gameEngine.GameState.StockPile.Add(selectedCard);
            if (gameState.IsColorChosen == 1 && selectedCard.CardValue != UnoCard.Value.Wild)
            {
                gameState.IsColorChosen = 0;
                gameEngine.GameState.IsColorChosen = false;
            }
            else if (gameState.IsColorChosen == 0 && selectedCard.CardValue == UnoCard.Value.Wild)
            {
                gameState.IsColorChosen = 1;
                gameState.CardColorChoice = new Random().Next(0, 4);
                gameEngine.GameState.IsColorChosen = true;
            }

            await context.SaveChangesAsync();

            gameEngine.GetNextPlayerId(gameState.CurrentPlayerIndex);

            gameState.CurrentPlayerIndex = gameEngine.GameState.CurrentPlayerIndex;
            gameState.GameDirection = gameEngine.GameState.GameDirection ? 1 : 0;

            var nextPlayer = players[gameState.CurrentPlayerIndex];
            foreach (var handCard in gameEngine.GameState.PlayersList[gameState.CurrentPlayerIndex].Hand)
            {
                context.Hands.Add(new Hand
                {
                    CardColor = (int)handCard.CardColor,
                    CardValue = (int)handCard.CardValue,
                    PlayerId = nextPlayer.Id,
                    GameStateId = gameId
                });
                var unoDeckCardToRemove = context.UnoDecks
                    .Where(deckCard =>
                        deckCard.GameStateId == gameId &&
                        deckCard.CardColor == (int)handCard.CardColor &&
                        deckCard.CardValue == (int)handCard.CardValue)
                    .OrderByDescending(deckCard => deckCard.Id)
                    .FirstOrDefault();
                context.UnoDecks.Remove(unoDeckCardToRemove!);
            }

            await CheckForWinner(gameId, playerId);
            
            await context.SaveChangesAsync();

            // Continue with the next AI player if applicable
            if (nextPlayer.Type == (int)Player.PlayerType.Ai)
            {
                await PlayCardAi(gameId, nextPlayer.Id);
            }
        }
        else
        {
            // If no valid cards, draw a card
            selectedCard = gameEngine.GameState.UnoDeck.DrawCard();
            context.Hands.Add(new Hand
            {
                CardColor = (int)selectedCard.CardColor,
                CardValue = (int)selectedCard.CardValue,
                PlayerId = currentPlayer.Id,
                GameStateId = gameId
            });

            // Update database with drawn card
            var unoDeckCardToRemove = context.UnoDecks
                .Where(deckCard =>
                    deckCard.GameStateId == gameId &&
                    deckCard.CardColor == (int)selectedCard.CardColor &&
                    deckCard.CardValue == (int)selectedCard.CardValue)
                .OrderByDescending(deckCard => deckCard.Id) // Order by Id in descending order
                .FirstOrDefault();
            context.UnoDecks.Remove(unoDeckCardToRemove!);

            await context.SaveChangesAsync();

            // Get the next player index
            gameEngine.GetNextPlayerId(gameState.CurrentPlayerIndex);

            gameState.CurrentPlayerIndex = gameEngine.GameState.CurrentPlayerIndex;
            gameState.GameDirection = gameEngine.GameState.GameDirection ? 1 : 0;
            await context.SaveChangesAsync();
            var nextPlayer = players[gameState.CurrentPlayerIndex];

            // Continue with the next AI player if applicable
            if (nextPlayer.Type == (int)Player.PlayerType.Ai)
            {
                await PlayCardAi(gameId, nextPlayer.Id);
            }
        }
    }
    
}

