using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessGameKotov
{
    public class BlockingMoves
    {
        public static Cells ChooseCellBlockingMovesByCountFreeMoves(Maps map, Cells currentCellWhiteQueen, Cells currentCellBlackQueen)
        {
            List<Cells> cellsWhiteQueen = Global.GetPossibleMoves(map, currentCellWhiteQueen); 
            List<Cells> cellsBlackQueen = Global.GetPossibleMoves(map, currentCellBlackQueen); 

            if (cellsWhiteQueen.Count == 0)
            {
                
                foreach (Cells move in cellsBlackQueen)
                {
                    if (!move.isVisited)
                        return move;
                }
                return null;
            }

            
            var cellToMoveCount = new Dictionary<Cells, int>();
            foreach (Cells cell in cellsWhiteQueen)
            {
                cellToMoveCount[cell] = Global.GetPossibleMoves(map, cell).Count;
            }

            
            var uniqueCounts = cellToMoveCount.Values.Distinct().OrderByDescending(x => x).ToList();
            foreach (int maxCount in uniqueCounts)
            {
               
                var maxCells = cellToMoveCount.Where(pair => pair.Value == maxCount)
                                              .Select(pair => pair.Key)
                                              .ToList();

                
                var intersection = maxCells.Intersect(cellsBlackQueen).ToList();

                if (intersection.Count > 0)
                {
                    
                    Random rand = new Random();
                    return intersection[rand.Next(intersection.Count)];
                }
            }

            
            foreach (Cells move in cellsBlackQueen)
            {
                if (!move.isVisited)
                    return move;
            }
            return null;
        }

        public static Cells ChooseCellBlockingMovesByMaxValue(Maps map, Cells currentCellWhiteQueen, Cells currentCellBlackQueen)
        {
            List<Cells> cellsWhiteQueen = Global.GetPossibleMoves(map, currentCellWhiteQueen);
            List<Cells> cellsBlackQueen = Global.GetPossibleMoves(map, currentCellBlackQueen);

            if (cellsWhiteQueen.Count == 0)
            {

                foreach (Cells move in cellsBlackQueen)
                {
                    if (!move.isVisited)
                        return move;
                }
                return null;
            }


            var uniqueValues = cellsWhiteQueen
                .Select(c => c.value)
                .Distinct()
                .OrderByDescending(v => v)
                .ToList();

            foreach (int value in uniqueValues)
            {

                var maxValueCells = cellsWhiteQueen.Where(c => c.value == value).ToList();


                var intersection = maxValueCells.Intersect(cellsBlackQueen).ToList();

                if (intersection.Count > 0)
                {

                    Random rand = new Random();
                    return intersection[rand.Next(intersection.Count)];
                }
            }


            foreach (Cells move in cellsBlackQueen)
            {
                if (!move.isVisited)
                    return move;
            }
            return null;
        }
    }
}


