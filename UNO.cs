using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnoGame.PlayerActivityLogger;

namespace UnoGame
{
    public partial class Form1 : Form
    {
        private Deck deck = new Deck();
        private List<Player> players;
        private const int CardsPerPlayer = 7;
        private string resourcesFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
        private Card currentCard;
        private Player currentPlayer;
        private bool isPlayDirectionClockwise = true;
        private System.Media.SoundPlayer player = new System.Media.SoundPlayer();

        public Form1()
        {
            // setting up the form
            InitializeComponent();
            InitializeGame();
            InitializeGameEnvironment();
            DisplayDeckBack();
            pictureBoxRenewCard.Click += pictureBoxRenewCard_Click;

            // setting up the game background
            string backgroundImageFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\table.jpg");
            if (File.Exists(backgroundImageFilePath))
            {
                // if background image exists, use it
                this.BackgroundImage = Image.FromFile(backgroundImageFilePath);
                this.BackgroundImageLayout = ImageLayout.Stretch;
            }
        }

        private void InitializeGameEnvironment()
        {
            // preparing each player for the game
            foreach (var player in players)
            {
                player.Hand.Clear();
                // giving cards to each player
                for (int i = 0; i < CardsPerPlayer; i++)
                {
                    player.DrawCard(deck);
                }
            }

            // picking a card that's not special as the starting card
            do
            {
                currentCard = deck.Draw();
            }
            while (currentCard.Value == "wild" ||
                   currentCard.Value == "draw4" ||
                   currentCard.Value == "draw2" ||
                   currentCard.Value == "reverse" ||
                   currentCard.Value == "skip");

            // adding the first card to the played cards pile
            deck.playedCards.Add(currentCard);

            // showing the current card and deck on the form
            DisplayCurrentCard(currentCard);
            DisplayCurrentCardDeck(deck.getFirstCard());

            // starting with the first player
            currentPlayer = players[0];

            // displaying all players' cards
            DisplayCards();

            // enabling click events on the human player's cards
            for (int i = 0; i < players[0].Hand.Count; i++)
            {
                string pictureBoxName = $"pictureBoxPlayer1Card{i + 1}";
                PictureBox pictureBox = Controls.Find(pictureBoxName, true).FirstOrDefault() as PictureBox;
                if (pictureBox != null)
                {
                    pictureBox.Enabled = true;
                    pictureBox.Click += PictureBox_Click;
                }
            }

            // setting the color indicator on the form
            string colorName = currentCard.Color;
            UpdateButtonCurrentColor(colorName);
        }

        private void DisplayDeckBack()
        {
            // showing the back of the deck
            string backImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "uno_card-back.png");
            if (File.Exists(backImagePath))
            {
                pictureBoxRenewCard.Image = Image.FromFile(backImagePath);
            }
        }

        private void DisplayCurrentCard(Card card)
        {
            // showing the current card
            pictureBoxCurrentCard.Image = Image.FromFile(card.ImageName);
        }

        private void DisplayCurrentCardDeck(Card card)
        {
            // showing the deck on the form
            pictureBoxRenewCard.Image = Image.FromFile(card.ImageName);
        }


        private void InitializeGame()
        {
            // initialize players for the game
            players = new List<Player>
    {
        new Player(true, "Human", this),
        new Player(false, "Computer 1", this),
        new Player(false, "Computer 2", this),
        new Player(false, "Computer 3", this)
    };
        }

        private bool CanPlayCard(Card selectedCard, Card currentCard)
        {
            // check if a card can be legally played
            return selectedCard.Value == "wild" || selectedCard.Value == "draw4" ||
                   (selectedCard.Color == currentCard.Color || selectedCard.Value == currentCard.Value);
        }

        private void UpdateCurrentCardDisplay(Card card)
        {
            // update the display for the current card
            pictureBoxCurrentCard.Image = Image.FromFile(card.ImageName);
            UpdateButtonCurrentColor(card.Color);
        }

        private void UpdateButtonCurrentColor(string color)
        {
            // change the button color based on the current card's color
            switch (color)
            {
                case "red":
                    buttonCurrentColour.BackColor = Color.Red;
                    break;
                case "green":
                    buttonCurrentColour.BackColor = Color.Green;
                    break;
                case "blue":
                    buttonCurrentColour.BackColor = Color.Blue;
                    break;
                case "yellow":
                    buttonCurrentColour.BackColor = Color.Yellow;
                    break;
                default:
                    buttonCurrentColour.BackColor = DefaultBackColor;
                    break;
            }
        }

