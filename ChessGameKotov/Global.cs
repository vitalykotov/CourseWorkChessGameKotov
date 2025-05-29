using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGameKotov
{
    public static class Global
    {
        public static Maps mapPCandPC = new Maps();

        public static Maps mapHumanandPC = new Maps();

        public static Maps mapHumanAndHuman = new Maps();

        public static Cells from = null;

        public static Cells to = null;

        public static void Move(Maps map, Cells fromCell, Cells toCell)
        {
            toCell.queen = fromCell.queen;
            fromCell.queen = null;
            fromCell.isVisited = true;


        }

        public static List<Cells> GetPossibleMoves(Maps map, Cells currentCell)
        {
            List<Cells> possibleMoves = new List<Cells>();
            int x = currentCell.x;
            int y = currentCell.y;

            // Проверка вверх
            for (int i = y + 1; i < 8; i++)
            {
                if (map.cells[x, i].queen != null || map.cells[x, i].isVisited)
                {
                    break;
                }
                possibleMoves.Add(map.cells[x, i]);
            }

            // Проверка вниз
            for (int i = y - 1; i >= 0; i--)
            {
                if (map.cells[x, i].queen != null || map.cells[x, i].isVisited)
                {
                    break;
                }
                possibleMoves.Add(map.cells[x, i]);
            }

            // Проверка вправо
            for (int i = x + 1; i < 8; i++)
            {
                if (map.cells[i, y].queen != null || map.cells[i, y].isVisited)
                {
                    break;
                }
                possibleMoves.Add(map.cells[i, y]);
            }

            // Проверка влево
            for (int i = x - 1; i >= 0; i--)
            {
                if (map.cells[i, y].queen != null || map.cells[i, y].isVisited)
                {
                    break;
                }
                possibleMoves.Add(map.cells[i, y]);
            }

            // Проверка по диагонали вверх-вправо
            int dx = 1;
            int dy = 1;
            while (x + dx < 8 && y + dy < 8)
            {
                if (map.cells[x + dx, y + dy].queen != null || map.cells[x + dx, y + dy].isVisited)
                {
                    break;
                }
                possibleMoves.Add(map.cells[x + dx, y + dy]);
                dx++;
                dy++;
            }

            // Проверка по диагонали вверх-влево
            dx = -1;
            dy = 1;
            while (x + dx >= 0 && y + dy < 8)
            {
                if (map.cells[x + dx, y + dy].queen != null || map.cells[x + dx, y + dy].isVisited)
                {
                    break;
                }
                possibleMoves.Add(map.cells[x + dx, y + dy]);
                dx--;
                dy++;
            }

            // Проверка по диагонали вниз-вправо
            dx = 1;
            dy = -1;
            while (x + dx < 8 && y + dy >= 0)
            {
                if (map.cells[x + dx, y + dy].queen != null || map.cells[x + dx, y + dy].isVisited)
                {
                    break;
                }
                possibleMoves.Add(map.cells[x + dx, y + dy]);
                dx++;
                dy--;
            }

            // Проверка по диагонали вниз-влево
            dx = -1;
            dy = -1;
            while (x + dx >= 0 && y + dy >= 0)
            {
                if (map.cells[x + dx, y + dy].queen != null || map.cells[x + dx, y + dy].isVisited)
                {
                    break;
                }
                possibleMoves.Add(map.cells[x + dx, y + dy]);
                dx--;
                dy--;
            }

            return possibleMoves;
        }



    }
}
