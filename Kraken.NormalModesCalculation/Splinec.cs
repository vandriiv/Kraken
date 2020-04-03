using System;
using System.Collections.Generic;
using System.Numerics;

namespace Kraken.Calculation
{
    public class Splinec
    {
        public void CSPLINE(List<double> TAU, List<List<Complex>> C, int i, int N, int IBCBEG, int IBCEND, int NDIM)
        {
            Complex G, DTAU, DIVDF1, DIVDF3;
            G = new Complex();
            /*DTAU = new Complex();
            DIVDF1 = new Complex();
            DIVDF3 = new Complex();*/

            var L = N - 1;

            for (var M = 2+i; M <= N+i; M++)
            {
                C[3][M] = TAU[M] - TAU[M - 1];
                C[4][M] = (C[1][M] - C[1][M - 1]) / C[3][M];
            }

            if (IBCBEG == 0)
            {
                if (N > 2)
                {
                    C[4][1+i] = C[3][3 + i];
                    C[3][1 + i] = C[3][2 + i] + C[3][3 + i];
                    C[2][1 + i] = ((C[3][2 + i] + 2.0 * C[3][1 + i]) * C[4][2 + i] * C[3][3 + i] + Complex.Pow(C[3][2 + i], 2) * C[4][3 + i]) / C[3][1 + i];
                }
                else
                {
                    C[4][1 + i] = new Complex(1.0, 0.0);
                    C[3][1 + i] = new Complex(1.0, 0.0);
                    C[2][1 + i] = 2.0 * C[4][2 + i];
                }
            }
            else if (IBCBEG == 1)
            {
                C[4][1 + i] = new Complex(1.0, 0.0);
                C[3][1 + i] = new Complex(0.0, 0.0);
            }
            else if (IBCBEG == 2)
            {
                C[4][1 + i] = new Complex(2.0, 0.0);
                C[3][1 + i] = new Complex(0.0, 0.0);
                C[2][1 + i] = 3.0 * C[4][2 + i] - C[2][1 + i] * C[3][2 + i] / 2.0;
            }

            for (var M = 2+i; M <= L+i; M++)
            {
                G = -C[3][M + 1] / C[4][M - 1];
                C[2][M] = G * C[2][M - 1] + 3.0 * (C[3][M] * C[4][M + 1] + C[3][M + 1] * C[4][M]);
                C[4][M] = G * C[3][M - 1] + 2.0 * (C[3][M] + C[3][M + 1]);
            }

            if (IBCEND != 1)
            {
                if (IBCEND == 0)
                {
                    if (N == 2 && IBCBEG == 0)
                    {
                        C[2][N + i] = C[4][N + i];//
                    }
                    else if ((N == 3 && IBCBEG == 0) || N == 2)
                    {
                        C[2][N + i] = 2.0 * C[4][N + i];
                        C[4][N + i] = new Complex(1.0, 0.0);
                        G = -1 / C[4][N - 1 + i];
                    }
                    else
                    {
                        G = C[3][N - 1 + i] + C[3][N + i];
                        C[2][N + i] = ((C[3][N + i] + 2.0 * G) * C[4][N + i] * C[3][N - 1 + i] + Complex.Pow(C[3][N + i], 2) * (C[1][N - 1 + i] - C[1][N - 2 + i]) / C[3][N - 1 + i]) / G;
                        G = -G/C[4][N-1 + i];
                        C[4][N + i] =C[3][N-1 + i];
                    }
                }
                else if (IBCEND == 2)
                {
                    C[2][N + i] = 3.0 * C[4][N + i] + C[2][N + i] * C[3][N + i] / 2.0;
                    C[4][N + i] = new Complex(2.0, 0.0);
                    G = -1.0 / C[4][N - 1 + i];
                }

                if (IBCBEG > 0 || N > 2)
                {
                    C[4][N + i] = G * C[3][N - 1 + i] + C[4][N + i];
                    C[2][N + i] = (G * C[2][N - 1 + i] + C[2][N + i]) / C[4][N + i];
                }
            }

            for (var j = L + i; j >= 1 + i; j--)
            {
                C[2][j] = (C[2][j] - C[3][j] * C[2][j + 1]) / C[4][j];
            }

            for(var I = 2+i;I<=N+i;I++){
                DTAU = C[3][I];
                DIVDF1 = (C[1][I]-C[1][I-1])/DTAU;
                DIVDF3 = C[2][I-1]+C[2][I] - 2.0*DIVDF1;
                C[3][I-1] = 2.0*(DIVDF1 - C[2][I-1]-DIVDF3)/DTAU;
                C[4][I-1] = (DIVDF3/DTAU) * (6.0/DTAU);
            }

            C[3][N + i] = C[3][L + i] + (TAU[N + i] - TAU[L + i]) * C[4][L + i];

            C[4][N + i] = new Complex();
            for (var I = 1+i; I <= L+i; I++)
            {               
                DTAU = TAU[I + 1] - TAU[I];                
                C[4][N + i] = C[4][N + i] + DTAU * (C[1][I] + DTAU * (C[2][I] / 2.0 +
                            DTAU * (C[3][I] / 6.0 + DTAU * C[4][I] / 24.0)));
            }
            C[4][N + i] = C[4][N + i] / (TAU[N + i] - TAU[1+i]);
        }

        public Complex ESPLINE(List<List<Complex>> C,int i, double H){
            var ESPLINE = C[1][i] + H*(C[2][i]+H*(C[3][i]/2 + H*C[4][i]/6));
            return ESPLINE;
        }
    }
}