        private void DisplayCards()
        {
            // display all cards for each player
            string resourcesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
            for (int i = 0; i < CardsPerPlayer; i++)
            {
                string pictureBoxName = $"pictureBoxPlayer1Card{i + 1}";
                PictureBox pictureBox = Controls.Find(pictureBoxName, true).FirstOrDefault() as PictureBox;
                if (pictureBox != null && i < players[0].Hand.Count)
                {
                    string imageName = Path.Combine(resourcesPath, $"uno_card-{players[0].Hand[i].Color}{players[0].Hand[i].Value}.png");
                    pictureBox.Image = File.Exists(imageName) ? Image.FromFile(imageName) : Image.FromFile(Path.Combine(resourcesPath, "uno_card-wildchange.png"));
                }
            }
            // set back image for non-human players
            for (int playerIndex = 1; playerIndex <= 3; playerIndex++)
            {
                for (int cardIndex = 0; cardIndex < CardsPerPlayer; cardIndex++)
                {
                    string pictureBoxName = $"pictureBoxPlayer{playerIndex + 1}Card{cardIndex + 1}";
                    PictureBox pictureBox = Controls.Find(pictureBoxName, true).FirstOrDefault() as PictureBox;
                    if (pictureBox != null)
                        pictureBox.Image = Image.FromFile(Path.Combine(resourcesPath, "uno_card-back.png"));
                }
            }
        }

