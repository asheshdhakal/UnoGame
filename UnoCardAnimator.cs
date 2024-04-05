using System;
using System.Drawing;
using System.Windows.Forms;

public class UnoCardAnimator
{
    private Panel mainDeckPanel;
    private Timer moveTimer;
    private const int MoveSpeed = 5;

    public UnoCardAnimator(Panel mainDeckPanel)
    {
        this.mainDeckPanel = mainDeckPanel;

        moveTimer = new Timer();
        moveTimer.Interval = 10;
        moveTimer.Tick += MoveTimer_Tick;
    }

    public void AnimateCardToDeck(Panel cardPanel)
    {
        // Start the timer to animate the card to the deck
        moveTimer.Tag = cardPanel;
        moveTimer.Start();
    }

    private void MoveTimer_Tick(object sender, EventArgs e)
    {
        // Move the card towards the main deck
        Timer timer = sender as Timer;
        Panel cardPanel = timer.Tag as Panel;

        Point cardLocation = cardPanel.Location;
        Point deckLocation = mainDeckPanel.Location;

        if (cardLocation.X < deckLocation.X)
        {
            cardLocation.X += MoveSpeed;
        }
        else if (cardLocation.X > deckLocation.X)
        {
            cardLocation.X -= MoveSpeed;
        }

        if (cardLocation.Y < deckLocation.Y)
        {
            cardLocation.Y += MoveSpeed;
        }
        else if (cardLocation.Y > deckLocation.Y)
        {
            cardLocation.Y -= MoveSpeed;
        }

        cardPanel.Location = cardLocation;

        // Once the card reaches the main deck, stop the timer
        if (cardLocation == deckLocation)
        {
            moveTimer.Stop();
            mainDeckPanel.Controls.Add(cardPanel); // Add the card panel to the main deck panel
        }
    }
}