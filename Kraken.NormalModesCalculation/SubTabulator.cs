using System;
using System.Collections.Generic;

namespace Kraken.Calculation
{
    class SubTabulator
    {
        public void SubTabulate(List<double> x, int Nx)
        {
            if (Nx >= 3)
            {
                if (Math.Abs(x[3] + 999.9) < 0.000001)
                {
                    if (Math.Abs(x[2] + 999.9) < 0.000001)
                    {
                        x[2] = x[1];
                    }
                    var deltaX = (x[2] - x[1]) / (Nx - 1);

                    var temp = x[1];
                    for (var i = 0; i < Nx; i++)
                    {
                        x[i+1] = temp + i * deltaX;
                    }
                }
            }
        }
    }
}