using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnoGame
{
  
    public class UnoRules
    {
        public static void ApplyCardEffect(UnoCard card, UnoPlayer currentPlayer, UnoPlayer nextPlayer, ref bool reverseDirection)
        {
            switch (card.Type)
            {
                case CardType.Skip:
                    SkipNextPlayer(nextPlayer);
                    break;
                case CardType.Reverse:
                    reverseDirection = !reverseDirection;
                    break;
                case CardType.DrawTwo:
                    DrawCards(nextPlayer, 2);
                    SkipNextPlayer(nextPlayer);
                    break;
                case CardType.Wild:
                    // Here you can implement logic to choose the color for the next player
                    break;
                case CardType.WildDrawFour:
                    // Here you can implement logic to choose the color for the next player
                    DrawCards(nextPlayer, 4);
                    SkipNextPlayer(nextPlayer);
                    break;
                default:
                    break;
            }
        }

        private static void SkipNextPlayer(UnoPlayer player)
        {
            player.IsTurnSkipped = true;
        }

        private static void DrawCards(UnoPlayer player, int count)
        {
            for (int i = 0; i < count; i++)
            {
                player.DrawCard();
            }
        }
    }
}
