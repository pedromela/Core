using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UtilsLib.Utils
{
    public class LinearRegression
    {
        public static float Slope(float[] x, float[] y)
        {
            float slope = 0.0f;
            if ((x != null) && (y != null) && (x.Length == y.Length) && (x.Length > 0))
            {
                float squaresSum = SumOfSquares(x);
                slope = squaresSum > 0 ? Correlation(x, y) / squaresSum : 0;
            }
            return slope;
        }

        public static float Intercept(float[] x, float[] y)
        {
            float intercept = 0.0f;
            if ((x != null) && (y != null) && (x.Length == y.Length) && (x.Length > 0))
            {
                float xave = Average(x);
                float yave = Average(y);
                intercept = yave - Slope(x, y) * xave;
            }
            return intercept;
        }

        public static float Average(float[] values)
        {
            float average = 0.0f;
            if ((values != null) && (values.Length > 0))
            {
                float sum = 0;
                for (int i = 0; i < values.Length; i++)
                {
                    sum += values[i];
                }
                average = values.Length > 0 ? sum / values.Length : 0;
            }
            return average;
        }

        public static float SumOfSquares(float[] values)
        {
            float sumOfSquares = 0.0f;
            if ((values != null) && (values.Length > 0))
            {
                float average = Average(values);
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = values[i] * values[i];
                    sumOfSquares += values[i];
                }
                sumOfSquares -= average * average * values.Length;
            }
            return sumOfSquares;
        }

        public static float Correlation(float[] x, float[] y)
        {
            float correlation = 0.0f;
            if ((x != null) && (y != null) && (x.Length == y.Length) && (x.Length > 0))
            {
                for (int i = 0; i < x.Length; ++i)
                {
                    correlation += x[i] * y[i];
                }
                float xave = Average(x);
                float yave = Average(y);
                correlation -= xave * yave * x.Length;
            }
            return correlation;
        }
    }
}
