using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Kraken.NormalModesCalculation
{
    class TwerskMod
    {
        public Complex TWERSK(string OPT, double OMEGA, double BUMDEN, double XI, double ETA, Complex KX, double RHO0,double C0)
        {
            Complex FS, TWERSK, KZ, COSPHI, SINPHI;
            double K;
            Complex AI = new Complex(0, 1);
            double PI = 3.141592653589793;
            double RADDEG = 180 / PI;
            var ISHS = 0;
            var ISHP = 0;
            if(OPT=="S" || OPT == "T")
            {
                ISHP = 0;
            }
            else
            {
                ISHP = 1;
            }

            K = OMEGA / C0;
            KZ = Complex.Sqrt(K * K - KX * KX);
            SINPHI = KX / K;
            COSPHI = KZ / K;
            var PHI = RADDEG * Math.Atan2(SINPHI.Real, COSPHI.Real);
            FS = FM(PHI, 180- PHI, K, XI, ETA, ISHS, ISHP);

            if(ISHS == 1)
            {
                TWERSK = -RHO0 * C0 * K / (BUMDEN * FS);
            }
            else
            {
                TWERSK = -RHO0 * C0 * BUMDEN * FS / (COSPHI * COSPHI * K);
            }

            return TWERSK;
        }

        private Complex FM(double pHI, double v, double k, double xI, double eTA, int iSHS, int iSHP)
        {
            throw new NotImplementedException();
        }
    }
}
