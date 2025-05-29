using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessGameKotov
{
    public class RandomDistribution
    {
        public static void Random(Maps map, IShowMap form)

        {
            Random random = new Random();
            if (map == null) return;


            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {

                    if (map.cells[i, j].queen == null)
                    {
                        if (i == j)
                        {
                            map.cells[i, j].value = random.Next(1, 6);
                            
                        }
                        else if (j > i)
                        {
                            int value = random.Next(1, 6);
                            map.cells[i, j].value = value;  
                            map.cells[j, i].value = value;  
                        }


                    }

                    else
                    {
                        map.cells[i, j].value = 0;
                    }
                }
            }
            form.ShowMap(map);
        }
    }
}
