using System;
using System.Collections.Generic;
using System.Text;

namespace GNB.Utilities.Helpers
{
    public class RounderHelper
    {
        public static decimal RoundToBankersRounding(decimal input)
        {
            try
            {
                //Math.Round uses Banker's rounding with MidpointRounding.ToEven parameter
                return Math.Round(input, 2, MidpointRounding.ToEven);
            }
            catch (Exception ex)
            {
                LoggerHelper.LogError(ex.ToString());
                return input;
            }
        }
    }
}
