using System;

namespace CorsairLink4.Module.HidDevices.Core.Helpers
{
    public class NumConverter
    {
        public static double ToDouble(int input_val)
        {
            int num1 = (int)input_val >> 11;
            int num2 = (int)input_val & 2047;
            if (num1 > 15)
            {
                num1 = -(32 - num1);
            }

            if (num2 > 1023)
            {
                num2 = -(2048 - num2);
            }

            if ((num2 & 1) == 1)
            {
                ++num2;
            }

            return ((int)(((double)num2 * Math.Pow(2.0, (double)num1) * 10.0) + 0.5)) / 10.0;
        }

        public static double ToHalfPrecisionFloatingPoint(int val)
        {
            int exp = val >> 10;
            int sign = ((exp >> 5) & 1) == 1 ? -1 : 1;
            exp &= 31;
            exp -= 15;

            int signifVal = val & 1023;
            double significandPrecision = signifVal / 1024.0;

            return sign * Math.Pow(2.0, exp) * (1 + significandPrecision);
        }
    }
}
