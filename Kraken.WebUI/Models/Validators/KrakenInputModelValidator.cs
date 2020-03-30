using System.Collections.Generic;
using System.Linq;

namespace Kraken.WebUI.Models.Validators
{
    public class KrakenInputModelValidator
    {
        public List<string> Validate(KrakenInputModel model)
        {
            var errors = new List<string>();

            if (model.Frequency <= 0)
            {
                errors.Add("Frequency must be greater than 0");
            }
            if (model.NModes <= 0)
            {
                errors.Add("Number of modes must be greater than 0");
            }
            if (model.NMedia <= 0)
            {
                errors.Add("Number of media must be greater than 0");
            }

            if (!IsValidTopBCType(model.TopBCType))
            {
                errors.Add("Top boundary condition is required and must contains only one character");
            }
            if (!IsValidInterpolationType(model.InterpolationType))
            {
                errors.Add("Interpolation type is required and must contains only one character");
            }
            if (!IsValidAttenuationUnit(model.AttenuationUnits))
            {
                errors.Add("Attenuation unit is required and must contains only one character");
            }


            if (model.TopBCType == "A")
            {
                if (model.ZT < 0)
                {
                    errors.Add("Z Top must be greater than or equal 0");
                }

                if (model.CPT < 0)
                {
                    errors.Add("CP Top must be greater than or equal 0");
                }

                if (model.CST < 0)
                {
                    errors.Add("CS Top must be greater than or equal 0");
                }

                if (model.RHOT < 0)
                {
                    errors.Add("RHO Top must be greater than or equal 0");
                }

                if (model.APT < 0)
                {
                    errors.Add("AP Top must be greater than or equal 0");
                }

                if (model.AST < 0)
                {
                    errors.Add("AS Top must be greater than or equal 0");
                }
            }

            if (IsTwersky(model.TopBCType) && model.BumDen < 0)
            {
                errors.Add("Bump density must be greater than or equal 0");
            }
            if (IsTwersky(model.TopBCType) && model.Eta < 0)
            {
                errors.Add("Principal radius 1 must be greater than or equal 0");
            }
            if (IsTwersky(model.TopBCType) && model.Xi < 0)
            {
                errors.Add("Principal radius 2 must be greater than or equal 0");
            }

            if (!IsValidBottomBCType(model.BottomBCType))
            {
                errors.Add("Bottom boundary condition is required and must contains only one character");
            }

            if (model.BottomBCType == "A")
            {
                if (model.ZB < 0)
                {
                    errors.Add("Z Bottom must be greater than or equal 0");
                }

                if (model.CPB < 0)
                {
                    errors.Add("CP Bottom must be greater than or equal 0");
                }

                if (model.CSB < 0)
                {
                    errors.Add("CS Bottom must be greater than or equal 0");
                }

                if (model.RHOB < 0)
                {
                    errors.Add("RHO Bottom must be greater than or equal 0");
                }

                if (model.APB < 0)
                {
                    errors.Add("AP Bottom must be greater than or equal 0");
                }

                if (model.ASB < 0)
                {
                    errors.Add("AS Bottom must be greater than or equal 0");
                }
            }

            if (model.CLow <= 0)
            {
                errors.Add("Lower phase speed limit must be greater than 0");
            }
            if (model.CHigh <= 0)
            {
                errors.Add("Upper phase speed limit must be greater than 0");
            }

            if (model.RMax < 0)
            {
                errors.Add("Maximum range must be greater than or equal 0");
            }

            if (model.NSD <= 0)
            {
                errors.Add("Number of source depth must be greater than 0");
            }
            if (model.NRD <= 0)
            {
                errors.Add("Number of receiver depth must be greater than 0");
            }

            if (model.SD == null || model.SD.Count == 0)
            {
                errors.Add("Source depth is required");
            }
            else if (model.SD.Any(x => x < 0))
            {
                errors.Add("Source depth must consist of non-negative numbers");
            }

            if (model.RD == null || model.RD.Count == 0)
            {
                errors.Add("Receiver depth is required");
            }
            else if (model.RD.Any(x => x < 0))
            {
                errors.Add("Receiver depth must consist of non-negative numbers");
            }

            if (model.MediumInfo == null || model.MediumInfo.Count == 0)
            {
                errors.Add("Medium info is required");
            }
            else
            {
                foreach (var m in model.MediumInfo)
                {                   

                    if (m == null || m.Count != 3)
                    {
                        errors.Add("Medium info must consist of lists with 3 elements each");
                        break;
                    }

                    if (m.Any(x => x < 0))
                    {
                        errors.Add("Medium info can't contain negative numbers");
                        break;
                    }
                }
            }

            if (model.SSP == null || model.SSP.Count == 0)
            {
                errors.Add("Sound speed profile is required");
            }
            else
            {
                foreach (var ssp in model.SSP)
                {
                    if (ssp == null || ssp.Count != 6)
                    {
                        errors.Add("Sound speed profile must consist of lists with 6 elements each");
                        break;
                    }

                    if (ssp.Any(x => x < 0))
                    {
                        errors.Add("Sound speed profile can't contain negative numbers");
                    }
                }
            }

            if (model.CalculateTransmissionLoss)
            {
                if (model.NModesForField <= 0)
                {
                    errors.Add("Number of modes for field computing must be greater than 0");
                }

                if (!IsValidSourceType(model.SourceType))
                {
                    errors.Add("Source type is required and must contains only one character");
                }

                if (!IsValidModesTheory(model.ModesTheory))
                {
                    errors.Add("Modes theory is required and must contains only one character");
                }

                if (model.NSDField <= 0)
                {
                    errors.Add("Number of source depth (for field) must be greater than 0");
                }

                if (model.NRDField <= 0)
                {
                    errors.Add("Number of receiver depth (for field) must be greater than 0");
                }              

                if (model.NR <= 0)
                {
                    errors.Add("The number of receiver ranges must be greater than 0");
                }

                if (model.NRR <= 0)
                {
                    errors.Add("The number of receiver range-displacements must be greater than 0");
                }

                if (model.RR == null || model.RR.Count == 0)
                {
                    errors.Add("The receiver displacements are required");
                }
                else if (model.RR.Any(x => x < 0))
                {
                    errors.Add("The receiver displacements must consist of non-negative numbers");
                }

                if (model.R == null || model.R.Count == 0)
                {
                    errors.Add("The receiver ranges are required");
                }
                else if (model.R.Any(x => x < 0))
                {
                    errors.Add("The receiver ranges must consist of non-negative numbers");
                }

                if (model.SDField == null || model.SDField.Count == 0)
                {
                    errors.Add("Source depth (for field) is required");
                }
                else if (model.SDField.Any(x => x < 0))
                {
                    errors.Add("Source depth (for field) must consist of non-negative numbers");
                }

                if (model.RDField == null || model.RDField.Count == 0)
                {
                    errors.Add("Receiver depth (for field) is required");
                }
                else if (model.RDField.Any(x => x < 0))
                {
                    errors.Add("Receiver depth (for field) must consist of non-negative numbers");
                }
            }

            return errors;
        }

