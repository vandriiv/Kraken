using System;
using System.Collections.Generic;
using System.Linq;

namespace Kraken.NormalModesCalculation
{
    public class SDRDRMod
    {
        public int Nsx { get; set; }
        public int Nsy { get; set; }
        public int Nsd { get; set; }
        public int Nrd { get; set; }
        public int Nr { get; set; }
        public int Ntheta { get; set; }

        public double Delta_r { get; set; }
        public double Delta_theta { get; set; }
        public List<int> isd { get; set; }
        public List<int> ird { get; set; }
        public List<double> sx { get; set; }
        public List<double> sy { get; set; }
        public List<double> sd { get; set; }
        public List<double> rd { get; set; }
        public List<double> ws { get; set; }
        public List<double> wr { get; set; }
        public List<double> r { get; set; }
        public List<double> theta { get; set; }

        public void SDRD(double ZMIN, double ZMAX, int Nsd, List<double> sd, int Nrd, List<double> rd, List<double> zsr, List<double> zrc)
        {
            if (Nsd <= 0)
            {
                throw new KrakenException("Number of sources must be positive");
            }

            if (Nrd <= 0)
            {
                throw new KrakenException("Number of receivers must be positive");
            }

            sd = Enumerable.Repeat(0d, Math.Max(3 + 1, Nsd + 1)).ToList();

            sd[3] = -999.9;
            var IQ = zsr.Count;
            for (var i = 1; i < IQ; i++)
            {
                sd[i] = zsr[i];
            }

            var subTabMod = new SubTabMod();
            subTabMod.SUBTAB(sd, Nsd);

            rd = Enumerable.Repeat(0d, Math.Max(3 + 1, Nrd + 1)).ToList();

            rd[3] = -999.9;
            IQ = zrc.Count;
            for (var i = 1; i < IQ; i++)
            {
                rd[i] = zrc[i];
            }

            subTabMod.SUBTAB(rd, Nrd);

            for (var IS = 1; IS <= Nsd; IS++)
            {
                if (sd[IS] < ZMIN)
                {
                    sd[IS] = ZMIN;                    
                }
                else if (sd[IS] > ZMAX)
                {
                    sd[IS] = ZMAX;
                }
            }

            for (var IR = 1; IR <= Nrd; IR++)
            {
                if (rd[IR] < ZMIN)
                {
                    rd[IR] = ZMIN;
                }
                else if (rd[IR] > ZMAX)
                {
                    rd[IR] = ZMAX;
                }
            }

            this.sd = sd;
            this.rd = rd;
            this.Nrd = Nrd;
            this.Nsd = Nsd;
        }

        public void RANGES(int NR, List<double> R)
        {
            R = Enumerable.Repeat(0d, Math.Max(3 + 1, NR + 1)).ToList();

            R[3] = -999.9;
            var subTabMod = new SubTabMod();
            subTabMod.SUBTAB(R, NR); //check

            R.Sort();

            R = R.Select(x => x * 1000.0).ToList();
        }

        public void ReadRcvrRanges(int Nr, List<double> givenR)
        {
            var r = Enumerable.Repeat(0d, Math.Max(3, Nr) + 1).ToList();
            r[3] = -999.9;
            var IQ = givenR.Count;
            for (var i = 1; i < IQ; i++)
            {
                r[i] = givenR[i];
            }

            var subtabMod = new SubTabMod();
            subtabMod.SUBTAB(r, Nr);
            r.Sort();

            for (var i = 1; i < r.Count; i++)
            {
                r[i] *= 1000;
            }

            Delta_r = 0;
            if (Nr != 1)
            {
                Delta_r = r[Nr] - r[Nr - 1];
            }

            var isIncreasing = r.OrderBy(x => x).SequenceEqual(r);
            if (!isIncreasing)
            {
                throw new KrakenException("Receiver ranges are not monotonically increasing");
            }

            this.r = r;
        }
    }
}
