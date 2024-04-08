using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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
            DisplayDeckBack();

            string backgroundImageFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\table.jpg");
            if (File.Exists(backgroundImageFilePath))
            {
                this.BackgroundImage = Image.FromFile(backgroundImageFilePath);
                this.BackgroundImageLayout = ImageLayout.Stretch; // Adjust this as needed
            }
            else
            {
                MessageBox.Show("Background image file not found.");
            }
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

            // Correctly initialize the class-level currentCard with the first card from the playedCards list
            currentCard = deck.playedCards.First(); // Remove the local declaration and use the class-level currentCard

            DisplayCurrentCard(currentCard);
            DisplayCurrentCardDeck(deck.getFirstCard());
            currentPlayer = players[0];
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

        private void DisplayDeckBack()
{
    string backImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "uno_card-back.png");
    if (File.Exists(backImagePath))
    {
        pictureBoxRenewCard.Image = Image.FromFile(backImagePath);
    }
    else
    {
        MessageBox.Show("Deck back image file not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
        // Correctly handle the initiation of the game loop.
        private void InitiateGameLoop()
        {
            PlayGame();
        }

        private bool CanPlayCard(Card selectedCard, Card currentCard)
        {
            // Check if the current card is not null
            if (currentCard != null)
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
            else
            {
                // If the current card is null, return false
                return false;
            }
        }

        // This would be called whenever a card is played
        private void UpdateCurrentCardDisplay(Card card)
        {
            pictureBoxCurrentCard.Image = Image.FromFile(card.ImageName);
        }



        private void DisplayCards()
        {
            string resourcesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");

            // Display human player's cards
            for (int i = 0; i < CardsPerPlayer; i++)
            {
                string pictureBoxName = $"pictureBoxPlayer1Card{i + 1}";
                PictureBox pictureBox = Controls.Find(pictureBoxName, true).FirstOrDefault() as PictureBox;
                if (pictureBox != null && i < players[0].Hand.Count)
                {
                    Card card = players[0].Hand[i];
                    string imageName = card.Color == "wild" ? Path.Combine(resourcesPath, card.ImageName) : Path.Combine(resourcesPath, $"uno_card-{card.Color}{card.Value}.png");
                    pictureBox.Image = Image.FromFile(imageName);
                }
            }

            // Display backs for computer players' cards
            for (int playerIndex = 1; playerIndex <= 3; playerIndex++)
            {
                for (int cardIndex = 0; cardIndex < CardsPerPlayer; cardIndex++)
                {
                    string pictureBoxName = $"pictureBoxPlayer{playerIndex + 1}Card{cardIndex + 1}";
                    PictureBox pictureBox = Controls.Find(pictureBoxName, true).FirstOrDefault() as PictureBox;
                    if (pictureBox != null)
                    {
                        string backImagePath = Path.Combine(resourcesPath, "uno_card-back.png");
                        pictureBox.Image = Image.FromFile(backImagePath);
                    }
                }
            }
        }
        private void PlayGame()
        {
            if (currentPlayer.IsHuman)
            {
                HandleHumanPlayerTurn();
            }
            else
            {
                HandleComputerPlayerTurn(currentPlayer);
            }

            // Check if the game is over
            if (!CheckGameOver())
            {
                // Switch to the next player after a delay
                Task.Delay(2000).ContinueWith(_ =>
                {
                    int currentPlayerIndex = players.IndexOf(currentPlayer);
                    currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
                    currentPlayer = players[currentPlayerIndex];
                    PlayGame(); // Recursive call to continue the game
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }




        private void HandleHumanPlayerTurn()
        {
            // Enable the picture boxes corresponding to the human player's cards
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
        private void PictureBox_Click(object sender, EventArgs e)
        {
            PictureBox clickedPictureBox = sender as PictureBox;
            if (clickedPictureBox != null)
            {
                int cardIndex = int.Parse(clickedPictureBox.Name.Substring(clickedPictureBox.Name.Length - 1)) - 1;
                Card selectedCard = players[0].Hand[cardIndex];

                if (CanPlayCard(selectedCard, currentCard))
                {
                    if (selectedCard.Color == "wild" || selectedCard.Value == "draw4")
                    {
                        ColorDialog colorDialog = new ColorDialog
                        {
                            AllowFullOpen = false,
                            AnyColor = false,
                            FullOpen = false,
                            CustomColors = new int[] { 0x0000FF, 0x00FF00, 0xFF0000, 0xFFFF00 }, // Red, Green, Blue, Yellow
                        };
                        if (colorDialog.ShowDialog() == DialogResult.OK)
                        {
                            selectedCard.Color = colorDialog.Color.Name;
                            buttonCurrentColour.BackColor = colorDialog.Color;
                            if (selectedCard.Value == "draw4")
                            {
                                DrawCardsForNextPlayer(4);
                            }
                        }
                    }

                    PlayCard(players[0], selectedCard);
                    currentCard = selectedCard;
                    pictureBoxCurrentCard.Image = Image.FromFile(currentCard.ImageName);
                    HandleSpecialCardEffects(selectedCard);
                    SwitchToNextPlayer();
                }
            }
        }

        private void HandleComputerPlayerTurn(Player player)
        {
            // Find a valid card that the computer player can play
            Card validCard = FindValidCard(player.Hand, currentCard);

            if (validCard != null)
            {
                // If a valid card is found, play it and update the UI accordingly
                PlayCard(player, validCard);

                // Update the current card to the one played by the computer player
                currentCard = validCard;

                // Update the current card display with the new current card
                UpdateCurrentCardDisplay(currentCard);

                // Handle any special effects the card may have
                HandleSpecialCardEffects(validCard);

                // Reflect the computer player's action in their respective PictureBoxes
                UpdateCardDisplay(player);
            }
            else
            {
                // If no valid card is found, the computer player draws a card
                player.DrawCard(deck);

                // Update the computer player's card display to reflect the new card
                UpdateCardDisplay(player);
            }

            // Move to the next player's turn
            SwitchToNextPlayer();
        }



        private Card FindValidCard(List<Card> hand, Card currentCard)
        {
            // Check if any card in the player's hand matches the current card's color or value
            foreach (Card card in hand)
            {
                if (card.Color == currentCard.Color || card.Value == currentCard.Value || card.Color == "wild")
                {
                    return card;
                }
            }

            // If no valid card is found, return null
            return null;
        }
        private void PlayCard(Player player, Card card)
        {
            deck.playedCards.Add(card);
            player.Hand.Remove(card);
            UpdateCardDisplay(player);

            // Check if the player has won after playing a card
            CheckForWin(player);
        }
        private void CheckForWin(Player player)
        {
            if (player.Hand.Count == 0)
            {
                // Display win message
                var winMessage = player.IsHuman ? "Congratulations! You've won the game!" : "A computer player has won the game!";
                MessageBox.Show(winMessage, "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
        }
        private void UpdateCardDisplay(Player player)
        {
            int playerIndex = players.IndexOf(player) + 1; // Assuming playerIndex starts from 1 for naming

            for (int i = 0; i < CardsPerPlayer; i++)
            {
                string pictureBoxName = $"pictureBoxPlayer{playerIndex}Card{i + 1}";
                PictureBox pictureBox = Controls.Find(pictureBoxName, true).FirstOrDefault() as PictureBox;

                if (pictureBox != null)
                {
                    if (i < player.Hand.Count)
                    {
                        string imagePath = player.IsHuman ?
                            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", player.Hand[i].ImageName) :
                            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "uno_card-back.png");

                        pictureBox.Image = Image.FromFile(imagePath);
                    }
                    else
                    {
                        pictureBox.Image = null;
                    }
                }
            }
        }


        /*private void UpdateCardDisplay(Player player)
{
    int playerIndex = players.IndexOf(player) + 1; // Assuming playerIndex starts from 1 for naming

    for (int i = 0; i < CardsPerPlayer; i++)
    {
        string pictureBoxName = $"pictureBoxPlayer{playerIndex}Card{i + 1}";
        PictureBox pictureBox = Controls.Find(pictureBoxName, true).FirstOrDefault() as PictureBox;

        if (pictureBox != null)
        {
            // Temporarily show the front of the cards for all players during testing
            // Comment this section out before production deployment
            if (i < player.Hand.Count)
            {
                pictureBox.Image = Image.FromFile(player.Hand[i].ImageName);
            }
            else
            {
                pictureBox.Image = null;
            }

            // Uncomment below for production to show back of card for computer players
            *//*
            if (player.IsHuman)
            {
                Image cardImage = i < player.Hand.Count ? Image.FromFile(player.Hand[i].ImageName) : null;
                pictureBox.Invoke(new Action(() => pictureBox.Image = cardImage));
            }
            else
            {
                // This line sets the back image for computer players, comment it out for testing
                // Image backImage = i < player.Hand.Count ? Image.FromFile(cardBackImagePath) : null;
                // pictureBox.Invoke(new Action(() => pictureBox.Image = backImage));
            }
            *//*
        }
    }
}*/



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
                case "draw4":
                    if (!currentPlayer.IsHuman)
                    {
                        // Automatically choose the color for the computer player
                        card.Color = ChooseColorForComputerPlayer(currentPlayer);
                        buttonCurrentColour.BackColor = System.Drawing.Color.FromName(card.Color);
                        if (card.Value == "draw4")
                        {
                            DrawCardsForNextPlayer(4);
                        }
                    }
                    else
                    {
                        // Show the color dialog for the human player to choose
                        ColorDialog colorDialog = new ColorDialog
                        {
                            AllowFullOpen = false,
                            AnyColor = false,
                            FullOpen = false,
                            CustomColors = new int[] { 0x0000FF, 0x00FF00, 0xFF0000, 0xFFFF00 }, // Red, Green, Blue, Yellow
                        };
                        if (colorDialog.ShowDialog() == DialogResult.OK)
                        {
                            card.Color = colorDialog.Color.Name; // Assuming the name matches your game's color strings
                            buttonCurrentColour.BackColor = colorDialog.Color;
                            if (card.Value == "draw4")
                            {
                                DrawCardsForNextPlayer(4);
                            }
                        }
                    }
                    SwitchToNextPlayer();
                    break;
            }
        }
        private string ChooseColorForComputerPlayer(Player player)
        {
            // Count the occurrences of each color in the computer player's hand
            var colorCount = player.Hand
                .Where(card => card.Color != "wild")
                .GroupBy(card => card.Color)
                .ToDictionary(group => group.Key, group => group.Count());

            // If the computer has no colored cards left, just return a random color
            if (colorCount.Count == 0)
                return new[] { "red", "yellow", "green", "blue" }.OrderBy(c => Guid.NewGuid()).First();

            // Otherwise, select the color which occurs most frequently
            return colorCount.OrderByDescending(kvp => kvp.Value).First().Key;
        }
        private void DrawCardsForNextPlayer(int numCards)
        {
            int nextPlayerIndex = (players.IndexOf(currentPlayer) + 1) % players.Count;
            Player nextPlayer = players[nextPlayerIndex];
            for (int i = 0; i < numCards; i++)
            {
                if (deck.cards.Count == 0)
                {
                    deck.ReshufflePlayedCards(); // Reshuffle played cards into the deck
                }
                nextPlayer.DrawCard(deck);
            }
            UpdateCardDisplay(nextPlayer);
        }


        private void SwitchToNextPlayer()
        {
            int currentPlayerIndex = players.IndexOf(currentPlayer);
            currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
            currentPlayer = players[currentPlayerIndex];

            if (!currentPlayer.IsHuman)
            {
                // Simulate a slight delay for computer player actions for better UX
                Task.Delay(1000).ContinueWith(t =>
                {
                    Invoke(new Action(() =>
                    {
                        HandleComputerPlayerTurn(currentPlayer);
                    }));
                });
            }
            // For human players, the UI awaits user interaction.
        }


        private void ReversePlayerOrder()
        {
            players.Reverse();
        }


    
        private Card findValidCard(List<Card> hand, Card currentCard)
        {
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
            if (currentPlayer.IsHuman)
            {
                var drawnCard = deck.Draw();
                if (drawnCard != null)
                {
                    currentPlayer.Hand.Add(drawnCard);
                    UpdateCardDisplay(currentPlayer);
                }
                else
                {
                    MessageBox.Show("The deck is empty!");
                }
            }
        }


        private void pictureBoxCurrentCard_Click_1(object sender, EventArgs e)
        {

        }
    }


}
