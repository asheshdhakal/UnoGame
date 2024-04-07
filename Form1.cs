using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace UnoGame
{
    public partial class Form1 : Form
    {
        private Deck deck = new Deck();
        private List<Player> players;
        private const int CardsPerPlayer = 7;
        private string cardBackImagePath = @"Resources\uno_card-back.png";
        private Card currentCard;
        private Player currentPlayer;

       

    


        public Form1()
        {
       

            InitializeComponent();
            InitializeGame();
            initgae();
            //CreateUI(); // Call the method that creates the UI
        }

        private void initgae()
        {

            foreach (var player in players)
            {
                player.Hand.Clear(); // Clear any existing cards

                for (int i = 0; i < CardsPerPlayer; i++)
                {
                    player.DrawCard(deck);
                }
            }


            // Get the first card from the playedCards list
            Card currentCard = deck.playedCards.First();



            DisplayCurrentCard(currentCard);
            DisplayCurrentCardDeck(deck.getFirstCard());
            currentPlayer = players[0];
            //PlayGame();
            DisplayCards();


            for (int i = 0; i < players[0].Hand.Count; i++)
            {
                string pictureBoxName = $"pictureBoxPlayer1Card{i + 1}";
                PictureBox pictureBox = Controls.Find(pictureBoxName, true).FirstOrDefault() as PictureBox;
                if (pictureBox != null)
                {
                    pictureBox.Enabled = true;
                    pictureBox.Click += PictureBox_Click; // Subscribe to the Click event
                }
            }
        }


        private PictureBox CreateCardPictureBox(string name, int x, int y)
        {
            PictureBox pb = new PictureBox();
            pb.Name = name;
            pb.Size = new Size(71, 96); // Standard card size
            pb.Location = new Point(x, y);
            pb.SizeMode = PictureBoxSizeMode.StretchImage;
            pb.Visible = true;
            return pb;
        }
        private void DisplayCurrentCard(Card card)
        {
            pictureBoxCurrentCard.Image = Image.FromFile(card.ImageName);
        }

        private void DisplayCurrentCardDeck(Card card)
        {
            pictureBoxRenewCard.Image = Image.FromFile(card.ImageName);
        }

  
        private void InitializeGame()
        {
            players = new List<Player>();
            players.Add(new Player(true)); // Human player
            for (int i = 0; i < 3; i++)
            {
                players.Add(new Player(false)); // Computer players
            }

            Console.WriteLine(players.Count);
          
        }
        private bool CanPlayCard(Card selectedCard, Card currentCard)
        {
            // Check if the selected card can be played on the current card
            // A card can be played if:
            // 1. The color matches the current card's color
            // 2. The value matches the current card's value
            // 3. The selected card is a wild card
            return selectedCard.Color == currentCard.Color ||
                   selectedCard.Value == currentCard.Value ||
                   selectedCard.Color == "wild";
        }

        // This would be called whenever a card is played
        private void UpdateCurrentCardDisplay(Card card)
        {
            pictureBoxCurrentCard.Image = Image.FromFile(card.ImageName);
        }



        private void DisplayCards()
        {
            // Display human player's cards
            for (int i = 0; i < CardsPerPlayer; i++)
            {
                var pictureBoxName = $"pictureBoxPlayer1Card{i + 1}";
                var pictureBox = Controls.Find(pictureBoxName, true).FirstOrDefault() as PictureBox;
                if (pictureBox != null && i < players[0].Hand.Count)
                {
                    pictureBox.Image = Image.FromFile(players[0].Hand[i].ImageName);
                }
            }

            // Display backs for computer players' cards
            for (int playerIndex = 1; playerIndex <= 3; playerIndex++)
            {
                for (int cardIndex = 0; cardIndex < CardsPerPlayer; cardIndex++)
                {
                    var pictureBoxName = $"pictureBoxPlayer{playerIndex + 1}Card{cardIndex + 1}";
                    var pictureBox = Controls.Find(pictureBoxName, true).FirstOrDefault() as PictureBox;
                    if (pictureBox != null)
                    {
                        pictureBox.Image = Image.FromFile(players[playerIndex].Hand[cardIndex].ImageName);
                    }
                }
            }

        }

        private void PlayGame()
        {
            while (true)
            {
                if (currentPlayer.IsHuman)
                {
                    HandleHumanPlayerTurn();
                    
                }
                else
                {
                    
                    HandleComputerPlayerTurn(currentPlayer);
                    //currentPlayer = players[3];
                }

                // Check if the game is over
                if (CheckGameOver())
                {
                    // Handle game over logic
                    break;
                }

                // Switch to the next player
                int currentPlayerIndex = players.IndexOf(currentPlayer);
                currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
                currentPlayer = players[currentPlayerIndex];
            }
        }



        private void HandleHumanPlayerTurn()
        {
            // Disable all card picture boxes initially
            

            // Enable the picture boxes corresponding to the human player's cards
           
        }
        private void PictureBox_Click(object sender, EventArgs e)
        {
            Console.WriteLine("I am clicked");
            PictureBox clickedPictureBox = sender as PictureBox;
            if (clickedPictureBox != null)
            {
                // Get the index of the clicked card in the human player's hand
                int cardIndex = int.Parse(clickedPictureBox.Name.Substring(clickedPictureBox.Name.Length - 1)) - 1;
                Console.WriteLine(cardIndex);
                Card selectedCard = players[0].Hand[cardIndex];

                // Play the selected card if it's valid
              
                    PlayCard(players[0], selectedCard);
                    currentCard = selectedCard;
                    pictureBoxCurrentCard.Image = Image.FromFile(currentCard.ImageName);
                    HandleSpecialCardEffects(selectedCard); // Handle special card effects
                    SwitchToNextPlayer();
          
            }
            Console.WriteLine("This is for normal deck",deck.cards.Count);
            Console.WriteLine(deck.playedCards.Count);

        }

        private void HandleComputerPlayerTurn(Player player)
        {
            Card validCard = findValidCard(player.Hand, currentCard);
            if (validCard != null)
            {
                PlayCard(player, validCard);
                currentCard = validCard;
                pictureBoxCurrentCard.Image = Image.FromFile(currentCard.ImageName);
                HandleSpecialCardEffects(validCard);
                SwitchToNextPlayer();
            }
            else
            {
                player.DrawCard(deck);
                SwitchToNextPlayer();
            }
        }
        private void PlayCard(Player player, Card card)
        {
            deck.playedCards.Add(card);
            player.Hand.Remove(card);
            UpdateCardDisplay(player);
        }

        private void UpdateCardDisplay(Player player)
        {
            int playerIndex = players.IndexOf(player) + 1;
            for (int i = 0; i < CardsPerPlayer; i++)
            {
                string pictureBoxName = $"pictureBoxPlayer{playerIndex}Card{i + 1}";
                PictureBox pictureBox = Controls.Find(pictureBoxName, true).FirstOrDefault() as PictureBox;
                if (pictureBox != null)
                {
                    if (i < player.Hand.Count)
                    {
                        if (player.IsHuman)
                        {
                            pictureBox.Image = Image.FromFile(player.Hand[i].ImageName);
                        }
                        else
                        {
                            pictureBox.Image = Image.FromFile(cardBackImagePath);
                        }
                    }
                    else
                    {
                        pictureBox.Image = null;
                    }
                }
            }
        }
        private void HandleSpecialCardEffects(Card card)
        {
            switch (card.Value)
            {

           case "skip":
                SwitchToNextPlayer();
                break;
            case "reverse":
                ReversePlayerOrder();
                break;
            case "draw2":
                DrawCardsForNextPlayer(2);
                SwitchToNextPlayer();
                break;
            case "wild":
                // Prompt the player to choose a new color
                ColorDialog colorDialog = new ColorDialog();
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    card.Color = colorDialog.Color.Name;
                }
                break;
            case "draw4":
                DrawCardsForNextPlayer(4);
                SwitchToNextPlayer();
                // Prompt the player to choose a new color
                ColorDialog colorDialog2 = new ColorDialog();
                if (colorDialog2.ShowDialog() == DialogResult.OK)
                {
                    card.Color = colorDialog2.Color.Name;
                }
                break;
            }
            }

        private void DrawCardsForNextPlayer(int numCards)
        {
            int nextPlayerIndex = (players.IndexOf(currentPlayer) + 1) % players.Count;
            Player nextPlayer = players[nextPlayerIndex];
            for (int i = 0; i < numCards; i++)
            {
                nextPlayer.DrawCard(deck);
            }
            UpdateCardDisplay(nextPlayer);
        }

        private void SwitchToNextPlayer()
        {
            int currentPlayerIndex = players.IndexOf(currentPlayer);
            currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
            currentPlayer = players[currentPlayerIndex];
        }

        private void ReversePlayerOrder()
        {
            players.Reverse();
        }


    
        private Card findValidCard(List<Card> hand, Card currentCard)
        {
            // Implement the logic to find a valid card based on the Uno game rules
            // Check if any card in the player's hand matches the current card's color or value
            // Return the valid card, or null if no valid card is found
            foreach (Card card in hand)
            {
                if (card.Color == currentCard.Color || card.Value == currentCard.Value || card.Color == "wild")
                {
                    return card;
                }
            }
            return null;
        }

     

        private bool CheckGameOver()
        {
           
            return false;
        }

      

        private void pictureBoxPlayer1Card1_Click(object sender, EventArgs e)
        {

        }

        private void pictureBoxCurrentCard_Click(object sender, EventArgs e)
        {
           
        }

        private void panelPlayer1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBoxRenewCard_Click(object sender, EventArgs e)
        {
            currentPlayer.Hand.Add(deck.Draw());
            Console.WriteLine(currentPlayer.Hand.Count);
            UpdateCardDisplay(currentPlayer);
        }

        private void pictureBoxCurrentCard_Click_1(object sender, EventArgs e)
        {

        }
    }

    // Other classes remain the same as provided
    // ...
}
