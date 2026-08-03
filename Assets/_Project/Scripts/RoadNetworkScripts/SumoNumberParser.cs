#if UNITY_EDITOR
using System;
using System.Globalization;

namespace Assets.Scripts.SUMOImporter.NetFileComponents
{
    public static class SumoNumberParser
    {
        private const NumberStyles FloatingPointStyle = NumberStyles.Float;

        public static double ParseDouble(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return double.Parse(
                value,
                FloatingPointStyle,
                CultureInfo.InvariantCulture);
        }

        public static float ParseFloat(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            return float.Parse(
                value,
                FloatingPointStyle,
                CultureInfo.InvariantCulture);
        }

        public static float ParseFloatOrDefault(string value, float defaultValue)
        {
            return string.IsNullOrEmpty(value) ? defaultValue : ParseFloat(value);
        }

        public static bool TryParseDouble(string value, out double result)
        {
            return double.TryParse(
                value,
                FloatingPointStyle,
                CultureInfo.InvariantCulture,
                out result);
        }

        public static bool TryParseFloat(string value, out float result)
        {
            return float.TryParse(
                value,
                FloatingPointStyle,
                CultureInfo.InvariantCulture,
                out result);
        }
    }
}
#endif