        private bool IsValidTopBCType(string topBCType)
        {
            return IsNotEmptyAndHasValidLength(topBCType) && "VARSHTI".Contains(topBCType);
        }

        private bool IsValidBottomBCType(string bottomBCType)
        {
            return IsNotEmptyAndHasValidLength(bottomBCType) && "VAR".Contains(bottomBCType);
        }

        private bool IsValidInterpolationType(string interpolationType)
        {
            return IsNotEmptyAndHasValidLength(interpolationType) && "CNS".Contains(interpolationType);
        }

        private bool IsValidAttenuationUnit(string attenuationUnit)
        {
            return IsNotEmptyAndHasValidLength(attenuationUnit) && "NFMWQT".Contains(attenuationUnit);
        }

        private bool IsValidSourceType(string sourceType)
        {
            return IsNotEmptyAndHasValidLength(sourceType) && "RX".Contains(sourceType);
        }

        private bool IsValidModesTheory(string modesTheory)
        {
            return IsNotEmptyAndHasValidLength(modesTheory) && "AS".Contains(modesTheory);
        }

        private bool IsNotEmptyAndHasValidLength(string value)
        {
            return !string.IsNullOrWhiteSpace(value) && value.Length == 1;
        }

        private bool IsTwersky(string bcType)
        {
            return bcType.Length == 1 && "SHTI".Contains(bcType);
        }
    }
}
