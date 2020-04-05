using System.Collections.Generic;
using System.Linq;

namespace Kraken.Calculation
{
    class VectorsManager
    {
        public (List<double>, int) MergeVectors(List<double> x, int Nx, List<double> y, int Ny)
        {
            var z = Enumerable.Repeat(0d, Nx + Ny + 1).ToList();

            var ix=1;
            var iy=1;
            var iz=1;

            while(ix<=Nx || iy<=Ny){
                if(iy>Ny){ 
                    z[iz] = x[ix];
                    ix++;
                    iz++;
                }
                else if(ix>Nx){
                    z[iz]=y[iy];
                    iy++;
                    iz++;
                }
                else if(x[ix]<=y[iy]){
                    z[iz] = x[ix];
                    ix++;
                    iz++;
                }
                else{
                    z[iz] = y[iy];
                    iz++;
                    iy++;
                }

                if(iz>2){
                    if(z[iz-1] == z[iz-2]){
                        iz -= 1;
                    }
                }
            }

            var Nz=iz-1;
            return (z, Nz);
        } 
    }
}
