using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Kraken.Calculation.Field
{
    public class PressueFieldCalculator
    {
        public List<List<Complex>> Evaluate(List<Complex> C, List<List<Complex>> phi, int Nz,
            List<double> receiverRanges, int Nr, List<double> rr, List<Complex> k, int modesCount,
            string option)
        {
            var p = new List<List<Complex>>(Nz + 1);
            for (var i = 0; i <= Nz; i++)
            {
                p.Add(Enumerable.Repeat(new Complex(), Nr + 1).ToList());
            }

            if (modesCount <= 0)
            {
                return p;
            }

            var ic = new Complex(0, 1);           
            var factor = ic * Math.Sqrt(2 * Math.PI) * Complex.Exp(ic * Math.PI / 4);

            var cnst = new List<Complex>();
            cnst.Add(new Complex());

            if (option[0] == 'X')
            {
                for (var i = 1; i <= modesCount; i++)
                {
                    cnst.Add(factor * C[i] / k[i]);
                }
            }
            else
            {
                for (var i = 1; i <= modesCount; i++)
                {
                    cnst.Add(factor * C[i] / Complex.Sqrt(k[i]));
                }
            }

            var cMat = new List<List<Complex>>(modesCount + 1);
            for (var i = 0; i <= modesCount; i++)
            {
                cMat.Add(Enumerable.Repeat(new Complex(), Nz + 1).ToList());
            }

            var ik = k.Select(x => x * (-ic)).ToList();
            if (option.Length >= 3)
            {
                if (option[3] == 'I')
                {
                    for (var i = 1; i < ik.Count; i++)
                    {
                        ik[i] = new Complex(ik[i].Real, 0);
                    }
                }

            }

            for (var iz = 1; iz <= Nz; iz++)
            {
                for (var i = 1; i <= modesCount; i++)
                {
                    cMat[i][iz] = cnst[i] * phi[i][iz] * Complex.Exp(ik[i] * rr[iz]);
                }
            }

            for (var ir = 1; ir <= Nr; ir++)
            {
                //problem in exp
                var hank = ik.Select(x => Complex.Exp(x * receiverRanges[ir])).ToList();

                if (option.Length <= 3 || option[3] != 'I')
                {
                    for (var iz = 1; iz <= Nz; iz++)
                    {
                        var sum = new Complex(0, 0);
                        for (var i = 1; i <= modesCount; i++)
                        {
                            sum += cMat[i][iz] * hank[i];
                        }

                        p[iz][ir] = sum;
                    }
                }
                else
                {
                    for (var iz = 1; iz <= Nz; iz++)
                    {
                        var sum = new Complex(0, 0);
                        for (var i = 1; i <= modesCount; i++)
                        {
                            sum += Complex.Pow(cMat[i][iz] * hank[i], 2);
                        }

                        p[iz][ir] = Complex.Sqrt(sum);
                    }
                }

                if (option[0] == 'R')
                {
                    for(var i = 1; i <= Nz; i++)
                    {
                        if(Math.Abs(receiverRanges[ir]+rr[i])> 1.17549435E-38)
                        {
                            p[i][ir] = p[i][ir] / Complex.Sqrt(receiverRanges[ir] + rr[i]);
                        }
                    }                    
                }
            }

            return p;
        }

    }
}
