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
using System.Security;
using System.Threading;
namespace ChessGameKotov
{
    public partial class HumanAndPC : Form,IShowMap
    {
        private CancellationTokenSource cts = new CancellationTokenSource();

        public HumanAndPC()
        {

            InitializeComponent();
            ShowMap(Global.mapHumanandPC);
            label20.Text = "0";
            label21.Text = "0";
            this.FormClosing += (s, e) => cts.Cancel();



        }
        private int sumPointHuman = 0; 
        private int sumPointComputer = 0; 

        

        public void ShowMap(Maps map)
        {
            if (this.IsDisposed || this.Disposing || map == null || map.cells == null)
                return;
            string path = System.IO.Directory.GetCurrentDirectory() + "\\Queens\\";
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Button picture = (Button)this.Controls["button" + i + j];
                    picture.BackgroundImageLayout = ImageLayout.Stretch;
                    Font font = new Font("Tahoma", 20, FontStyle.Bold);
                    picture.Font = font;
                  

                    if (map.cells[i, j].queen != null)
                    {
                        picture.BackgroundImage = new Bitmap(path + "Queen" + map.cells[i, j].queen.color + ".png");
                        
                    }
                    else
                    {
                        picture.BackgroundImage = null;
                    }
                    picture.Text = map.cells[i, j].value.ToString();
                    if (i==0 && j == 0 || i == 7 && j == 7)
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
        private void rb_CheckedChanged(object sender, EventArgs e)
        {

            if (radioButton1.Checked)
            {
                RandomDistribution.Random(Global.mapHumanandPC, this);
            }
            else
            {
                Poisson.ClearValues(Global.mapHumanandPC,this);
            }
        }
        private void MoveImageToButton(Cells targetCell)
        {
            if (Global.from != null)
            {
                string oldButtonName = $"button{Global.from.x}{Global.from.y}";
                Button oldButton = (Button)this.Controls[oldButtonName];
                oldButton.BackgroundImage = null;
            }

            if (targetCell == null || targetCell.queen == null)
            {
                
                return;
            }

            string buttonName = $"button{targetCell.x}{targetCell.y}";
            Button targetButton = (Button)this.Controls[buttonName];

            string path = System.IO.Directory.GetCurrentDirectory() + "\\Queens\\";
            if (targetCell.queen.color == 0)
                targetButton.BackgroundImage = new Bitmap(path + "Queen0" + ".png");
            else
                targetButton.BackgroundImage = new Bitmap(path + "Queen1" + ".png");
        }




        private bool isHumanTurn = true;


        private async void buttonFirstStrategy_Click(object sender, EventArgs e)
        {
            if (!isHumanTurn)
                return;

            string name = ((Button)sender).Name;

            int x = Int32.Parse(name.Substring(6, 1));
            int y = Int32.Parse(name.Substring(7, 1));

            Cells clickedCell = Global.mapHumanandPC.cells[x, y];

            if (Global.from == null)
            {
                
                if (clickedCell.queen != null && clickedCell.queen.color == 0)
                {
                    Global.from = clickedCell;
                    HighlightPossibleMoves(Global.from);
                }
                else
                {
                   
                    Global.from = null;
                    Global.to = null;
                    ResetHighlight();
                    ShowMap(Global.mapHumanandPC);
                }
            }
            else
            {
                Global.to = clickedCell;
                ResetHighlight();

                List<Cells> possibleMoves = Global.GetPossibleMoves(Global.mapHumanandPC, Global.from);

                if (possibleMoves.Contains(Global.to))
                {
                    Global.from.isVisited = true;
                    Global.Move(Global.mapHumanandPC, Global.from, Global.to);

                    if (Global.to.queen != null)
                        MoveImageToButton(Global.to);

                    isHumanTurn = false;

                    sumPointHuman += Global.to.value;
                    label20.Text = sumPointHuman.ToString();

                    Global.from = null;
                    Global.to = null;

                    if (!HasPossibleMovesForHuman(Global.mapHumanandPC) && !HasPossibleMovesForComputer(Global.mapHumanandPC))
                    {
                        ShowMap(Global.mapHumanandPC);
                        ShowResults();
                        return;
                    }
                    else if (!HasPossibleMovesForHuman(Global.mapHumanandPC))
                    {
                        MessageBox.Show("У человека нет доступных ходов, компьютер доигрывает");
                        await PlayOutComputerMoves(cts.Token);
                        ShowMap(Global.mapHumanandPC);
                        return;
                    }
                    else
                    {
                        ShowMap(Global.mapHumanandPC);
                    }

                    await Task.Delay(500);

                    MakeComputerMoveAsync();
                }
                else
                {
                  
                    Global.from = null;
                    Global.to = null;
                    ResetHighlight();
                    ShowMap(Global.mapHumanandPC);
                }
            }
        }


        private async void buttonDefensiveStrategy_Click(object sender, EventArgs e)
        {
            if (!isHumanTurn)
                return;

            string name = ((Button)sender).Name;

            int x = Int32.Parse(name.Substring(6, 1));
            int y = Int32.Parse(name.Substring(7, 1));

            Cells clickedCell = Global.mapHumanandPC.cells[x, y];

            if (Global.from == null)
            {
              
                if (clickedCell.queen != null && clickedCell.queen.color == 0)
                {
                    Global.from = clickedCell;
                    HighlightPossibleMoves(Global.from);
                }
                else
                {
                    
                    Global.from = null;
                    Global.to = null;
                    ResetHighlight();
                    ShowMap(Global.mapHumanandPC);
                }
            }
            else
            {
                Global.to = clickedCell;
                ResetHighlight();

                List<Cells> possibleMoves = Global.GetPossibleMoves(Global.mapHumanandPC, Global.from);

                if (possibleMoves.Contains(Global.to))
                {
                    Global.from.isVisited = true;
                    Global.Move(Global.mapHumanandPC, Global.from, Global.to);

                    if (Global.to.queen != null)
                        MoveImageToButton(Global.to);

                    isHumanTurn = false;

                    sumPointHuman += Global.to.value;
                    label20.Text = sumPointHuman.ToString();

                    Global.from = null;
                    Global.to = null;

                    if (!HasPossibleMovesForHuman(Global.mapHumanandPC) && !HasPossibleMovesForComputer(Global.mapHumanandPC))
                    {
                        ShowMap(Global.mapHumanandPC);
                        ShowResults();
                        return;
                    }
                    else if (!HasPossibleMovesForHuman(Global.mapHumanandPC))
                    {
                        MessageBox.Show("У человека нет доступных ходов, компьютер доигрывает");
                        await PlayOutComputerMovesDefensiveStrategy(cts.Token);
                        ShowMap(Global.mapHumanandPC);
                        return;
                    }
                    else
                    {
                        ShowMap(Global.mapHumanandPC);
                    }

                    await Task.Delay(500);

                    MakeComputerMoveDefensiveStrategy();
                }
                else
                {
                   
                    Global.from = null;
                    Global.to = null;
                    ResetHighlight();
                    ShowMap(Global.mapHumanandPC);
                }
            }
        }

        private async void buttonBlockingMoveMaxValueStrategy_Click(object sender, EventArgs e)
        {
            if (!isHumanTurn)
                return;

            Button clickedButton = (Button)sender;
            string name = clickedButton.Name;
            int x = int.Parse(name.Substring(6, 1));
            int y = int.Parse(name.Substring(7, 1));
            Cells clickedCell = Global.mapHumanandPC.cells[x, y];

            if (Global.from == null)
            {
               
                if (clickedCell.queen != null && clickedCell.queen.color == 0)
                {
                    Global.from = clickedCell;
                    HighlightPossibleMoves(Global.from);
                }
                else
                {
                    
                    Global.from = null;
                    Global.to = null;
                    ResetHighlight();
                    ShowMap(Global.mapHumanandPC);
                }
            }
            else
            {
                Global.to = clickedCell;
                ResetHighlight();

                List<Cells> possibleMoves = Global.GetPossibleMoves(Global.mapHumanandPC, Global.from);

                if (possibleMoves.Contains(Global.to))
                {
                    Global.from.isVisited = true;
                    Global.Move(Global.mapHumanandPC, Global.from, Global.to);

                    if (Global.to.queen != null)
                        MoveImageToButton(Global.to);

                    sumPointHuman += Global.to.value;
                    label20.Text = sumPointHuman.ToString();

                    isHumanTurn = false;
                    Global.from = null;
                    Global.to = null;

                    ShowMap(Global.mapHumanandPC);

                    if (!HasPossibleMovesForHuman(Global.mapHumanandPC) && !HasPossibleMovesForComputer(Global.mapHumanandPC))
                    {
                        ShowResults();
                        return;
                    }
                    else if (!HasPossibleMovesForHuman(Global.mapHumanandPC))
                    {
                        MessageBox.Show("У человека нет доступных ходов, компьютер доигрывает");
                        await PlayOutComputerMovesBlockingMoveMaxValueStrategy(cts.Token);
                        ShowMap(Global.mapHumanandPC);
                        return;
                    }

                    await Task.Delay(500);
                    MakeComputerMoveBlockingMoveMaxValueStrategy();
                }
                else
                {
                   
                    Global.from = null;
                    Global.to = null;
                    ResetHighlight();
                    ShowMap(Global.mapHumanandPC);
                }
            }
        }


        private async void MakeComputerMoveBlockingMoveMaxValueStrategy()
        {
            if (!isHumanTurn)
            {
                Cells bestMove = BlockingMoves.ChooseCellBlockingMovesByMaxValue(Global.mapHumanandPC, GetWhiteQueenPosition(), GetBlackQueenPosition());


                if (bestMove != null)
                {
                    Cells currentBlackQueenPosition = GetBlackQueenPosition();
                    Global.Move(Global.mapHumanandPC, currentBlackQueenPosition, bestMove);
                    ShowMap(Global.mapHumanandPC);

                    sumPointComputer += bestMove.value;
                    label21.Text = sumPointComputer.ToString();
                    isHumanTurn = true;
                    await Task.Delay(500);
                }
                else
                {
                    if (!HasPossibleMovesForHuman(Global.mapHumanandPC))
                    {
                        MessageBox.Show("Игра окончена, ходов больше нет");

                    }
                    isHumanTurn = true;

                }
            }

        }


        private async Task PlayOutComputerMovesBlockingMoveMaxValueStrategy(CancellationToken token)
        {
            while (!isHumanTurn && HasPossibleMovesForComputer(Global.mapHumanandPC))
            {
                if (token.IsCancellationRequested || this.IsDisposed || this.Disposing)
                    return;

                Cells bestMove = BlockingMoves.ChooseCellBlockingMovesByMaxValue(Global.mapHumanandPC, GetWhiteQueenPosition(), GetBlackQueenPosition());

                if (bestMove != null)
                {
                    Cells currentBlackQueenPosition = GetBlackQueenPosition();
                    Global.Move(Global.mapHumanandPC, currentBlackQueenPosition, bestMove);

                    if (token.IsCancellationRequested || this.IsDisposed || this.Disposing)
                        return;

                    ShowMap(Global.mapHumanandPC);
                    sumPointComputer += bestMove.value;
                    label21.Text = sumPointComputer.ToString();

                    try { await Task.Delay(500, token); } catch { return; }
                }
                else
                {
                    if (token.IsCancellationRequested || this.IsDisposed || this.Disposing)
                        return;

                    MessageBox.Show("Компьютер не может ходить. Игра окончена.");
                    ShowMap(Global.mapHumanandPC);
                    break;
                }
            }

            if (token.IsCancellationRequested || this.IsDisposed || this.Disposing)
                return;

            if (!HasPossibleMovesForHuman(Global.mapHumanandPC) && !HasPossibleMovesForComputer(Global.mapHumanandPC))
            {
                MessageBox.Show("Игра окончена, ходов больше нет");
            }

            isHumanTurn = true;
            ShowResults();
        }





        private async void buttonAttackStrategy_Click(object sender, EventArgs e)
        {
            if (!isHumanTurn)
                return;

            string name = ((Button)sender).Name;

            int x = Int32.Parse(name.Substring(6, 1));
            int y = Int32.Parse(name.Substring(7, 1));

            Cells clickedCell = Global.mapHumanandPC.cells[x, y];

            if (Global.from == null)
            {
                
                if (clickedCell.queen != null && clickedCell.queen.color == 0)
                {
                    Global.from = clickedCell;
                    HighlightPossibleMoves(Global.from);
                }
                else
                {
                    
                    Global.from = null;
                    Global.to = null;
                    ResetHighlight();
                    ShowMap(Global.mapHumanandPC);
                }
            }
            else
            {
                Global.to = clickedCell;
                ResetHighlight();

                List<Cells> possibleMoves = Global.GetPossibleMoves(Global.mapHumanandPC, Global.from);

                if (possibleMoves.Contains(Global.to))
                {
                    Global.from.isVisited = true;
                    Global.Move(Global.mapHumanandPC, Global.from, Global.to);

                    if (Global.to.queen != null)
                        MoveImageToButton(Global.to);

                    isHumanTurn = false;

                    sumPointHuman += Global.to.value;
                    label20.Text = sumPointHuman.ToString();

                    Global.from = null;
                    Global.to = null;

                    if (!HasPossibleMovesForHuman(Global.mapHumanandPC) && !HasPossibleMovesForComputer(Global.mapHumanandPC))
                    {
                        ShowMap(Global.mapHumanandPC);
                        ShowResults();
                        return;
                    }
                    else if (!HasPossibleMovesForHuman(Global.mapHumanandPC))
                    {
                        MessageBox.Show("У человека нет доступных ходов, компьютер доигрывает");
                        await PlayOutComputerMovesAttackStrategy(cts.Token);
                        ShowMap(Global.mapHumanandPC);
                        return;
                    }
                    else
                    {
                        ShowMap(Global.mapHumanandPC);
                    }

                    await Task.Delay(500);

                    MakeComputerMoveAttackStrategy();
                }
                else
                {
                    
                    Global.from = null;
                    Global.to = null;
                    ResetHighlight();
                    ShowMap(Global.mapHumanandPC);
                }
            }
        }


        private async void MakeComputerMoveAttackStrategy()
        {
            if (!isHumanTurn)
            {
                Cells bestMove = BlockingMoves.ChooseCellBlockingMovesByCountFreeMoves(Global.mapHumanandPC, GetWhiteQueenPosition(),GetBlackQueenPosition());

                
                if (bestMove != null)
                {
                    Cells currentBlackQueenPosition = GetBlackQueenPosition();
                    Global.Move(Global.mapHumanandPC, currentBlackQueenPosition, bestMove);
                    ShowMap(Global.mapHumanandPC);

                    sumPointComputer += bestMove.value;
                    label21.Text = sumPointComputer.ToString();
                    isHumanTurn = true;
                    await Task.Delay(500);
                }
                else
                {
                    if (!HasPossibleMovesForHuman(Global.mapHumanandPC))
                    {
                        MessageBox.Show("Игра окончена, ходов больше нет");

                    }
                    isHumanTurn = true;

                }
            }

        }


        private async Task PlayOutComputerMovesAttackStrategy(CancellationToken token)
        {
            while (!isHumanTurn && HasPossibleMovesForComputer(Global.mapHumanandPC))
            {
                if (token.IsCancellationRequested || this.IsDisposed || this.Disposing)
                    return;

                Cells bestMove = BlockingMoves.ChooseCellBlockingMovesByCountFreeMoves(
                    Global.mapHumanandPC, GetWhiteQueenPosition(), GetBlackQueenPosition());

                if (bestMove != null)
                {
                    Cells currentBlackQueenPosition = GetBlackQueenPosition();
                    Global.Move(Global.mapHumanandPC, currentBlackQueenPosition, bestMove);

                    if (token.IsCancellationRequested || this.IsDisposed || this.Disposing)
                        return;

                    ShowMap(Global.mapHumanandPC);

                    sumPointComputer += bestMove.value;
                    label21.Text = sumPointComputer.ToString();

                    try { await Task.Delay(500, token); } catch { return; }
                }
                else
                {
                    if (token.IsCancellationRequested || this.IsDisposed || this.Disposing)
                        return;

                    MessageBox.Show("Компьютер не может ходить. Игра окончена.");
                    ShowMap(Global.mapHumanandPC);
                    break;
                }
            }

            if (token.IsCancellationRequested || this.IsDisposed || this.Disposing)
                return;

            if (!HasPossibleMovesForHuman(Global.mapHumanandPC) && !HasPossibleMovesForComputer(Global.mapHumanandPC))
            {
                MessageBox.Show("Игра окончена, ходов больше нет");
            }

            isHumanTurn = true;
            ShowResults();
        }



        private async Task PlayOutComputerMovesDefensiveStrategy(CancellationToken token)
        {
            while (!isHumanTurn && HasPossibleMovesForComputer(Global.mapHumanandPC))
            {
                if (token.IsCancellationRequested || this.IsDisposed || this.Disposing)
                    return;

                Cells bestMove = DefensiveStrategy.ChooseBestMoveDefensive(Global.mapHumanandPC, GetBlackQueenPosition());

                if (bestMove != null)
                {
                    Cells currentBlackQueenPosition = GetBlackQueenPosition();
                    Global.Move(Global.mapHumanandPC, currentBlackQueenPosition, bestMove);

                    if (token.IsCancellationRequested || this.IsDisposed || this.Disposing)
                        return;

                    ShowMap(Global.mapHumanandPC);

                    sumPointComputer += bestMove.value;
                    label21.Text = sumPointComputer.ToString();

                    try { await Task.Delay(500, token); } catch { return; }
                }
                else
                {
                    if (token.IsCancellationRequested || this.IsDisposed || this.Disposing)
                        return;

                    MessageBox.Show("Компьютер не может ходить. Игра окончена.");
                    ShowMap(Global.mapHumanandPC);
                    break;
                }
            }

            if (token.IsCancellationRequested || this.IsDisposed || this.Disposing)
                return;

            if (!HasPossibleMovesForHuman(Global.mapHumanandPC) && !HasPossibleMovesForComputer(Global.mapHumanandPC))
            {
                MessageBox.Show("Игра окончена, ходов больше нет");
            }

            isHumanTurn = true;
            ShowResults();
        }

        private async void MakeComputerMoveDefensiveStrategy()
        {
            if (!isHumanTurn)
            {
                Cells bestMove = DefensiveStrategy.ChooseBestMoveDefensive(Global.mapHumanandPC, GetBlackQueenPosition());
                
                if (bestMove != null)
                {
                    Cells currentBlackQueenPosition = GetBlackQueenPosition();
                    Global.Move(Global.mapHumanandPC, currentBlackQueenPosition, bestMove);
                    ShowMap(Global.mapHumanandPC);

                    sumPointComputer += bestMove.value;
                    label21.Text = sumPointComputer.ToString();
                    isHumanTurn = true;
                    await Task.Delay(500);
                }
                else
                {
                    if (!HasPossibleMovesForHuman(Global.mapHumanandPC))
                    {
                        MessageBox.Show("Игра окончена, ходов больше нет");

                    }
                    isHumanTurn = true;

                }
            }

        }







        private async Task PlayOutComputerMoves(CancellationToken token)
        {
            while (!isHumanTurn && HasPossibleMovesForComputer(Global.mapHumanandPC))
            {
                if (token.IsCancellationRequested || this.IsDisposed || this.Disposing)
                    return;

                Cells bestMove = MaxValueStrategy.ChooseBestMoveMaxValueStrategy(Global.mapHumanandPC, GetBlackQueenPosition());

                if (bestMove != null)
                {
                    Cells currentBlackQueenPosition = GetBlackQueenPosition();
                    Global.Move(Global.mapHumanandPC, currentBlackQueenPosition, bestMove);

                    if (token.IsCancellationRequested || this.IsDisposed || this.Disposing)
                        return;

                    ShowMap(Global.mapHumanandPC);

                    sumPointComputer += bestMove.value;
                    label21.Text = sumPointComputer.ToString();

                    try { await Task.Delay(500, token); } catch { return; }
                }
                else
                {
                    if (token.IsCancellationRequested || this.IsDisposed || this.Disposing)
                        return;

                    MessageBox.Show("Компьютер не может ходить. Игра окончена.");
                    ShowMap(Global.mapHumanandPC);
                    break;
                }
            }

            if (token.IsCancellationRequested || this.IsDisposed || this.Disposing)
                return;

            if (!HasPossibleMovesForHuman(Global.mapHumanandPC) && !HasPossibleMovesForComputer(Global.mapHumanandPC))
            {
                MessageBox.Show("Игра окончена, ходов больше нет");
            }

            isHumanTurn = true;
            ShowResults();
        }

        private async void MakeComputerMoveAsync()
        {
            if (!isHumanTurn)
            {
                Cells bestMove = MaxValueStrategy.ChooseBestMoveMaxValueStrategy(Global.mapHumanandPC, GetBlackQueenPosition());

                if (bestMove != null)
                {
                    Cells currentBlackQueenPosition = GetBlackQueenPosition();
                    Global.Move(Global.mapHumanandPC, currentBlackQueenPosition, bestMove);
                    ShowMap(Global.mapHumanandPC);

                    sumPointComputer += bestMove.value;
                    label21.Text = sumPointComputer.ToString();
                    isHumanTurn = true;
                    await Task.Delay(500);
                }
                else
                {
                    if (!HasPossibleMovesForHuman(Global.mapHumanandPC))
                    {
                        MessageBox.Show("Игра окончена, ходов больше нет");

                    }
                    isHumanTurn = true;

                }
            }

        }




        private bool HasPossibleMovesForHuman(Maps map)
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

        private bool HasPossibleMovesForComputer(Maps map)
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
                    if (Global.mapHumanandPC.cells[i, j].queen != null && Global.mapHumanandPC.cells[i, j].queen.color == 1) 
                    {
                        return Global.mapHumanandPC.cells[i, j];
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
                    if (Global.mapHumanandPC.cells[i, j].queen != null && Global.mapHumanandPC.cells[i, j].queen.color == 0)
                    {
                        return Global.mapHumanandPC.cells[i, j];
                    }
                }
            }

