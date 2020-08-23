using System;
using UnityEngine;

namespace SweetLibs
{
    public static class MathHelpers
    {
        public static float Map(float value, float initialMin, float initialMax, float destinationMin,
            float destinationMax)
        {
            var t = (value - initialMin) / (initialMax - initialMin);
            return Mathf.Lerp(destinationMin, destinationMax, t);
        }

        /// <summary>
        /// Wraps the given value around the given minimum [inclusive] and maximum [inclusive]. Returns the given value if it is 
        /// within the minimum and maximum range. Returns the minimum value added by the difference of the maximum value 
        /// and the value if the value is greater than the maximum. Vice versa for the minimum.
        /// </summary>
        /// <param name="value">The integer value to wrap within the given range defined by the min and max values.</param>
        /// <param name="min">The minimum value to compare against.</param>
        /// <param name="max">The maximum value to compare against.</param>
        /// <returns>The result wrapped around the min and max values.</returns>
        public static int Wrap(int value, int min, int max)
        {
            if (max < min)
            {
                throw new ArgumentException("Max must be greater than min");
            }

            if (max == min)
            {
                return min;
            }

            if (value >= min && value <= max)
            {
                return value;
            }

            int range = (max - min) + 1; // + 1 because max is inclusive
            int wrappedValue = (value - min) % range;

            if (wrappedValue < 0)
            {
                wrappedValue += range;
            }

            return wrappedValue + min;
        }
    }
}