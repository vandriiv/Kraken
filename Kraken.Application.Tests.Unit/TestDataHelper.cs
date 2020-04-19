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

        public AcousticProblemData GetAcousticProblemDataForKrakenAndField()
        {
            var data = GetAcousticProblemDataForKraken();

            data.CalculateTransmissionLoss = true;

            return data;
        }

        public KrakenResult GetKrakenResult()
        {
            var data = new KrakenResult
            {
                ModesCount = 44,
                GroupSpeed = Enumerable.Repeat(0d, 45).ToList(),
                PhaseSpeed = Enumerable.Repeat(0d, 45).ToList(),
                K = Enumerable.Repeat(new Complex(), 45).ToList(),
                Modes = new List<List<double>> { new List<double> { 0 },
                                                  new List<double> {0} },
                Warnings = new List<string>(),
                ZM = Enumerable.Repeat(0d, 3).ToList()
            };

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
            data.Depth = Enumerable.Repeat(0d, 501).ToList();
            data.DepthBottom = 5000;
            data.DepthTop = 0;
            data.Frequency = 10;
            data.K = Enumerable.Repeat(new Complex(), 45).ToList();
            data.Material = Enumerable.Repeat("", 501).ToList();
            data.ModesCount = 44;
            data.N = Enumerable.Repeat(0, 501).ToList();
            data.NMat = 2;
            data.NMedia = 1;
            data.NTot = 2;
            data.Phi = new List<List<Complex>>() { new List<Complex> { 0, 0 } };
            data.RhoBottom = 2;
            data.RhoTop = 0;
            data.Z = new List<double> { 0, 0, 0 };

            return data;
        }

        public AcousticFieldSnapshots GetAcousticFieldSnapshots()
        {
            var data = new AcousticFieldSnapshots
            {
                Ranges = new List<double> { 0, 0 },
                ReceiverDepths = new List<double> { 0, 0 },
                SourceDepths = new List<double> { 0, 0 },
                Warnings = new List<string>(),
                Snapshots = new List<List<List<Complex>>> { new List<List<Complex>> { },
            new List<List<Complex>>{ new List<Complex> { 0,0 }, new List<Complex> { 0,0 } }}
            };

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
                MediumInfo = new List<List<double>> { new List<double> { 500, 0, 5000 } },
                ModesTheory = "A",
                NMedia = 1,
                NModes = 44,
                NModesForField = 9999,
                NR = 1,
                NRDField = 1,
                NRR = 1,
                NSD = 1,
                NSDField = 1,
                R = new List<double> { 200, 220 },
                RD = new List<double> { 2500 },
                RDField = new List<double> { 2500 },
                RHOB = 2,
                RHOT = 0,
                RMax = 1000,
                RR = new List<double> { 0 },
                SD = new List<double> { 500 },
                SDField = new List<double> { 500 },
                SSP = new List<List<double>> { new List<double> {0,1500,0,1,0,0 },
                                                new List<double> {5000,1500,0,1,0,0} },
                Sigma = 0,
                SourceType = "R",
                TopBCType = "V",
                Xi = 0,
                ZB = 5000,
                ZT = 0
            };

            return data;
        }
    }
}