            return null;
        }



        private void ResetHighlight()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    string buttonName = $"button{i}{j}";
                    Button button = (Button)this.Controls[buttonName];

                   
                    if ((i + j) % 2 == 0)
                    {
                        button.BackColor = Color.Beige;
                    }
                    else
                    {
                        button.BackColor = Color.SaddleBrown; 
                    }
                    if (Global.mapHumanandPC.cells[i, j].isVisited)
                    {
                        button.BackColor = Color.Red;
                    }
                }
            }
        }





        private void HighlightPossibleMoves(Cells currentCell)
        {
            List<Cells> possibleMoves = Global.GetPossibleMoves(Global.mapHumanandPC, currentCell);

            foreach (var move in possibleMoves)
            {
               
                string buttonName = $"button{move.x}{move.y}";
                Button button = (Button)this.Controls[buttonName];

              
                button.BackColor = Color.LightBlue;
            }
        }




        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton5.Checked)
            {
                Poisson.GeneratePoissonValues(Global.mapHumanandPC, this);
            }
            else
            {
                Poisson.ClearValues(Global.mapHumanandPC, this);
            }
        }

        private void ShowResults()
        {
            string message;
            if (sumPointHuman > sumPointComputer)
            {
                message = $"Вы выиграли! Счет: {sumPointHuman} : {sumPointComputer}";
            }
            else if (sumPointComputer > sumPointHuman)
            {
                message = $"Выиграл компьютер! Счет: {sumPointComputer} : {sumPointHuman}";
            }
            else
            {
                message = $"Ничья! Счет: {sumPointHuman} : {sumPointComputer}";
            }

            MessageBox.Show(message, "Результаты игры");
        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Button picture = (Button)this.Controls["button" + i + j];
                    picture.Click += buttonFirstStrategy_Click;
                }
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Button picture = (Button)this.Controls["button" + i + j];
                    picture.Click += buttonDefensiveStrategy_Click;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            PCandPC pc = new PCandPC();
            pc.Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            HumanAndHuman gameHumans = new HumanAndHuman();
            gameHumans.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Button picture = (Button)this.Controls["button" + i + j];
                    picture.Click += buttonAttackStrategy_Click;
                }
            }
        }

        private void ResetGame()
        {
            
            sumPointHuman = 0;
            sumPointComputer = 0;
            label20.Text = "0";
            label21.Text = "0";

            
            Global.mapHumanandPC = new Maps(); 
            Global.from = null;
            Global.to = null;
            radioButton1.Checked = false;
            radioButton5.Checked = false;

            isHumanTurn = true;

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Button button = (Button)this.Controls["button" + i + j];
                    button.Click -= buttonFirstStrategy_Click;
                    button.Click -= buttonDefensiveStrategy_Click;
                    button.Click -= buttonAttackStrategy_Click;
                    button.Click -= buttonBlockingMoveMaxValueStrategy_Click;
                }
            }

            ShowMap(Global.mapHumanandPC);
        }


        private void button6_Click(object sender, EventArgs e)
        {
            ResetGame();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Button picture = (Button)this.Controls["button" + i + j];
                    picture.Click += buttonBlockingMoveMaxValueStrategy_Click;
                }
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            string rules = "Игроки по очереди совершают ходы, начинает белый ферзь. " +
                "За каждый ход игроку начисляются очки в соответ\r\nствии со стоимостью клетки." +
                "\r\n В данной игре нельзя съесть ферзя оппонента.\r\n Для каждого игрока игра заканчивается тогда, когда новый ход невозможен. " +
                "Если у одного игрока ходы закончи\r\nлись, а у другого нет, то второй игрок продолжает ходить до тех пор, пока это возможно." +
                "\r\n В конце игры осуществляется подсчёт очков. Победителем становится тот игрок, сумма очков которого больше.";
            MessageBox.Show(rules, "Правила игры");
        }

        private void HumanAndPC_FormClosing(object sender, FormClosingEventArgs e)
        {
            ResetGame();
        }
    }
}
