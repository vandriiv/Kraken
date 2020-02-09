using System;
using System.Collections.Generic;
using System.Linq;

namespace Kraken.NormalModesCalculation
{
    public class SDRDRMod
    {
        public int Nsd { get; set; }
        public int Nrd { get; set; }
        public int NR { get; set; }
        public int Ntheta { get; set; }
        public List<int> Isd { get; set; }
        public List<int> Ird { get; set; }
        public List<double> sd { get; set; }
        public List<double> rd { get; set; }
        public List<double> WS { get; set; }
        public List<double> WR { get; set; }
        public List<double> R { get; set; }
        public List<double> theta { get; set; }

        public void SDRD(double ZMIN, double ZMAX, int Nsd, List<double> sd, int Nrd, List<double> rd,List<double> zsr, List<double> zrc)
        {           
            
            sd = Enumerable.Repeat(0d,Math.Max(3+1,Nsd+1)).ToList();
            WS = Enumerable.Repeat(0d, Nsd+1).ToList();
            Isd = Enumerable.Repeat(0, Nsd+1).ToList();

            sd[3] = -999.9;
            var IQ = zsr.Count;
            for(var i=1;i<IQ;i++){
                sd[i] = zsr[i];
            }

            var subTabMod = new SubTabMod();
            subTabMod.SUBTAB(sd,Nsd);

            rd = Enumerable.Repeat(0d, Math.Max(3+1,Nrd+1)).ToList();
            WR = Enumerable.Repeat(0d, Nrd+1).ToList();
            Ird = Enumerable.Repeat(0, Nrd+1).ToList();

            rd[3] = -999.9;
            IQ = zrc.Count;
            for(var i=1;i<IQ;i++){
                rd[i] = zrc[i];
            }

            subTabMod.SUBTAB(rd,Nrd);

            for(var IS=1;IS<=Nsd;IS++){
                if(sd[IS] < ZMIN){
                    sd[IS] = ZMIN;
                }
                else if(sd[IS]>ZMAX){
                    sd[IS] = ZMAX;
                }
            }

            for(var IR=1;IR<=Nrd;IR++){
                if(rd[IR] < ZMIN){
                    rd[IR] = ZMIN;
                }
                else if(rd[IR]>ZMAX){
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
            R = Enumerable.Repeat(0d, Math.Max(3+1,NR+1)).ToList();

            R[3] = -999.9;
            var subTabMod = new SubTabMod();
            subTabMod.SUBTAB(R, NR); //check

            R.Sort();

            R = R.Select(x=>x*1000.0).ToList();
        }
    }
}
