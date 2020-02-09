using System.Collections.Generic;

namespace Kraken.NormalModesCalculation
{
    class MergeVMod
    {
        public void MERGEV(List<double> X, int NX, List<double> Y, int NY, List<double> Z, ref int NZ)
        {
            var IX=1;
            var IY=1;
            var IZ=1;

            while(IX<=NX || IY<=NY){
                if(IY>NY){ 
                    Z[IZ] = X[IX];
                    IX++;
                    IZ++;
                }
                else if(IX>NX){
                    Z[IZ]=Y[IY];
                    IY++;
                    IZ++;
                }
                else if(X[IX]<=Y[IY]){
                    Z[IZ] = X[IX];
                    IX++;
                    IZ++;
                }
                else{
                    Z[IZ] = Y[IY];
                    IZ++;
                    IY++;
                }

                if(IZ>2){
                    if(Z[IZ-1] == Z[IZ-2]){
                        IZ = IZ-1;
                    }
                }
            }

            NZ=IZ-1;

        } 
    }
}
