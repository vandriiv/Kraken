using System;
using System.Collections.Generic;
using System.Linq;

namespace Kraken.Calculation
{
    class EigenvectorFinder
    {
        public void FindEigenvectorUsingInverseIteration(int matrixOrder, List<double> mainDiagonal, List<double> subDiagonal, List<double> eigenvector, ref int errorFlag)
        {
            var maxIteration = 2500;
            errorFlag = 0;
            double norm = mainDiagonal.Sum(x => Math.Abs(x)) + subDiagonal.GetRange(2, matrixOrder - 1).Sum(x => Math.Abs(x));

            double eps3 = 100 * 2.220446049250313 / Math.Pow(10, 16) * norm;
            double uk = matrixOrder;
            double eps4 = uk * eps3;
            uk = eps4 / Math.Sqrt(uk);

            double xu = 1.0;
            double u = mainDiagonal[1];
            double v = subDiagonal[2];

            var rv1 = Enumerable.Repeat(0d, matrixOrder + 1).ToList();
            var rv2 = Enumerable.Repeat(0d, matrixOrder + 1).ToList();
            var rv3 = Enumerable.Repeat(0d, matrixOrder + 1).ToList();
            var rv4 = Enumerable.Repeat(0d, matrixOrder + 1).ToList();

            for (var I = 2; I <= matrixOrder; I++)
            {                
                if (Math.Abs(subDiagonal[I]) >= Math.Abs(u))
                {
                    xu = u / subDiagonal[I];
                    rv4[I] = xu;
                    rv1[I - 1] = subDiagonal[I];
                    rv2[I - 1] = mainDiagonal[I];
                    rv3[I - 1] = subDiagonal[I + 1];
                    u = v - xu * rv2[I - 1];
                    v = -xu * rv3[I - 1];
                }
                else
                {
                    xu = subDiagonal[I] / u;
                    rv4[I] = xu;
                    rv1[I - 1] = u;
                    rv2[I - 1] = v;
                    rv3[I - 1] = 0.0;
                    u = mainDiagonal[I] - xu * v;
                    v = subDiagonal[I + 1];
                }
            }

            if (u == 0.0)
            {
                u = eps3;
            }

            rv3[matrixOrder - 1] = 0.0;
            rv1[matrixOrder] = u;
            rv2[matrixOrder] = 0.0;
            rv3[matrixOrder] = 0.0;

            for (var i = 1; i < eigenvector.Count; i++)
            {
                eigenvector[i] = uk;
            }
            
            for (var iteration = 1; iteration <= maxIteration; iteration++)
            {

                for (var I = matrixOrder; I >= 1; I--)
                {
                    eigenvector[I] = (eigenvector[I] - u * rv2[I] - v * rv3[I]) / rv1[I];
                    v = u;
                    u = eigenvector[I];
                }

                norm = eigenvector.Sum(x => Math.Abs(x));
                if (norm >= 1)
                {
                    return;
                }

                xu = eps4 / norm;
                for (var i = 1; i < eigenvector.Count; i++)
                {
                    eigenvector[i] *= xu;
                }

                for (var I = 2; I <= matrixOrder; I++)
                {
                    u = eigenvector[I];

                    if (rv1[I - 1] == subDiagonal[I])
                    {
                        u = eigenvector[I - 1];
                        eigenvector[I - 1] = eigenvector[I];
                    }
                    eigenvector[I] = u - rv4[I] * eigenvector[I - 1];
                }
            }

            errorFlag = -1;
        }
    }
}
