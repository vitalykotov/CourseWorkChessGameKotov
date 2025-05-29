using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ChessGameKotov
{
    public partial class PCandPC : Form, IShowMap
    {
        private Button buttonStartComputerGame;
        private StrategyType whiteStrategy;
        private StrategyType blackStrategy;
        private int player1Wins = 0;
        private int player2Wins = 0;
        private int draws = 0;
        private int gameCounter = 0;
        private int numberOfGames;
        private System.Drawing.Image image;
        private int sumPointComputer1 = 0;
        private int sumPointComputer2 = 0;
        private bool isComputer1Turn;

        public PCandPC()
        {
            InitializeComponent();
            image = System.Drawing.Image.FromFile("..\\..\\PhotoTime.jpg");
            this.DoubleBuffered = true;
            this.Paint += Form1_Paint;

            InitializeComputerGameButton();
            comboBox1.DataSource = strategyMap.Keys.ToList();
            comboBox2.DataSource = strategyMap.Keys.ToList();
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            ShowMap(Global.mapPCandPC);
            label20.Text = "0";
            label21.Text = "0";
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Rectangle destRect = new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height);
            e.Graphics.DrawImage(image, destRect);
        }

        private void InitializeComputerGameButton()
        {
            buttonStartComputerGame = new Button();
            buttonStartComputerGame.Text = "Запустить игру";
            buttonStartComputerGame.Name = "buttonStartComputerGame";
            buttonStartComputerGame.Location = new Point(7, 30);
            buttonStartComputerGame.Height = 40;
            buttonStartComputerGame.Width = 180;
            buttonStartComputerGame.Click += ButtonStartComputerGame_Click;
            Controls.Add(buttonStartComputerGame);

            buttonStartComputerGame.Enabled = true;
            button1.Enabled = false;
        }

        public void ShowMap(Maps map)
        {
            
            if (map == null) return;
            string path = System.IO.Directory.GetCurrentDirectory() + "\\Queens\\";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Button picture = (Button)this.Controls["button" + i + j];
                    if (picture == null) continue;

                    picture.BackgroundImageLayout = ImageLayout.Stretch;
                    Font font = new Font("Tahoma", 20, FontStyle.Bold);
                    picture.Font = font;

                    if (map.cells[i, j].queen != null)
                    {
                        try
                        {
                            string imagePath = Path.Combine(path, "Queen" + map.cells[i, j].queen.color + ".png");
                            if (File.Exists(imagePath))
                            {
                                picture.BackgroundImage = new Bitmap(imagePath);
                            }
                            else
                            {
                                Console.WriteLine($"Image not found: {imagePath}");
                                picture.BackgroundImage = null;
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"Error loading image: {e.Message}");
                            picture.BackgroundImage = null;
                        }
                    }
                    else
                    {
                        picture.BackgroundImage = null;
                    }

                    picture.Text = map.cells[i, j].value.ToString();
                    if (i == 0 && j == 0 || i == 7 && j == 7)
                    {
                        picture.Text = "";
                    }
                    if (map.cells[i, j].isVisited)
                    {
                        picture.BackColor = Color.Red;
                    }
                    else
                    {
                        if ((i + j) % 2 == 0)
                        {
                            picture.BackColor = Color.Beige;
                        }
                        else
                        {
                            picture.BackColor = Color.SaddleBrown;
                        }
                    }
                }
            }
        }

        private Dictionary<string, StrategyType> strategyMap = new Dictionary<string, StrategyType>()
        {
            { "Клетки с максимальной стоимостью", StrategyType.MaxValue },
            { "Забор", StrategyType.Defensive },
            { "Блокировка клеток по наибольшему кол-ву ходов", StrategyType.BlockingMoveByBestCells },
            { "Блокировка максимальных клеток", StrategyType.BlockingMoveByMaxValue }
        };

     
        

        private void MoveImageToButton(Cells targetCell)
        {
           
            if (Global.from != null)
            {
                string oldButtonName = $"button{Global.from.x}{Global.from.y}";
                Button oldButton = (Button)this.Controls[oldButtonName];
                if (oldButton != null)
                {
                    oldButton.BackgroundImage = null;
                }
            }
            string buttonName = $"button{targetCell.x}{targetCell.y}";
            Button targetButton = (Button)this.Controls[buttonName];
            if (targetButton != null)
            {
                string path = System.IO.Directory.GetCurrentDirectory() + "\\Queens\\";
                if (targetCell.queen.color == 0)
                    targetButton.BackgroundImage = new Bitmap(path + "Queen0" + ".png");
                else
                    targetButton.BackgroundImage = new Bitmap(path + "Queen1" + ".png");
            }
        }



        private bool isVisualGame = false;


        private async void ButtonStartComputerGame_Click(object sender, EventArgs e)
        {
            buttonStartComputerGame.Enabled = false;
            button1.Enabled = false;
            radioButton1.Enabled = false;
            radioButton5.Enabled = false;
            string whiteStrategyText = comboBox1.SelectedItem.ToString();
            string blackStrategyText = comboBox2.SelectedItem.ToString();
            comboBox1.Enabled = false;
            comboBox2.Enabled = false;
            if (!string.IsNullOrEmpty(textBox1.Text) && int.TryParse(textBox1.Text, out int games))
            {
                numberOfGames = games;
                if(numberOfGames <= 0)
                {
                    MessageBox.Show("Кол-во игр должно быть больше нуля!", "Ввод неправильный");
                    buttonStartComputerGame.Enabled = true;
                    button1.Enabled = false;
                    radioButton1.Enabled = true;
                    radioButton5.Enabled = true;
                    comboBox1.Enabled = true;
                    comboBox2.Enabled = true;
                    return;
                }
            }
            else
            {
                MessageBox.Show("Вы ввели неправильное значение!", "Ввод неправильный");
                buttonStartComputerGame.Enabled = true;
                button1.Enabled = false;
                radioButton1.Enabled = true;
                radioButton5.Enabled = true;
                comboBox1.Enabled = true;
                comboBox2.Enabled = true;
                return;
               
            }


            whiteStrategy = strategyMap[whiteStrategyText];
            blackStrategy = strategyMap[blackStrategyText];

            int visualGames = 1; 

            player1Wins = 0;
            player2Wins = 0;
            draws = 0;

            for (int i = 0; i < numberOfGames; i++)
            {
                ResetGame();
                isVisualGame = (i < visualGames);
                await StartComputerGame();
            }

            MessageBox.Show($"Все {numberOfGames} игр завершены!\nКомпьютер №1 победил: {player1Wins}\nКомпьютер №2 победил: {player2Wins}\nНичьи: {draws}");

            buttonStartComputerGame.Enabled = false;
            button1.Enabled = true;


        }

        private async Task StartComputerGame()
        {
            isComputer1Turn = true;
            await MakeComputerMove(isComputer1Turn, whiteStrategy, blackStrategy);

        }



        private async Task MakeComputerMove(bool isWhiteTurn, StrategyType whiteStrategy, StrategyType blackStrategy)
        {
            Cells bestMove = null;
            Cells currentQueenPosition = null;
            Cells anotherQueenPosition = null;
            StrategyType currentStrategy = isWhiteTurn ? whiteStrategy : blackStrategy;


            currentQueenPosition = isWhiteTurn ? GetWhiteQueenPosition() : GetBlackQueenPosition();
            anotherQueenPosition = !isWhiteTurn ? GetBlackQueenPosition() : GetWhiteQueenPosition();
            if (currentQueenPosition == null)
                return;


            switch (currentStrategy)
            {
                case StrategyType.MaxValue:
                    bestMove = MaxValueStrategy.ChooseBestMoveMaxValueStrategy(Global.mapPCandPC, currentQueenPosition);
                    break;
                case StrategyType.Defensive:
                    if (isWhiteTurn)
                    {
                        bestMove = DefensiveStrategy.ChooseBestMoveDefensiveWhiteQueen(Global.mapPCandPC, currentQueenPosition);
                    }
                    else
                    {
                        bestMove = DefensiveStrategy.ChooseBestMoveDefensive(Global.mapPCandPC, currentQueenPosition);
                    }
                    break;
                case StrategyType.BlockingMoveByBestCells:
                    bestMove = BlockingMoves.ChooseCellBlockingMovesByCountFreeMoves(Global.mapPCandPC, currentQueenPosition, anotherQueenPosition);
                    break;
                case StrategyType.BlockingMoveByMaxValue:
                    bestMove = BlockingMoves.ChooseCellBlockingMovesByMaxValue(Global.mapPCandPC, currentQueenPosition, anotherQueenPosition);
                    break;
            }

            if (bestMove != null)
            {
                Global.Move(Global.mapPCandPC, currentQueenPosition, bestMove);
                if (isVisualGame)
                {
                    MoveImageToButton(bestMove);
                    ShowMap(Global.mapPCandPC);
                }

                if (isWhiteTurn)
                {
                    sumPointComputer1 += bestMove.value;
                    if (isVisualGame) label20.Text = sumPointComputer1.ToString();
                }
                else
                {
                    sumPointComputer2 += bestMove.value;
                    if (isVisualGame) label21.Text = sumPointComputer2.ToString();
                }

                if (isVisualGame)
                    await Task.Delay(1000); 
            }

            if (!HasPossibleMovesForComputer1(Global.mapPCandPC) && !HasPossibleMovesForComputer2(Global.mapPCandPC))
            {
                gameCounter++;
                if (sumPointComputer1 > sumPointComputer2)
                    player1Wins++;
                else if (sumPointComputer1 < sumPointComputer2)
                    player2Wins++;
                else
                    draws++;
                return;
            }

            await MakeComputerMove(!isWhiteTurn, whiteStrategy, blackStrategy);
        }






        private bool HasPossibleMovesForComputer1(Maps map)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (map.cells[i, j].queen != null && map.cells[i, j].queen.color == 0)
                    {
                        if (Global.GetPossibleMoves(map, map.cells[i, j]).Count > 0)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private bool HasPossibleMovesForComputer2(Maps map)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (map.cells[i, j].queen != null && map.cells[i, j].queen.color == 1)
                    {
                        if (Global.GetPossibleMoves(map, map.cells[i, j]).Count > 0)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }



        private Cells GetBlackQueenPosition()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (Global.mapPCandPC.cells[i, j].queen != null && Global.mapPCandPC.cells[i, j].queen.color == 1)
                    {
                        return Global.mapPCandPC.cells[i, j];
                    }
                }
            }
            return null;
        }

        private Cells GetWhiteQueenPosition()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (Global.mapPCandPC.cells[i, j].queen != null && Global.mapPCandPC.cells[i, j].queen.color == 0)
                    {
                        return Global.mapPCandPC.cells[i, j];
                    }
                }
            }
            return null;
        }

        private void PCandPC_FormClosing(object sender, FormClosingEventArgs e)
        {
            ResetGameAll();
            buttonStartComputerGame.Enabled = true;
            button1.Enabled = false;
           
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ResetGameAll();
            buttonStartComputerGame.Enabled = true;
            button1.Enabled = false;
            radioButton1.Enabled = true;
            radioButton5.Enabled = true;
            comboBox1.Enabled = true;
            comboBox2.Enabled = true;
        }

        private void ResetGameAll()
        {
            sumPointComputer1 = 0;
            sumPointComputer2 = 0;
            player1Wins = 0;
            player2Wins = 0;
            draws = 0;
            gameCounter = 0;
            label20.Text = "0";
            label21.Text = "0";
            isComputer1Turn = true;
            radioButton1.Checked = false;
            radioButton5.Checked = false;
            Global.mapPCandPC = new Maps();
            ShowMap(Global.mapPCandPC);
        }

        private void ResetGame()
        {
            sumPointComputer1 = 0;
            sumPointComputer2 = 0;
            label20.Text = "0";
            label21.Text = "0";
            isComputer1Turn = true;

            Global.mapPCandPC = new Maps();
            if (radioButton1.Checked)
            {
                RandomDistribution.Random(Global.mapPCandPC, this);
            }
            else if (radioButton5.Checked)
            {
                Poisson.GeneratePoissonValues(Global.mapPCandPC, this);
            }
            ShowMap(Global.mapPCandPC);
        }


        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                RandomDistribution.Random(Global.mapPCandPC, this);
               
            }
            else
            {
                Poisson.ClearValues(Global.mapPCandPC, this);
                
            }
        }


        private void rb_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton5.Checked)
            {
                Poisson.GeneratePoissonValues(Global.mapPCandPC, this);
                
            }
            else
            {
                Poisson.ClearValues(Global.mapPCandPC, this);
                
            }
        }

    }
}