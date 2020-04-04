using System;
using System.Numerics;

namespace Kraken.Calculation
{
    class KupermanIngenitoFormulation
    {
        public Complex EvaluateImaginaryPerturbation(double sigma, double eta1Sq, 
                              double rho1, double eta2Sq, double rho2,
                              Complex p, Complex u)
        {
            Complex i = new Complex(0.0,1.0);
            Complex kupIng = new Complex(0.0,0.0);
            Complex eta1, eta2,Del, a11, a12, a21, a22;

            if(sigma == 0.0){
                return kupIng;
            }

            eta1 = ScatterRoot(eta1Sq);
            eta2 = ScatterRoot(eta2Sq);
            Del = rho1*eta2 + rho2*eta1;

            if(Del!=0){
                a11 = 0.5 * ( eta1Sq - eta2Sq ) - ( rho2 * eta1Sq - rho1 * eta2Sq ) * ( eta1 + eta2 ) / Del;
                a12 =   i * Math.Pow(( rho2 - rho1 ),2) * eta1 * eta2 / Del;
                a21 =  -i * Math.Pow(( rho2 * eta1Sq - rho1 * eta2Sq ),2) / ( rho1 * rho2 * Del );
                a22 = 0.5 * ( eta1Sq - eta2Sq ) + ( rho2 - rho1 ) * eta1 * eta2 * ( eta1 + eta2 ) / Del;
            
                kupIng = -Math.Pow(sigma,2) * ( -a21 * Complex.Pow(p,2) + ( a11 - a22 ) * p * u + a12 * Complex.Pow(u,2) );
            }

            return kupIng;
        }

        public Complex ScatterRoot(Complex z)
        {
            Complex scatterRoot;
            if(z.Real >=0.0){
                scatterRoot = Complex.Sqrt(z);
            }
            else{             
                scatterRoot = -new Complex(0.0, 1.0) * Complex.Sqrt(-z);
            }

            return scatterRoot;
        } 
    }
}