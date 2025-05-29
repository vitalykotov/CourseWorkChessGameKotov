using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChessGameKotov
{
    public class Poisson
    {
       
        static double PoissonPMF(int k, double λ)
        {
            return Math.Exp(-λ) * Math.Pow(λ, k) / Factorial(k);
        }

        public static void GeneratePoissonValues(Maps map, IShowMap form1)
        {
            var random = new Random();

            int k = 5;


            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (i == j)
                    {
                        if (i == 0 && j == 0 || i == 7 && j == 7)
                        {
                            map.cells[i, j].value = 0;

                        }
                        else
                        {
                            map.cells[i, j].value = random.Next(1, 6);
                        }

                    }
                    else if (j > i)
                    {
                        double[] probabilities = new double[5];
                        double sumProbabilities = 0;

                        for (int λ = 1; λ <= 5; λ++)
                        {
                            probabilities[λ - 1] = PoissonPMF(k, λ);
                            sumProbabilities += probabilities[λ - 1];
                        }

                        for (int λ = 0; λ < 5; λ++)
                        {
                            probabilities[λ] /= sumProbabilities;
                        }


                        double r = random.NextDouble();
                        double cumulativeProbability = 0;
                        for (int λ = 0; λ < 5; λ++)
                        {
                            cumulativeProbability += probabilities[λ];
                            if (r <= cumulativeProbability)
                            {
                                map.cells[i, j].value = λ + 1;
                                break;
                            }
                        }
                    }
                    else
                    {


                        map.cells[i, j].value = map.cells[j, i].value;

                    }
                }
            }

            form1.ShowMap(map);
        }



       

        static double Factorial(int n)
        {
            if (n <= 1) return 1;
            double result = 1;
            for (int i = 2; i <= n; i++)
            {
                result *= i;
            }
            return result;
        }
        
        public static void ClearValues(Maps map,IShowMap form1)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    map.cells[i, j].value = 0;
                }
            }
            form1.ShowMap(map);
        }

    }
}
