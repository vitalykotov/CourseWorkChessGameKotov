using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessGameKotov
{
    public enum StrategyType
    {
        MaxValue,
        Defensive,
        BlockingMoveByBestCells,
        BlockingMoveByMaxValue
    }
}
