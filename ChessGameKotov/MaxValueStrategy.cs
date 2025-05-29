using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace ChessGameKotov
{
    public class MaxValueStrategy
    {
        public static Cells ChooseBestMoveMaxValueStrategy(Maps map, Cells currentCell)
        {
            List<Cells> possibleMoves = Global.GetPossibleMoves(map, currentCell);
            Cells bestMove = null;
            int bestValue = int.MinValue;

            foreach (var move in possibleMoves)
            {
                int currentValue = move.value; 

                if (currentValue > bestValue)
                {
                    bestValue = currentValue;
                    bestMove = move;
                }
            }

            return bestMove;
        }

    }
}
