using System;
using System.Numerics;

namespace Kraken.Calculation
{
    class KupingMod
    {
        public Complex KUPING(double sigma, double eta1SQ, 
                              double rho1, double eta2SQ, double rho2,
                              Complex P, Complex U)
        {
            Complex i = new Complex(0.0,1.0);
            Complex KUPING = new Complex(0.0,0.0);
            Complex eta1, eta2,DEL, A11, A12, A21, A22;
            if(sigma == 0.0){
                return KUPING;
            }
            eta1 = SCATRT(eta1SQ);
            eta2 = SCATRT(eta2SQ);
            DEL = rho1*eta2 + rho2*eta1;

            if(DEL!=0){
                A11 = 0.5 * ( eta1SQ - eta2SQ ) - ( rho2 * eta1SQ - rho1 * eta2SQ ) * ( eta1 + eta2 ) / DEL;
                A12 =   i * Math.Pow(( rho2 - rho1 ),2) * eta1 * eta2 / DEL;
                A21 =  -i * Math.Pow(( rho2 * eta1SQ - rho1 * eta2SQ ),2) / ( rho1 * rho2 * DEL );
                A22 = 0.5 * ( eta1SQ - eta2SQ ) + ( rho2 - rho1 ) * eta1 * eta2 * ( eta1 + eta2 ) / DEL;
            
                KUPING = -Math.Pow(sigma,2) * ( -A21 * Complex.Pow(P,2) + ( A11 - A22 ) * P * U + A12 * Complex.Pow(U,2) );
            }

            return KUPING;
        }

        public Complex SCATRT(Complex Z)
        {
            Complex SCATRT;
            if(Z.Real >=0.0){
                SCATRT = Complex.Sqrt(Z);
            }
            else{
                var s = new Complex(0.0,1.0);
                SCATRT = -s * Complex.Sqrt(-s);
            }

            return SCATRT;
        } 
    }
}