using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
namespace ChessGameKotov
{
    public class Maps
    {
        public Cells[,] cells = new Cells[8,8];

        public Maps()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    cells[i, j] = new Cells { x = i, y = j };
                }
            }
            cells[0, 0].queen = new Queen { cell = cells[0, 0], color = 0 };
            cells[7, 7].queen = new Queen { cell = cells[7, 7], color = 1 };
        }
    }
}
