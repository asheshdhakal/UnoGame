using System.Windows.Forms;
using UnoGame;

namespace UnoGame
{
    // Represents a player in the Uno game.
    public class Player : BasePlayer
    {
        private Form1 form;
        public bool IsHuman { get; private set; }

        public Player(bool isHuman, string name, Form1 form) : base(name)
        {
            IsHuman = isHuman;
            this.form = form;
        }

        public override void DrawCard(Deck deck)
        {
            // check if the hand is full
            if (Hand.Count >= 7)
            {
                if (IsHuman)
                {
                    DialogResult result = MessageBox.Show("Your hand is full. You cannot draw more cards.", "Hand Full", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);

                    if (result == DialogResult.OK)
                    {
                        form.SwitchToNextPlayer();
                    }
                }
                else
                {
                    form.SwitchToNextPlayer();
                }
            }
            else
            {                
                // draw a card and update the card display if human

                base.DrawCard(deck);

                if (IsHuman)
                {
                    form.UpdateCardDisplay(this);
                }
            }
        }
    }


}