using System;
using System.Collections.Generic;

namespace Kraken.NormalModesCalculation
{
    class SubTabMod
    {
        public void SUBTAB(List<double> x, int Nx)
        {
            if (Nx >= 3)
            {
                if (Math.Abs(x[3] + 999.9) < 0.000001)
                {
                    if (Math.Abs(x[2] + 999.9) < 0.000001)
                    {
                        x[2] = x[1];
                    }
                    var deltax = (x[2] - x[1]) / (Nx - 1);

                    var temp = x[1];
                    for (var i = 0; i < Nx; i++)
                    {
                        x[i+1] = temp + i * deltax;
                    }
                }
            }
        }
    }
}