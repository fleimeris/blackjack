using System.Collections.Generic;
using System.Linq;

namespace Blackjack
{
    public class Player
    {
        public string Name { get; set; }
        public float Money { get; set; } = 0f;
        public float Bet { get; set; } = 0f;
        public int AcePoints { get; set; } = 0;
        public bool IsPlaying = true;
        public bool IsStanding = false;
        public bool IsLoser = false;
        
        public List<string> Cards { get; } = new();

        public int SumPoints
        {
            get
            {
                var sum = Cards.Sum(card => Card.Cards[card]);

                return sum += AcePoints;
            }
        }

        public void ResetForGame()
        {
            IsLoser = false;
            IsStanding = false;
            IsPlaying = true;
            Bet = 0f;
            AcePoints = 0;
            Cards.Clear();
        }
    }
}