using System;
using System.Collections.Generic;
using System.Linq;
using Blackjack.Enums;

namespace Blackjack
{
    public class Game
    {
        private readonly Dictionary<string, int> _dealersCard = new Dictionary<string, int>();
        private float _dealerAceBonus = 0f;
        private readonly List<Player> _players = new();
        private GameState _gameState;

        private readonly Card _card = new();

        public Game()
        {
            _gameState = GameState.FillPlayers;
            GameLoop();
        }

        private void GiveInitCardsToPlayers()
        {
            for (var i = 0; i < _players.Count; i++)
            {
                var card = _card.DrawRandomCard();
                _players[i].Cards.Add(card);
                CheckAceCard(card, _players[i]);
                Console.WriteLine($"{_players[i].Name} name got a card {card}");
                
                card = _card.DrawRandomCard();
                _players[i].Cards.Add(card);
                CheckAceCard(card, _players[i]);
                Console.WriteLine($"{_players[i].Name} name got a card {card}\n");
            }

            var dealerCard = _card.DrawRandomCard();


            if (dealerCard == "AS" || dealerCard == "AC" || dealerCard == "AH" || dealerCard == "AD")
            {
                var aceBonus = new[] { 1, 11 }[new Random(DateTime.Now.Millisecond).Next(2)];
                _dealersCard.Add(dealerCard, aceBonus);
            }
            else
            {
                _dealersCard.Add(dealerCard, Card.Cards[dealerCard]);
            }
                
            
            Console.WriteLine($"Dealer got a {dealerCard} and XX cards");
            
            dealerCard = _card.DrawRandomCard();
            
            if (dealerCard == "AS" || dealerCard == "AC" || dealerCard == "AH" || dealerCard == "AD")
            {
                var aceBonus = new[] { 1, 11 }[new Random(DateTime.Now.Millisecond).Next(2)];
                _dealersCard.Add(dealerCard, aceBonus);
            }
            else
            {
                _dealersCard.Add(dealerCard, Card.Cards[dealerCard]);
            }
            
            CheckWinOrLoseState();
        }

        private void CheckAceCard(string card, Player player)
        {
            if (card == "AS" || card == "AC" || card == "AH" || card == "AD")
            {
                Console.WriteLine($"Player {player} got an Ace, choose what value it should be (1 or 11):");
                var stringValue = Console.ReadLine();
                var value = int.Parse(stringValue);
                player.Bet += value;
            }
        }

        private void CheckWinOrLoseState()
        {
            foreach (var player in _players)
            {
                var sum = 0;
                
                foreach (var cards in player.Cards)
                    sum += Card.Cards[cards];

                sum += player.AcePoints;

                if (sum == 21)
                {
                    Console.WriteLine($"Player {player.Name} has won!");
                    player.Money += player.Bet * 1.5f;
                    player.Bet = 0f;
                    player.IsPlaying = false;
                    player.IsStanding = true;
                } else if (sum > 21)
                {
                    Console.WriteLine($"Player {player.Name} has lost!");
                    player.Bet = 0f;
                    player.IsPlaying = false;
                    player.IsStanding = true;
                    player.IsLoser = true;
                }
            }
        }

        private void FillPlayers()
        {
            Console.WriteLine("How many players will be playing?");
            var playerCount = Console.ReadLine();

            if (int.TryParse(playerCount, out var result))
            {
                for (int i = 0; i < result; i++)
                {
                    Console.WriteLine($"What is the name of the {i + 1} player");
                    var name = Console.ReadLine();
                    
                    if(name?.Length == 0)
                        return;
                    
                    _players.Add(new Player
                    {
                        Money = 1000,
                        Name = name
                    });
                }
            }

            _gameState = GameState.SetPlayerBetAmount;
        }

        private void SelectPlayersBetAmount()
        {
            foreach (var player in _players)
            {
                Console.WriteLine($"Player {player.Name} how much do you want to bet? (Balance: {player.Money})");
                var betValue = Console.ReadLine();
                var bet = float.Parse(betValue);
                
                if (bet > player.Money || bet <= 0)
                {
                    player.IsLoser = true;
                    player.IsStanding = true;
                    player.IsPlaying = false;
                }
                else
                {
                    player.Bet = bet;
                    player.Money -= bet;
                }
            }

            _gameState = GameState.GiveInitialCardsCards;
        }
        
