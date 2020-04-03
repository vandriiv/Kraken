using System.Collections.Generic;
using System.Linq;

namespace Kraken.Calculation
{
    class WeightsCalculator
    {
        public (List<double>,List<int>) CalculateWeightsAndIndices(List<double> x, int Nx, List<double> xTab, int NxTab)
        {
            var w = Enumerable.Repeat(0d, NxTab + 1).ToList();
            var Ix = Enumerable.Repeat(0, NxTab + 1).ToList();

            if (Nx==1){
                w[1] = 0;
                Ix[1] = 1;
                return (w, Ix);
            }

            var L = 1;

            for(var IxTab =1; IxTab<=NxTab;IxTab++){
                while( xTab[IxTab] > x[L + 1] && L < Nx - 1 ){
                    L++;
                }

                Ix[IxTab] = L;
                w[IxTab] = ( xTab[IxTab] - x[L] ) / ( x[L+1] - x[L]);
            }

            return (w, Ix);
        }
    }
} 
