using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Kraken.NormalModesCalculation
{
    class SinvitdMod
    {
        public void SINVIT(int N, List<double> D, List<double> E,ref int IERR, List<double> EigenVector)
        {
            IERR = 0;
            double NORM = D.Sum(x => Math.Abs(x)) + E.GetRange(2, N - 1).Sum(x => Math.Abs(x));

            double EPS3 = 100 * 2.220446049250313 / Math.Pow(10, 16) * NORM;
            double UK = N;
            double EPS4 = UK * EPS3;
            UK = EPS4 / Math.Sqrt(UK);

            double XU = 1.0;
            double U = D[1];
            double V = E[2];

            var RV1 = Enumerable.Repeat(0d, N + 1).ToList();
            var RV2 = Enumerable.Repeat(0d, N + 1).ToList();
            var RV3 = Enumerable.Repeat(0d, N + 1).ToList();
            var RV4 = Enumerable.Repeat(0d, N + 1).ToList();

            for (var I = 2; I <= N; I++)
            {
                if (I == N)
                {

                }
                if (Math.Abs(E[I]) >= Math.Abs(U))
                {
                    XU = U / E[I];
                    RV4[I] = XU;
                    RV1[I - 1] = E[I];
                    RV2[I - 1] = D[I];
                    RV3[I - 1] = E[I + 1];
                    U = V - XU * RV2[I - 1];
                    V = -XU * RV3[I - 1];
                }
                else
                {
                    XU = E[I] / U;
                    RV4[I] = XU;
                    RV1[I - 1] = U;
                    RV2[I - 1] = V;
                    RV3[I - 1] = 0.0;
                    U = D[I] - XU * V;
                    V = E[I + 1];
                }
            }

            if (U == 0.0)
            {
                U = EPS3;
            }

            RV3[N - 1] = 0.0;
            RV1[N] = U;
            RV2[N] = 0.0;
            RV3[N] = 0.0;

            for (var i = 1; i < EigenVector.Count; i++)
            {
                EigenVector[i] = UK;
            }

            var MAXIT = 5;
            for (var ITER = 1; ITER <= MAXIT; ITER++)
            {

                for (var I = N; I >= 1; I--)
                {
                    EigenVector[I] = (EigenVector[I] - U * RV2[I] - V * RV3[I]) / RV1[I];
                    V = U;
                    U = EigenVector[I];
                }

                NORM = EigenVector.Sum(x => Math.Abs(x));
                if (NORM >= 1)
                {
                    return;
                }


                XU = EPS4 / NORM;
                for (var i = 1; i < EigenVector.Count; i++)
                {
                    EigenVector[i] *= XU;
                }

                for (var I = 2; I <= N; I++)
                {
                    U = EigenVector[I];

                    if (RV1[I - 1] == E[I])
                    {
                        U = EigenVector[I - 1];
                        EigenVector[I - 1] = EigenVector[I];
                    }
                    EigenVector[I] = U - RV4[I] * EigenVector[I - 1];
                }
            }

            IERR = -1;
        }
    }
}
