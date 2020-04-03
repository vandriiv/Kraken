using System.Collections.Generic;

namespace Kraken.Calculation
{
    class WeightMod
    {
        public void WEIGHT(List<double> x, int Nx, List<double> xTab, int NxTab, List<double> w, List<int> Ix)
        {
            if(Nx==1){
                w[1] = 0;
                Ix[1] = 1;
                return;
            }

            var L = 1;

            for(var IxTab =1; IxTab<=NxTab;IxTab++){
                while( xTab[IxTab] > x[L + 1] && L < Nx - 1 ){
                    L=L+1;
                }

                Ix[IxTab] = L;
                w[IxTab] = ( xTab[IxTab] - x[L] ) / ( x[L+1] - x[L]);
            }
        }
    }
} 
