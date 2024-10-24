using UnityEngine;

namespace Cubequad
{
    public static class Utilities
    {
        /// <summary>
        /// Get normalized value of a number from the given range.
        /// For example, if you have a value 56 from range [4, 178] and you want to get this value from range [0, 1] this method for you! The result will be approximately 0.3 (from the range [0, 1])
        /// </summary>
        /// <param name="currentValue">Current value from the input range (value 56 from the example)</param>
        /// <param name="max">Maximum limit of the input range (178 from the example)</param>
        /// <param name="min">Minimum limit of the input range (4 from the example)</param>
        /// <param name="inverse">Invert the result. Optional (from the example: 1 - 0.3 = 0.7 )</param>
        /// <param name="clamp">Clamp output value to range [0, 1] (from the example if true, if input value not 56 but something like 201 the output still will be 1 and not 1.13)</param>
        /// <returns></returns>
        public static float Normalize(float currentValue, float max, float min = 0, bool inverse = false, bool clamp = false)
        {
            if (clamp)
            {
                if (currentValue > max)
                    currentValue = max;
                else if (currentValue < min)
                    currentValue = min;
            }

            var normalizedValue = (currentValue - min) / (max - min);
            if (inverse)
                return 1 - normalizedValue;
            else
                return normalizedValue;
        }

        /// <summary>
        /// Opposite of the Normalize. Input range is [0, 1] and output is [min, mix].
        /// </summary>
        /// <param name="currentValue"></param>
        /// <param name="max"></param>
        /// <param name="min"></param>
        /// <returns></returns>
        public static float Denormalize(float currentValue, float max, float min)
        {
            return currentValue * (max - min) + min;
        }
    }
}