        private void PlayGame()
        {
            // manage the flow of the game
            if (currentPlayer.IsHuman)
                HandleHumanPlayerTurn();
            else
                HandleComputerPlayerTurn(currentPlayer);

            if (!CheckGameOver())
                Task.Delay(2000).ContinueWith(_ => {
                    currentPlayer = players[(players.IndexOf(currentPlayer) + 1) % players.Count];
                    PlayGame();
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private void HandleHumanPlayerTurn()
        {
            // enable interaction for human player
            for (int i = 0; i < players[0].Hand.Count; i++)
            {
                string pictureBoxName = $"pictureBoxPlayer1Card{i + 1}";
                PictureBox pictureBox = Controls.Find(pictureBoxName, true).FirstOrDefault() as PictureBox;
                if (pictureBox != null)
                {
                    pictureBox.Enabled = true;
                    pictureBox.Click += PictureBox_Click;
                }
            }
        }

        // Handles the click event on a PictureBox representing a card.

        private void PictureBox_Click(object sender, EventArgs e)

        {    // Retrieve the clicked PictureBox and ensure it has an image.

            PictureBox clickedPictureBox = sender as PictureBox;
            if (clickedPictureBox != null && clickedPictureBox.Image != null)
            {
                // Extract the card index from the PictureBox's name and get the corresponding card.

                int cardIndex = int.Parse(clickedPictureBox.Name.Substring(clickedPictureBox.Name.Length - 1)) - 1;
                Card selectedCard = players[0].Hand[cardIndex];
                if (CanPlayCard(selectedCard, currentCard))
                {
                    if (selectedCard.Value == "draw2")
                    {
                        PlayCard(players[0], selectedCard);
                        currentCard = selectedCard;
                        UpdateCurrentCardDisplay(currentCard);
                        HandleDrawTwoCard(currentPlayer);
                    }
                    else if (selectedCard.Value == "reverse")
                    {
                        PlayCard(players[0], selectedCard);
                        currentCard = selectedCard;
                        UpdateCurrentCardDisplay(currentCard);
                        HandleReverseCard();
                        SwitchToNextPlayer();
                    }
                    else if (selectedCard.Value == "skip")
                    {
                        PlayCard(players[0], selectedCard);
                        currentCard = selectedCard;
                        UpdateCurrentCardDisplay(currentCard);
                        HandleSkipCard(currentPlayer);
                    }
                    else if (selectedCard.Color == "wild")
                    {
                        string chosenColor = ChooseColor();
                        if (!string.IsNullOrEmpty(chosenColor))
                        {
                            PlayCard(players[0], selectedCard);
                            currentCard = new Card(chosenColor, selectedCard.Value, selectedCard.ImageName);
                            UpdateCurrentCardDisplay(currentCard);
                            if (selectedCard.Value == "draw4")
                            {
                                HandleDrawFourCard(currentPlayer);
                            }
                            else
                            {
                                SwitchToNextPlayer();
                            }
                        }
                    }
                    else
                    {
                        PlayCard(players[0], selectedCard);
                        currentCard = selectedCard;
                        UpdateCurrentCardDisplay(currentCard);
                        SwitchToNextPlayer();
                    }

                    PlayCard(players[0], selectedCard);  
                    ActivityLogger.LogPlayerActivity(players[0].Name, $"played {selectedCard.Color} {selectedCard.Value}");
                }

            }
            else
            {
                MessageBox.Show("This card slot is empty.");
            }
        }
        // Handles the turn of a computer player.

        private async void HandleComputerPlayerTurn(Player player)
        {
            // Find a valid card in the computer player's hand.

            Card validCard = FindValidCard(player.Hand, currentCard);
            if (validCard != null)
            {      
                // Execute actions based on the type of the valid card found.

                if (validCard.Value == "draw2")
                {
                    PlayCard(player, validCard);
                    currentCard = validCard;
                    UpdateCurrentCardDisplay(currentCard);
                    UpdateCardDisplay(player);
                    HandleDrawTwoCard(player);
                }
                else if (validCard.Value == "reverse")
                {
                    PlayCard(player, validCard);
                    currentCard = validCard;
                    UpdateCurrentCardDisplay(currentCard);
                    UpdateCardDisplay(player);
                    HandleReverseCard();
                    SwitchToNextPlayer();
                }
                else if (validCard.Value == "skip")
                {
                    PlayCard(player, validCard);
                    currentCard = validCard;
                    UpdateCurrentCardDisplay(currentCard);
                    UpdateCardDisplay(player);
                    HandleSkipCard(player);
                }
                else if (validCard.Color == "wild")
                {
                    string randomColor = GetRandomColor();
                    PlayCard(player, validCard);
                    currentCard = new Card(randomColor, validCard.Value, validCard.ImageName);
                    UpdateCurrentCardDisplay(currentCard);
                    UpdateCardDisplay(player);
                    if (validCard.Value == "draw4")
                    {
                        HandleDrawFourCard(player);
                    }
                    else
                    {
                        SwitchToNextPlayer();
                    }
                }
                else
                {
                    PlayCard(player, validCard);
                    currentCard = validCard;
                    UpdateCurrentCardDisplay(currentCard);
                    UpdateCardDisplay(player);
                    SwitchToNextPlayer();
                }
                // Play the valid card and log the player's activity.

                PlayCard(player, validCard);
                ActivityLogger.LogPlayerActivity(player.Name, $"played {validCard.Color} {validCard.Value}"); 
            }
            else
            {       
                // If no valid card is found, draw a card and decide whether it can be played.

                var drawnCard = deck.Draw();
                player.Hand.Add(drawnCard);
                UpdateCardDisplay(player);
                await Task.Delay(1000);
                if (CanPlayCard(drawnCard, currentCard))
                {
                    if (drawnCard.Value == "draw2")
                    {
                        PlayCard(player, drawnCard);
                        currentCard = drawnCard;
                        UpdateCurrentCardDisplay(currentCard);
                        HandleDrawTwoCard(player);
                    }
                    else if (drawnCard.Value == "reverse")
                    {
                        PlayCard(player, drawnCard);
                        currentCard = drawnCard;
                        UpdateCurrentCardDisplay(currentCard);
                        HandleReverseCard();
                        SwitchToNextPlayer();
                    }
                    else if (drawnCard.Value == "skip")
                    {
                        PlayCard(player, drawnCard);
                        currentCard = drawnCard;
                        UpdateCurrentCardDisplay(currentCard);
                        HandleSkipCard(player);
                    }
                    else if (drawnCard.Color == "wild")
                    {
                        string randomColor = GetRandomColor();
                        PlayCard(player, drawnCard);
                        currentCard = new Card(randomColor, drawnCard.Value, drawnCard.ImageName);
                        UpdateCurrentCardDisplay(currentCard);
                        if (drawnCard.Value == "draw4")
                        {
                            HandleDrawFourCard(player);
                        }
                        else
                        {
                            SwitchToNextPlayer();
                        }
                    }
                    else
                    {
                        PlayCard(player, drawnCard);
                        currentCard = drawnCard;
                        UpdateCurrentCardDisplay(currentCard);
                        SwitchToNextPlayer();
                    }
                }
                else
                {
                    SwitchToNextPlayer();
                }
            }


        }
        // Finds a valid card to play from the given hand based on the current card, it provides abstraction by encapsulating details and exposes only what is necessary,

        private Card FindValidCard(List<Card> hand, Card currentCard)
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
        // plays a card by adding it to the played cards pile, removing it from the player's hand, updating the card display, and checking for a win.

        private void PlayCard(Player player, Card card)
        {
            deck.playedCards.Add(card);
            player.Hand.Remove(card);
            UpdateCardDisplay(player);
            CheckForWin(player);
        }
      
        // checks if a player has won the game by having an empty hand.

        private void CheckForWin(Player player)
        {
            if (player.Hand.Count == 0)
            {
                string winMessage = player.IsHuman ? "Congratulations! You've won the game!" : "A computer player has won the game!";
                MessageBox.Show(winMessage, "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
        }

        // updates the display of cards for a specific player.

        public void UpdateCardDisplay(Player player)
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
                        string imagePath = player.IsHuman ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", player.Hand[i].ImageName) : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "uno_card-back.png");
                        pictureBox.Image = Image.FromFile(imagePath);
                    }
                    else
                    {
                        pictureBox.Image = null;
                    }
                }
            }

            // Update player panel color
            Control playerPanel = Controls.Find($"panelPlayer{playerIndex}", true).FirstOrDefault();
            if (playerPanel != null)
            {
                if (player.Hand.Count == 1)
                {
                    playerPanel.BackColor = Color.Red;
                }
                else
                {
                    playerPanel.BackColor = DefaultBackColor;
                }
            }
        }
        
        // switches the turn to the next player

        public void SwitchToNextPlayer(bool skipNextTurn = false)
        {
            int direction = isPlayDirectionClockwise ? 1 : -1;
            int currentPlayerIndex = players.IndexOf(currentPlayer) + direction;

            if (skipNextTurn)
            {
                currentPlayerIndex += direction;
            }

            currentPlayerIndex = (currentPlayerIndex + players.Count) % players.Count;
            currentPlayer = players[currentPlayerIndex];

            if (!currentPlayer.IsHuman)
            {
                Task.Delay(1000).ContinueWith(t =>
                {
                    if (!IsDisposed)
                    {
                        Invoke(new Action(() => HandleComputerPlayerTurn(currentPlayer)));
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
           
        }

        // checks if the game is over.

        private bool CheckGameOver()
        {
            return false;
        }
        // handles the click event on the "Renew Card" PictureBox.

        private void pictureBoxRenewCard_Click(object sender, EventArgs e)
        {
            if (currentPlayer.IsHuman)
            {
                var drawnCard = deck.Draw();
                if (drawnCard != null)
                {
                    currentPlayer.Hand.Add(drawnCard);
                    UpdateCardDisplay(currentPlayer);
                    if (!CanPlayCard(drawnCard, currentCard))
                    {
                        SwitchToNextPlayer();
                    }
                }
            }
        }

        // Wild and special cards handling
        // Handles the effects of a Draw Two card on the current player and the next player.

        private void HandleDrawTwoCard(Player currentPlayer)
        {
            int direction = isPlayDirectionClockwise ? 1 : -1;
            int currentPlayerIndex = players.IndexOf(currentPlayer);
            int nextPlayerIndex = currentPlayerIndex + direction;

            if (nextPlayerIndex < 0)
            {
                nextPlayerIndex += players.Count;
            }
            else if (nextPlayerIndex >= players.Count)
            {
                nextPlayerIndex %= players.Count;
            }

            Player nextPlayer = players[nextPlayerIndex];

            for (int i = 0; i < 2; i++)
            {
                var drawnCard = deck.Draw();
                if (drawnCard != null)
                {
                    nextPlayer.Hand.Add(drawnCard);
                }
            }

            UpdateCardDisplay(nextPlayer);

            SwitchToNextPlayer(true);
        }
        // Handles the effects of a Draw Four card on the current player and the next player.

        private void HandleDrawFourCard(Player currentPlayer)
        {
            int direction = isPlayDirectionClockwise ? 1 : -1;
            int currentPlayerIndex = players.IndexOf(currentPlayer);
            int nextPlayerIndex = currentPlayerIndex + direction;

            if (nextPlayerIndex < 0)
            {
                nextPlayerIndex += players.Count; 
            }
            else if (nextPlayerIndex >= players.Count)
            {
                nextPlayerIndex %= players.Count; 
            }

            Player nextPlayer = players[nextPlayerIndex];

            for (int i = 0; i < 4; i++)
            {
                var drawnCard = deck.Draw();
                if (drawnCard != null)
                {
                    nextPlayer.Hand.Add(drawnCard);
                }
            }

            UpdateCardDisplay(nextPlayer);
            SwitchToNextPlayer(true);
        }


        // reversing the direction of play.

        private void HandleReverseCard()
        {
            isPlayDirectionClockwise = !isPlayDirectionClockwise;
        }
        // Skips the turn 

        private void HandleSkipCard(Player player)
        {
            SwitchToNextPlayer(true);
        }
        // Opens a form to allow the human player to choose a color for a Wild card, it provides abstraction by encapsulating details and exposes only what is necessary,

        private string ChooseColor()
        {
            Form colorForm = new Form();
            colorForm.Text = "Choose a color";
            colorForm.StartPosition = FormStartPosition.CenterScreen;
            colorForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            colorForm.MaximizeBox = false;
            colorForm.MinimizeBox = false;
            colorForm.AutoSize = true; 

            FlowLayoutPanel panel = new FlowLayoutPanel();
            panel.FlowDirection = FlowDirection.LeftToRight; 
            panel.AutoSize = true;
            //red,green,blue or yellow
            Button redButton = new Button { Text = "Red", BackColor = Color.Red, ForeColor = Color.White, AutoSize = true };
            Button greenButton = new Button { Text = "Green", BackColor = Color.Green, ForeColor = Color.White, AutoSize = true };
            Button blueButton = new Button { Text = "Blue", BackColor = Color.Blue, ForeColor = Color.White, AutoSize = true };
            Button yellowButton = new Button { Text = "Yellow", BackColor = Color.Yellow, ForeColor = Color.Black, AutoSize = true };

            redButton.Click += (sender, e) => { colorForm.DialogResult = DialogResult.OK; colorForm.Tag = "red"; colorForm.Close(); };
            greenButton.Click += (sender, e) => { colorForm.DialogResult = DialogResult.OK; colorForm.Tag = "green"; colorForm.Close(); };
            blueButton.Click += (sender, e) => { colorForm.DialogResult = DialogResult.OK; colorForm.Tag = "blue"; colorForm.Close(); };
            yellowButton.Click += (sender, e) => { colorForm.DialogResult = DialogResult.OK; colorForm.Tag = "yellow"; colorForm.Close(); };

            panel.Controls.Add(redButton);
            panel.Controls.Add(greenButton);
            panel.Controls.Add(blueButton);
            panel.Controls.Add(yellowButton);

            colorForm.Controls.Add(panel);

            colorForm.ShowDialog();

            return colorForm.Tag?.ToString();
        }

        // randomly selects a color for a Wild card which is used for computer player
        private string GetRandomColor()
        {
            string[] colors = { "red", "green", "blue", "yellow" };
            Random random = new Random();
            return colors[random.Next(colors.Length)];
        }

        //changing the backgrounds of the game

        private void woodenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string imagePath = Path.Combine(resourcesFolderPath, "wooden_background.jpg");
            this.BackgroundImage = Image.FromFile(imagePath);
            this.BackgroundImageLayout = ImageLayout.Stretch;
        }

        private void goldenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string imagePath = Path.Combine(resourcesFolderPath, "golden_background.jpg");
            this.BackgroundImage = Image.FromFile(imagePath);
            this.BackgroundImageLayout = ImageLayout.Stretch;
        }

        private void quartzToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string imagePath = Path.Combine(resourcesFolderPath, "quartz_background.jpg");
            this.BackgroundImage = Image.FromFile(imagePath);
            this.BackgroundImageLayout = ImageLayout.Stretch;
        }
        //adding music to the game
        private void onToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string musicFilePath = Path.Combine(resourcesFolderPath, "background_music.wav");
            player.SoundLocation = musicFilePath;
            player.PlayLooping();
        }
        //user can also turn off the music
        private void offToolStripMenuItem_Click(object sender, EventArgs e)
        {
            player.Stop();
        }

       

    }
}