using System;
using System.Collections.Generic;
using System.Text;

namespace Kraken.Calculation.Field
{
    public class InterpMod
    {
        public void Interp1(List<double> x, List<double> y, List<double> xi, List<double> yi)
        {
            var N = x.Count;
            var Ni = xi.Count;
            var iseg = 1;
            yi.Add(0);

            for (var I = 1; I <= Ni; I++)
            {
                while (xi[I] > x[iseg + 1])
                {
                    if (iseg < N - 2)
                    {
                        iseg++;
                    }
                }

                while (xi[I] < x[iseg])
                {
                    if (iseg > 1)
                    {
                        iseg--;
                    }
                }

                var R = (xi[I] - x[iseg]) / (x[iseg + 1] - x[iseg]);
                var value = (1.0 - R) * y[iseg] + R * y[iseg + 1];
                yi.Add(value);
            }
        }
    }
}
