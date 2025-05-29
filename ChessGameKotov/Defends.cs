using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGameKotov
{
    public class DefensiveStrategy
    {
        public static Cells ChooseBestMoveDefensive(Maps map, Cells currentCell)
        {
            int x = currentCell.x;
            int y = currentCell.y;
            List<Cells> possibleMoves = Global.GetPossibleMoves(map, currentCell);

           
            if (x > 4)
            {
                Cells targetCell = map.cells[4, y];
                if (possibleMoves.Contains(targetCell))
                {
                    return targetCell;  
                }
            }
       
            if (x == 4 && y > 0)
            {
                Cells targetCell = map.cells[x, y - 1];
                if (possibleMoves.Contains(targetCell))
                {
                    return targetCell; 
                }
            }
       
            if (x == 4 && y == 0)
            {
               
                for (int i = currentCell.x + 1; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        Cells targetCell = map.cells[i, j];
                        if (!targetCell.isVisited && possibleMoves.Contains(targetCell))
                        {
                            return targetCell;
                        }
                    }
                }
            }

           
            foreach (var move in possibleMoves)
            {
                if (!move.isVisited)
                {
                    return move; 
                }
            }

            return null;
        }

        public static Cells ChooseBestMoveDefensiveWhiteQueen(Maps map, Cells currentCell)
        {
            int x = currentCell.x;
            int y = currentCell.y;
            List<Cells> possibleMoves = Global.GetPossibleMoves(map, currentCell);


            if (x < 3)
            {
                Cells targetCell = map.cells[3, y];
                if (possibleMoves.Contains(targetCell))
                {
                    return targetCell;
                }
            }

            if (x == 3 && y < 7)
            {
                Cells targetCell = map.cells[x, y + 1];
                if (possibleMoves.Contains(targetCell))
                {
                    return targetCell;
                }
            }

            if (x == 3 && y == 7)
            {

                for (int i = currentCell.x + 1; i < 8; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        Cells targetCell = map.cells[i, j];
                        if (!targetCell.isVisited && possibleMoves.Contains(targetCell))
                        {
                            return targetCell;
                        }
                    }
                }
            }


            foreach (var move in possibleMoves)
            {
                if (!move.isVisited)
                {
                    return move;
                }
            }

            return null;
        }




    }
}
