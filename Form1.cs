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
        private bool isPlayDirectionClockwise = true;

        public Form1()
        {
            InitializeComponent();
            InitializeGame(); 
            InitializeGameEnvironment();
            DisplayDeckBack();
            pictureBoxRenewCard.Click += pictureBoxRenewCard_Click;
            string backgroundImageFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resources\table.jpg");
            if (File.Exists(backgroundImageFilePath))
            {
                this.BackgroundImage = Image.FromFile(backgroundImageFilePath);
                this.BackgroundImageLayout = ImageLayout.Stretch;
            }
        }

        private void InitializeGameEnvironment()
        {
            foreach (var player in players)
            {
                player.Hand.Clear();
                for (int i = 0; i < CardsPerPlayer; i++)
                {
                    player.DrawCard(deck);
                }
            }

            currentCard = deck.playedCards.FirstOrDefault(c => c.Value != "wild" && c.Value != "draw4" && c.Value != "draw2" && c.Value != "reverse" && c.Value != "skip");
            if (currentCard == null)
            {
                currentCard = deck.playedCards.First();
            }

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
                    pictureBox.Click += PictureBox_Click;
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
            return selectedCard.Value == "wild" || selectedCard.Value == "draw4" ||
                   (selectedCard.Color == currentCard.Color || selectedCard.Value == currentCard.Value);
        }

        private void UpdateCurrentCardDisplay(Card card)
        {
            pictureBoxCurrentCard.Image = Image.FromFile(card.ImageName);
            UpdateButtonCurrentColor(card.Color);
        }

        private void UpdateButtonCurrentColor(string color)
        {
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
            string resourcesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
            for (int i = 0; i < CardsPerPlayer; i++)
            {
                string pictureBoxName = $"pictureBoxPlayer1Card{i + 1}";
                PictureBox pictureBox = Controls.Find(pictureBoxName, true).FirstOrDefault() as PictureBox;
                if (pictureBox != null && i < players[0].Hand.Count)
                {
                    Card card = players[0].Hand[i];
                    string imageName = Path.Combine(resourcesPath, $"uno_card-{card.Color}{card.Value}.png");
                    if (File.Exists(imageName))
                    {
                        using (FileStream stream = new FileStream(imageName, FileMode.Open, FileAccess.Read))
                        {
                            pictureBox.Image = Image.FromStream(stream);
                        }

                    }
                    else
                    {
                        pictureBox.Image = null;
                    }
                }
            }
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
            if (!CheckGameOver())
            {
                Task.Delay(2000).ContinueWith(_ =>
                {
                    int currentPlayerIndex = players.IndexOf(currentPlayer);
                    currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
                    currentPlayer = players[currentPlayerIndex];
                    PlayGame();
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
        }

        private void HandleHumanPlayerTurn()
        {
            for (int i = 0; i < players[0].Hand.Count; i++)
            {
                string pictureBoxName = $"pictureBoxPlayer1Card{i + 1}"; PictureBox pictureBox = Controls.Find(pictureBoxName, true).FirstOrDefault() as PictureBox;
                if (pictureBox != null)
                {
                    pictureBox.Enabled = true;
                    pictureBox.Click += PictureBox_Click;
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
                }
            }
        }

        private async void HandleComputerPlayerTurn(Player player)
        {
            Card validCard = FindValidCard(player.Hand, currentCard);
            if (validCard != null)
            {
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
            }
            else
            {
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

        private void PlayCard(Player player, Card card)
        {
            deck.playedCards.Add(card);
            player.Hand.Remove(card);
            UpdateCardDisplay(player);
            CheckForWin(player);
        }

        private void CheckForWin(Player player)
        {
            if (player.Hand.Count == 0)
            {
                string winMessage = player.IsHuman ? "Congratulations! You've won the game!" : "A computer player has won the game!";
                MessageBox.Show(winMessage, "Game Over", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

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

        private bool CheckGameOver()
        {
            return false;
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
                    if (!CanPlayCard(drawnCard, currentCard))
                    {
                        SwitchToNextPlayer();
                    }
                }
            }
        }

        // Wild and special cards handling

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

        private void HandleReverseCard()
        {
            isPlayDirectionClockwise = !isPlayDirectionClockwise;
        }

        private void HandleSkipCard(Player player)
        {
            SwitchToNextPlayer(true);
        }

        private string ChooseColor()
        {
            Form colorForm = new Form();
            colorForm.Text = "Choose a color";
            colorForm.StartPosition = FormStartPosition.CenterScreen;
            colorForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            colorForm.MaximizeBox = false;
            colorForm.MinimizeBox = false;

            Button redButton = new Button { Text = "Red", BackColor = Color.Red, ForeColor = Color.White };
            Button greenButton = new Button { Text = "Green", BackColor = Color.Green, ForeColor = Color.White };
            Button blueButton = new Button { Text = "Blue", BackColor = Color.Blue, ForeColor = Color.White };
            Button yellowButton = new Button { Text = "Yellow", BackColor = Color.Yellow, ForeColor = Color.Black };

            redButton.Click += (sender, e) => { colorForm.DialogResult = DialogResult.OK; colorForm.Tag = "red"; colorForm.Close(); };
            greenButton.Click += (sender, e) => { colorForm.DialogResult = DialogResult.OK; colorForm.Tag = "green"; colorForm.Close(); };
            blueButton.Click += (sender, e) => { colorForm.DialogResult = DialogResult.OK; colorForm.Tag = "blue"; colorForm.Close(); };
            yellowButton.Click += (sender, e) => { colorForm.DialogResult = DialogResult.OK; colorForm.Tag = "yellow"; colorForm.Close(); };

            colorForm.Controls.Add(redButton);
            colorForm.Controls.Add(greenButton);
            colorForm.Controls.Add(blueButton);
            colorForm.Controls.Add(yellowButton);

            colorForm.ShowDialog();

            return colorForm.Tag?.ToString();
        }

        private string GetRandomColor()
        {
            string[] colors = { "red", "green", "blue", "yellow" };
            Random random = new Random();
            return colors[random.Next(colors.Length)];
        }

        private void DrawPenaltyCards(Player player, int count)
        {
            for (int i = 0; i < count; i++)
            {
                var drawnCard = deck.Draw();
                if (drawnCard != null)
                {
                    player.Hand.Add(drawnCard);
                }
            }

            UpdateCardDisplay(player);
        }

        
    }
}