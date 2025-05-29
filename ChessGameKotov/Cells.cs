using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGameKotov
{
    public class Cells
    {
        public int x, y;
        public Queen queen;
        public int value;
        public bool isVisited;
        public Cells()
        {
            isVisited = false;
        }
        public Cells(int x,int y)
        {
            this.x = x;
            this.y = y;
        }
        
    }

}
