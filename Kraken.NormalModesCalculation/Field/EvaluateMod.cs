using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Kraken.NormalModesCalculation.Field
{
    public class EvaluateMod
    {
        public List<List<Complex>> Evaluate(List<Complex> C, List<List<Complex>> phi, int Nz,
            List<double> r, int Nr, List<double> rr, List<Complex> k, int M,
            string Option)
        {
            var P = new List<List<Complex>>(Nz + 1);
            for (var i = 0; i <= Nz; i++)
            {
                P.Add(Enumerable.Repeat(new Complex(), Nr + 1).ToList());
            }

            if (M <= 0)
            {
                return P;
            }

            var ic = new Complex(0, 1);
            var pi = 3.1415926;
            var factor = ic * Math.Sqrt(2 * pi) * Complex.Exp(ic * pi / 4);

            var cnst = new List<Complex>();
            cnst.Add(new Complex());

            if (Option[0] == 'X')
            {
                for (var i = 1; i <= M; i++)
                {
                    cnst.Add(factor * C[i] / k[i]);
                }
            }
            else
            {
                for (var i = 1; i <= M; i++)
                {
                    cnst.Add(factor * C[i] / Complex.Sqrt(k[i]));
                }
            }

            var Cmat = new List<List<Complex>>(M + 1);
            for (var i = 0; i <= M; i++)
            {
                Cmat.Add(Enumerable.Repeat(new Complex(), Nz + 1).ToList());
            }

            var ik = k.Select(x => x * (-ic)).ToList();
            if (Option.Length >= 3)
            {
                if (Option[3] == 'I')
                {
                    for (var i = 1; i < ik.Count; i++)
                    {
                        ik[i] = new Complex(ik[i].Real, 0);
                    }
                }

            }

            for (var iz = 1; iz <= Nz; iz++)
            {
                for (var i = 1; i <= M; i++)
                {
                    Cmat[i][iz] = cnst[i] * phi[i][iz] * Complex.Exp(ik[i] * rr[iz]);
                }
            }

            for (var ir = 1; ir <= Nr; ir++)
            {
                //problem in exp
                var Hank = ik.Select(x => Complex.Exp(x * r[ir])).ToList();

                if (Option.Length <= 3 || Option[3] != 'I')
                {
                    for (var iz = 1; iz <= Nz; iz++)
                    {
                        var sum = new Complex(0, 0);
                        for (var i = 1; i <= M; i++)
                        {
                            sum += Cmat[i][iz] * Hank[i];
                        }

                        P[iz][ir] = sum;
                    }
                }
                else
                {
                    for (var iz = 1; iz <= Nz; iz++)
                    {
                        var sum = new Complex(0, 0);
                        for (var i = 1; i <= M; i++)
                        {
                            sum += Complex.Pow(Cmat[i][iz] * Hank[i], 2);
                        }

                        P[iz][ir] = Complex.Sqrt(sum);
                    }
                }

                if (Option[0] == 'R')
                {
                    var temp = rr.Select(x => Math.Abs(x + r[ir])).ToList();
                    for (var t = 1; t <= Nz; t++)
                    {
                        if (temp[t] > 1.17549435E-38)
                        {
                            for (var i = 1; i <= Nz; i++)
                            {
                                P[i][ir] = P[i][ir] / Complex.Sqrt(r[ir] + rr[i]);
                            }
                        }
                    }
                }

            }

            return P;
        }

    }
}
