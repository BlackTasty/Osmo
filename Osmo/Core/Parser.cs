using System;

namespace Osmo.Core
{
    static class Parser
    {
        public static bool TryParse(string value, bool defaultValue)
        {
            if (bool.TryParse(value, out bool result))
                return result;
            else
                return defaultValue;
        }

        public static int TryParse(string value, int defaultValue)
        {
            double temp = TryParse(value, (double)defaultValue);
            if (temp > Int32.MaxValue)
            {
                return Int32.MaxValue;
            }
            else if (temp < Int32.MinValue)
            {
                return Int32.MinValue;
            }
            else
            {
                if (int.TryParse(value, out int result))
                    return result;
                else
                    return defaultValue;
            }
        }

        public static double TryParse(string value, double defaultValue)
        {
            if (double.TryParse(value, out double result))
                return result;
            else
                return defaultValue;
        }
    }
}