        private void PlayerChoice()
        {
            while (_players.Exists(p => !p.IsStanding && p.IsPlaying && !p.IsLoser))
            {
                foreach (var player in _players)
                {
                    if (player.IsPlaying && !player.IsStanding && !player.IsLoser)
                    {
                        Console.WriteLine($"Player {player.Name}, write what you want to do: (h - take one more card, s - dont take card)");
                        Console.WriteLine("Your cards are:");
                        foreach (var card in player.Cards)
                            Console.Write($"{card} ");
                        
                        var choice = Console.ReadLine();

                        if (choice.Equals("h", StringComparison.CurrentCultureIgnoreCase))
                        {
                            var card = _card.DrawRandomCard();
                            player.Cards.Add(card);
                            CheckAceCard(card, player);
                            Console.WriteLine($"{player.Name} name got a card {card}\n");
                            
                            CheckWinOrLoseState();
                        } else if (choice.Equals("s", StringComparison.CurrentCultureIgnoreCase))
                        {
                            player.IsPlaying = false;
                            player.IsStanding = true;
                            Console.WriteLine($"Player {player.Name} choice to stand");
                        }
                    }
                }
            }

            _gameState = GameState.RevealDealerCard;
        }

        private void RevealDealerCard()
        {
            Console.WriteLine("Revealing dealers cards:");

            foreach (var card in _dealersCard)
            {
                if (card.Key == "AS" || card.Key == "AC" || card.Key == "AH" || card.Key == "AD")
                    Console.WriteLine($"{card} with a value of {card.Value}");
                else
                {
                    Console.WriteLine($"{card}");
                }
            }

            _gameState = GameState.WinCounter;
        }

        private void WinCounter()
        {
            var dealerCardSum = 0;
            foreach (var point in _dealersCard.Values)
                dealerCardSum += point;

            if (dealerCardSum <= 16)
            {
                Console.WriteLine("Since dealers point value is equals or less than 16, dealer is drawing one more card");
                
                var card = _card.DrawRandomCard();
                
                if (card == "AS" || card == "AC" || card == "AH" || card == "AD")
                {
                    if (dealerCardSum + 11 > 21)
                    {
                        
                        _dealersCard.Add(card, 1);
                        dealerCardSum += 1;
                    }
                    else
                    {
                        Console.WriteLine($"Dealer got a {card} with a value of 11");
                        _dealersCard.Add(card, 11);
                        dealerCardSum += 11;
                    }
                }
                else
                {
                    Console.WriteLine($"Dealer got a {card}");
                    _dealersCard.Add(card, Card.Cards[card]);
                    dealerCardSum += Card.Cards[card];
                }
            }

            if (dealerCardSum > 21)
            {
                foreach (var player in _players.Where(p => !p.IsLoser && p.IsStanding && !p.IsPlaying))
                {
                    player.Money += player.Bet * 2f;
                    Console.WriteLine($"Player {player.Name} has won {player.Bet * 2f}!");
                    player.Bet = 0f;
                }
                
                _gameState = GameState.RestartGame;
                
                return;
            }

            if (dealerCardSum == 21)
            {
                foreach (var player in _players.Where(p => !p.IsLoser && p.IsStanding && !p.IsPlaying))
                {
                    Console.WriteLine($"Player {player.Name} has lost {player.Bet}!");
                    player.Bet = 0f;
                }
            }
            else
            {
                foreach (var player in _players.Where(p => !p.IsLoser && p.IsStanding && !p.IsPlaying))
                {
                    if (player.SumPoints > dealerCardSum)
                    {
                        player.Money += player.Bet * 2f;
                        Console.WriteLine($"Player {player.Name} has won {player.Bet}!");
                        player.Bet = 0f;
                    }
                    else
                    {
                        Console.WriteLine($"Player {player.Name} has lost {player.Bet}!");
                        player.Bet = 0f;
                    }
                }
            }

            _gameState = GameState.RestartGame;
        }

        private void RestartGame()
        {
            _dealersCard.Clear();
            _dealerAceBonus = 0f;
            _card.ResetCards();

            foreach (var player in _players)
                player.ResetForGame();

            _gameState = GameState.SetPlayerBetAmount;
        }
        
        private void GameLoop()
        {
            while (true)
            {
                var doesPlayersHaveMoney = !_players.Exists(p => p.Money > 0);
                
                if(!doesPlayersHaveMoney)
                    return;
                
                switch (_gameState)
                {
                    case GameState.FillPlayers:
                        FillPlayers();
                        break;
                    case GameState.SetPlayerBetAmount:
                        SelectPlayersBetAmount();
                        GiveInitCardsToPlayers();
                        break;
                    case GameState.GiveInitialCardsCards:
                        PlayerChoice();
                        break;
                    case GameState.RevealDealerCard:
                        RevealDealerCard();
                        break;
                    case GameState.WinCounter:
                        WinCounter();
                        break;
                    case GameState.RestartGame:
                        RestartGame();
                        break;
                }
            }
        }
    }
}