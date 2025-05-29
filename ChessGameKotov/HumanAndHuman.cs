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
    public partial class HumanAndHuman : Form,IShowMap
    {
        private bool isPlayer1Turn = true; 
        private Cells selectedCell = null;
        private int player1Score;
        private int player2Score;
      
        public HumanAndHuman()
        {
            InitializeComponent();
            
            ShowMap(Global.mapHumanAndHuman);
            LoadImageInBackground("..\\..\\PhotoZakat.jpg");

        }
        private void LoadImageInBackground(string path)
        {
            Task.Run(() =>
            {
               
                Image img;
                try
                {
                    img = Image.FromFile(path);
                }
                catch (Exception)
                {
                 
                    return;
                }

                
                this.Invoke((Action)(() =>
                {
                    this.BackgroundImage = img;
                    this.BackgroundImageLayout = ImageLayout.Stretch;
                }));
            });
        }



        public void ShowMap(Maps map)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Queens");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Button button = (Button)this.Controls["button" + i + j];
                    if (button == null) continue;

                    button.BackgroundImageLayout = ImageLayout.Stretch;
                    button.Font = new Font("Tahoma", 20, FontStyle.Bold);

                    if (map.cells[i, j].queen != null)
                    {
                        string imagePath = Path.Combine(path, "Queen" + map.cells[i, j].queen.color + ".png");
                        if (File.Exists(imagePath))
                        {
                            button.BackgroundImage = new Bitmap(imagePath);
                        }
                        else
                        {
                            button.BackgroundImage = null;
                        }
                    }
                    else
                    {
                        button.BackgroundImage = null;
                    }

                    button.Text = map.cells[i, j].value.ToString();
                    if (i == 0 && j == 0 || i == 7 && j == 7)
                    {
                        button.Text = "";
                    }

                    if (map.cells[i, j].isVisited)
                    {
                        button.BackColor = Color.Red;
                    }
                    else
                    {
                        if ((i + j) % 2 == 0)
                        {
                            button.BackColor = Color.Beige;
                        }
                        else
                        {
                            button.BackColor = Color.SaddleBrown;
                        }
                    }
                }
            }
        }

        private void HighlightPossibleMoves(Cells cell)
        {
            ResetHighlight();

            List<Cells> possibleMoves = Global.GetPossibleMoves(Global.mapHumanAndHuman, cell);
            foreach (Cells move in possibleMoves)
            {
                string buttonName = $"button{move.x}{move.y}";
                Button button = (Button)this.Controls[buttonName];
                if (button != null)
                {
                    button.BackColor = Color.LightBlue;
                }
            }
        }

        private void ResetHighlight()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    string buttonName = $"button{i}{j}";
                    Button button = (Button)this.Controls[buttonName];
                    
                    if (button != null)
                    {                  
                        if ((i + j) % 2 == 0)
                        {
                            button.BackColor = Color.Beige;
                        }
                        else
                        {
                            button.BackColor = Color.SaddleBrown;
                        }
                        if (Global.mapHumanAndHuman.cells[i, j].isVisited)
                        {
                            button.BackColor = Color.Red;
                        }
                    }
                }
            }
        }

        private void SwitchTurn()
        {
            isPlayer1Turn = !isPlayer1Turn;
            selectedCell = null; 
            
                             

        }
        private void button_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            string name = clickedButton.Name;
            int x = int.Parse(name.Substring(6, 1));
            int y = int.Parse(name.Substring(7, 1));
            Cells clickedCell = Global.mapHumanAndHuman.cells[x, y];

            if (selectedCell == null)
            {
                if (clickedCell.queen != null && ((isPlayer1Turn && clickedCell.queen.color == 0) || (!isPlayer1Turn && clickedCell.queen.color == 1)))
                {
                    selectedCell = clickedCell;
                    HighlightPossibleMoves(selectedCell);
                }
                else
                {
                    
                    selectedCell = null;
                    ResetHighlight();
                    ShowMap(Global.mapHumanAndHuman); 
                }
            }
            else
            {
                List<Cells> possibleMoves = Global.GetPossibleMoves(Global.mapHumanAndHuman, selectedCell);
                if (possibleMoves.Contains(clickedCell))
                {
                    Global.Move(Global.mapHumanAndHuman, selectedCell, clickedCell);
                    clickedCell.isVisited = true;

                    if (isPlayer1Turn)
                    {
                        player1Score += Global.mapHumanAndHuman.cells[clickedCell.x, clickedCell.y].value;
                    }
                    else
                    {
                        player2Score += Global.mapHumanAndHuman.cells[clickedCell.x, clickedCell.y].value;
                    }
                    label20.Text = player1Score.ToString();
                    label21.Text = player2Score.ToString();

                    ShowMap(Global.mapHumanAndHuman);

                    SwitchTurn();
                    if (!HasPossibleMoves())
                    {
                        SwitchTurn();
                        if (!HasPossibleMoves())
                        {
                            MessageBox.Show("У обоих игроков нет ходов! Игра окончена!");
                            ShowResults();
                        }
                    }

                    selectedCell = null;  
                    ResetHighlight();
                }
                else
                {
                  
                    selectedCell = null;
                    ResetHighlight();
                    ShowMap(Global.mapHumanAndHuman);
                }
            }
        }

        private bool HasPossibleMoves()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Cells cell = Global.mapHumanAndHuman.cells[i, j];
                    if (cell.queen != null && ((isPlayer1Turn && cell.queen.color == 0) || (!isPlayer1Turn && cell.queen.color == 1)))
                    {
                        List<Cells> possibleMoves = Global.GetPossibleMoves(Global.mapHumanAndHuman, cell);
                        if (possibleMoves.Count > 0)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Button picture = (Button)this.Controls["button" + i + j];
                    picture.Click += button_Click;
                }
            }
        }


        private void ShowResults()
        {
            string message;
            if (player1Score > player2Score)
            {
                message = $"Выиграл белый ферзь! Счет: {player1Score} : {player2Score}";
            }
            else if (player2Score > player1Score)
            {
                message = $"Выиграл чёрный ферзь! Счет: {player2Score} : {player1Score}";
            }
            else
            {
                message = $"Ничья! Счет: {player1Score} : {player2Score}";
            }

            MessageBox.Show(message, "Результаты игры");
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {
                RandomDistribution.Random(Global.mapHumanAndHuman, this);

            }
            else
            {
                Poisson.ClearValues(Global.mapHumanAndHuman, this);
            }
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton5.Checked)
            {
                Poisson.GeneratePoissonValues(Global.mapHumanAndHuman, this);
            }
            else
            {
                Poisson.ClearValues(Global.mapHumanAndHuman, this);
            }
        }

        private void ResetGame()
        {

            player1Score = 0;
            player2Score = 0;
            label20.Text = "0";
            label21.Text = "0";


            Global.mapHumanAndHuman = new Maps();
            Global.from = null;
            Global.to = null;
            radioButton1.Checked = false;
            radioButton5.Checked = false;

            isPlayer1Turn = true;

            

            ShowMap(Global.mapHumanAndHuman);
        }


        private void button2_Click(object sender, EventArgs e)
        {
            ResetGame();

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string rules = "Игроки по очереди совершают ходы, начинает белый ферзь. " +
                "За каждый ход игроку начисляются очки в соответ\r\nствии со стоимостью клетки." +
                "\r\n В данной игре нельзя съесть ферзя оппонента.\r\n Для каждого игрока игра заканчивается тогда, когда новый ход невозможен. " +
                "Если у одного игрока ходы закончи\r\nлись, а у другого нет, то второй игрок продолжает ходить до тех пор, пока это возможно." +
                "\r\n В конце игры осуществляется подсчёт очков. Победителем становится тот игрок, сумма очков которого больше.";
            MessageBox.Show(rules, "Правила игры");
        }

        private void HumanAndHuman_FormClosing(object sender, FormClosingEventArgs e)
        {
            ResetGame();
        }
    }
}
