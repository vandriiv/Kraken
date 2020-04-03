using System.Collections.Generic;
using System.Numerics;

namespace Kraken.Calculation
{
    public class SplineCalculator
    {
        public void CalculateCubicSpline(List<double> tau, List<List<Complex>> c, int offset, int N, int IBCBEG, int IBCEND)
        {
            Complex g, dtau, divdf1, divdf3;
            g = new Complex();           

            var l = N - 1;

            for (var m = 2+offset; m <= N+offset; m++)
            {
                c[3][m] = tau[m] - tau[m - 1];
                c[4][m] = (c[1][m] - c[1][m - 1]) / c[3][m];
            }

            if (IBCBEG == 0)
            {
                if (N > 2)
                {
                    c[4][1+offset] = c[3][3 + offset];
                    c[3][1 + offset] = c[3][2 + offset] + c[3][3 + offset];
                    c[2][1 + offset] = ((c[3][2 + offset] + 2.0 * c[3][1 + offset]) * c[4][2 + offset] * c[3][3 + offset] + Complex.Pow(c[3][2 + offset], 2) * c[4][3 + offset]) / c[3][1 + offset];
                }
                else
                {
                    c[4][1 + offset] = new Complex(1.0, 0.0);
                    c[3][1 + offset] = new Complex(1.0, 0.0);
                    c[2][1 + offset] = 2.0 * c[4][2 + offset];
                }
            }
            else if (IBCBEG == 1)
            {
                c[4][1 + offset] = new Complex(1.0, 0.0);
                c[3][1 + offset] = new Complex(0.0, 0.0);
            }
            else if (IBCBEG == 2)
            {
                c[4][1 + offset] = new Complex(2.0, 0.0);
                c[3][1 + offset] = new Complex(0.0, 0.0);
                c[2][1 + offset] = 3.0 * c[4][2 + offset] - c[2][1 + offset] * c[3][2 + offset] / 2.0;
            }

            for (var m = 2+offset; m <= l+offset; m++)
            {
                g = -c[3][m + 1] / c[4][m - 1];
                c[2][m] = g * c[2][m - 1] + 3.0 * (c[3][m] * c[4][m + 1] + c[3][m + 1] * c[4][m]);
                c[4][m] = g * c[3][m - 1] + 2.0 * (c[3][m] + c[3][m + 1]);
            }

            if (IBCEND != 1)
            {
                if (IBCEND == 0)
                {
                    if (N == 2 && IBCBEG == 0)
                    {
                        c[2][N + offset] = c[4][N + offset];//
                    }
                    else if ((N == 3 && IBCBEG == 0) || N == 2)
                    {
                        c[2][N + offset] = 2.0 * c[4][N + offset];
                        c[4][N + offset] = new Complex(1.0, 0.0);
                        g = -1 / c[4][N - 1 + offset];
                    }
                    else
                    {
                        g = c[3][N - 1 + offset] + c[3][N + offset];
                        c[2][N + offset] = ((c[3][N + offset] + 2.0 * g) * c[4][N + offset] * c[3][N - 1 + offset] + Complex.Pow(c[3][N + offset], 2) * (c[1][N - 1 + offset] - c[1][N - 2 + offset]) / c[3][N - 1 + offset]) / g;
                        g = -g/c[4][N-1 + offset];
                        c[4][N + offset] =c[3][N-1 + offset];
                    }
                }
                else if (IBCEND == 2)
                {
                    c[2][N + offset] = 3.0 * c[4][N + offset] + c[2][N + offset] * c[3][N + offset] / 2.0;
                    c[4][N + offset] = new Complex(2.0, 0.0);
                    g = -1.0 / c[4][N - 1 + offset];
                }

                if (IBCBEG > 0 || N > 2)
                {
                    c[4][N + offset] = g * c[3][N - 1 + offset] + c[4][N + offset];
                    c[2][N + offset] = (g * c[2][N - 1 + offset] + c[2][N + offset]) / c[4][N + offset];
                }
            }

            for (var j = l + offset; j >= 1 + offset; j--)
            {
                c[2][j] = (c[2][j] - c[3][j] * c[2][j + 1]) / c[4][j];
            }

            for(var i = 2+offset;i<=N+offset;i++){
                dtau = c[3][i];
                divdf1 = (c[1][i]-c[1][i-1])/dtau;
                divdf3 = c[2][i-1]+c[2][i] - 2.0*divdf1;
                c[3][i-1] = 2.0*(divdf1 - c[2][i-1]-divdf3)/dtau;
                c[4][i-1] = (divdf3/dtau) * (6.0/dtau);
            }

            c[3][N + offset] = c[3][l + offset] + (tau[N + offset] - tau[l + offset]) * c[4][l + offset];

            c[4][N + offset] = new Complex();
            for (var i = 1+offset; i <= l+offset; i++)
            {               
                dtau = tau[i + 1] - tau[i];                
                c[4][N + offset] = c[4][N + offset] + dtau * (c[1][i] + dtau * (c[2][i] / 2.0 +
                            dtau * (c[3][i] / 6.0 + dtau * c[4][i] / 24.0)));
            }
            c[4][N + offset] = c[4][N + offset] / (tau[N + offset] - tau[1+offset]);
        }

        public Complex CalculateExponentialSpline(List<List<Complex>> c,int offset, double h){
            var spline = c[1][offset] + h*(c[2][offset]+h*(c[3][offset]/2 + h*c[4][offset]/6));
            return spline;
        }
    }
}
