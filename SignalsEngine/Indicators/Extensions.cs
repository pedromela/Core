// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Pedro Mela">
//   Copyright (c) 2003-2015 Pedro Mela. All rights reserved.
// </copyright>
// <summary>
//   Contains various helpers functions
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace SignalsEngine
{
    using System;

    /// <summary>
    /// Contains various helpers functions
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Checks if the number is almost zero.
        /// </summary>
        /// <param name="value">Checked number.</param>
        /// <returns>true if is almost zero</returns>
        public static bool IsAlmostZero(this float value)
        {
            return Math.Abs(value) < float.Epsilon;
        }

        /// <summary>
        /// Compares the numbers.
        /// </summary>
        /// <param name="value">First number.</param>
        /// <param name="compareTo">Second number.</param>
        /// <returns>true if almost equal</returns>
        public static bool AlmostEqual(this float value, float compareTo)
        {
            return Math.Abs(value - compareTo) < float.Epsilon;
        }

        /// <summary>
        /// Subtracts one series from another.
        /// </summary>
        /// <param name="src">Source series.</param>
        /// <param name="dst">Subtracted series.</param>
        public static void Subtract(this float[] src, float[] dst)
        {
            for (int i = 0; i < src.Length; i++)
            {
                src[i] -= dst[i];
            }
        }

        /// <summary>
        /// Subtracts one series from another and creates new series.
        /// </summary>
        /// <param name="src">Source series.</param>
        /// <param name="dst">Subtracted series.</param>
        /// <returns>New series.</returns>
        public static float[] CreateSubstract(this float[] src, float[] dst)
        {
            var t = new float[src.Length];
            for (int i = 0; i < src.Length; i++)
            {
                t[i] = src[i] - dst[i];
            }

            return t;
        }
    }
}
