using Kraken.Application.Models;
using Kraken.Calculation.Models;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Kraken.Application.Tests.Unit
{
    public class TestDataHelper
    {
        public AcousticProblemData GetAcousticProblemDataForKraken()
        {
            var data = GetAcousticProblemDataTestData();

            return data;
        }

        public KrakenInputProfile GetKrakenInputProfile()
        {
            var nModes = 44;
            var frequency = 10.0;
            var nMedia = 2;
            var options = "RTR";
            var bcBottom = "R";
            var mediumInfo = new List<List<double>>();
            var ssp = new List<List<double>>();
            var bsigma = 0.0;
            var cLow = 0;
            var cHigh = 0;
            var rMax = 100;
            var nsd = 1;
            var sDepths = new List<double>();
            var nrd = 1;
            var rDepths = new List<double>();
            var tahsp = new List<double>();
            var tsp = new List<double>();
            var bahsp = new List<double>();

            var krakenInputProfile = new KrakenInputProfile(nModes, frequency, nMedia,
                options, bcBottom, mediumInfo, ssp, bsigma, cLow, cHigh, rMax,
                nsd, sDepths, nrd, rDepths, tahsp, tsp, bahsp);

            return krakenInputProfile;
        }

        public FieldInputData GetFieldInputData()
        {
            var modesInfo = GetCalculatedModesInfo();
            var options = "RA";
            var mLimit = 999;
            var nr = 1;
            var rr = new List<double>();
            var nsd = 1;
            var sDepths = new List<double>();
            var nrd = 1;
            var rDepths = new List<double>();
            var nrr = 1;
            var rd = new List<double>();

            var fieldInputData = new FieldInputData(modesInfo, options,
                mLimit, nr, rr, nsd, sDepths, nrd, rDepths, nrr, rd);

            return fieldInputData;
        }

        public AcousticProblemData GetAcousticProblemDataForKrakenAndField()
        {
            var data = GetAcousticProblemDataForKraken();

            data.CalculateTransmissionLoss = true;

            return data;
        }

        public KrakenResult GetKrakenResult()
        {
            var data = new KrakenResult();
            data.ModesCount = 44;
            data.GroupSpeed.AddRange(Enumerable.Repeat(0d, 45));
            data.PhaseSpeed.AddRange(Enumerable.Repeat(0d, 45));
            data.K.AddRange(Enumerable.Repeat(new Complex(), 45));
            data.Modes.AddRange(new List<List<double>> { new List<double> { 0 },
                                                  new List<double> {0} });
            data.ZM.AddRange(new List<double> { 0, 0, 0 });            

            return data;
        }

        public CalculatedModesInfo GetCalculatedModesInfo()
        {
            var data = new CalculatedModesInfo();

            data.BCBottom = "A";
            data.BCTop = "V";
            data.CPBottom = new Complex(2000, 0);
            data.CPTop = 0;
            data.CSBottom = 0;
            data.CSTop = 0;
            data.Depth.AddRange(Enumerable.Repeat(0d, 501));
            data.DepthBottom = 5000;
            data.DepthTop = 0;
            data.Frequency = 10;
            data.K.AddRange(Enumerable.Repeat(new Complex(), 45));
            data.Material.AddRange(Enumerable.Repeat("", 501));
            data.ModesCount = 44;
            data.N.AddRange(Enumerable.Repeat(0, 501));
            data.NMat = 2;
            data.NMedia = 1;
            data.NTot = 2;
            data.Phi = new List<List<Complex>>() { new List<Complex> { 0, 0 } };
            data.RhoBottom = 2;
            data.RhoTop = 0;
            data.Z.AddRange(Enumerable.Repeat(0d, 3));

            return data;
        }

        public AcousticFieldSnapshots GetAcousticFieldSnapshots()
        {
            var data = new AcousticFieldSnapshots();

            data.Ranges.AddRange(new List<double> { 0, 0 });
            data.ReceiverDepths.AddRange(new List<double> { 0, 0 });
            data.SourceDepths.AddRange(new List<double> { 0, 0 });
            data.Snapshots.AddRange(new List<List<List<Complex>>> { new List<List<Complex>> { },
            new List<List<Complex>>{ new List<Complex> { 0,0 }, new List<Complex> { 0,0 } }});            

            return data;
        }

        private AcousticProblemData GetAcousticProblemDataTestData()
        {
            var data = new AcousticProblemData
            {
                APB = 0,
                APT = 0,
                ASB = 0,
                AST = 0,
                AddedVolumeAttenuation = "",
                AttenuationUnits = "F",
                BottomBCType = "A",
                BumDen = 0,
                CHigh = 2000,
                CLow = 1400,
                CPB = 2000,
                CPT = 0,
                CSB = 0,
                CST = 0,
                CalculateTransmissionLoss = false,
                Eta = 0,
                Frequency = 10,
                InterpolationType = "N",             
                ModesTheory = "A",
                NMedia = 1,
                NModes = 44,
                NModesForField = 9999,
                NR = 1,
                NRDField = 1,
                NRR = 1,
                NSD = 1,
                NSDField = 1,               
                RHOB = 2,
                RHOT = 0,
                RMax = 1000,                
                Sigma = 0,
                SourceType = "R",
                TopBCType = "V",
                Xi = 0,
                ZB = 5000,
                ZT = 0
            };

            data.MediumInfo.AddRange(new List<List<double>> { new List<double> { 500, 0, 5000 } });
            data.R.AddRange(new List<double> { 200, 220 });
            data.RD.AddRange(new List<double> { 2500 });
            data.RDField.AddRange(new List<double> { 2500 });
            data.RR.AddRange(new List<double> { 0 });
            data.SD.AddRange(new List<double> { 500 });
            data.SDField.AddRange(new List<double> { 500 });
            data.SSP.AddRange(new List<List<double>> { new List<double> {0,1500,0,1,0,0 },
                                                new List<double> {5000,1500,0,1,0,0} });

            return data;
        }
    }
}
