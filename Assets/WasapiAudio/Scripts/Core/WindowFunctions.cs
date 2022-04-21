using System;

namespace Assets.WasapiAudio.Scripts.Core
{
    public delegate float WindowFunction(int index, int width);

    /// <summary>
    /// Defines window functions.
    /// </summary>
    public static class WindowFunctions
    {
        /// <summary>
        /// Hamming Window
        /// </summary>
        public static readonly WindowFunction Hamming = (index, width)
            => (float)(0.54 - 0.46 * Math.Cos((2 * Math.PI * index) / (width - 1)));

        /// <summary>
        /// Hamming Window (periodic version)
        /// </summary>
        public static readonly WindowFunction HammingPeriodic = (index, width)
            => (float)(0.54 - 0.46 * Math.Cos((2 * Math.PI * index) / (width)));

        /// <summary>
        /// Hanning Window 
        /// </summary>
        public static readonly WindowFunction Hanning = (index, width)
            => (float)(0.5 - 0.5 * Math.Cos(index * ((2.0 * Math.PI) / width)));

        /// <summary>
        /// Hanning Window (periodic version)
        /// </summary>
        public static readonly WindowFunction HanningPeriodic = (index, width)
            => (float)(0.5 - 0.5 * Math.Cos(index * ((2.0 * Math.PI) / width)));

        public static readonly WindowFunction BlackmannHarris = (index, width)
            => (float)(0.35875 - (0.48829 * Math.Cos((2 * Math.PI * index) / (width - 1))) + (0.14128 * Math.Cos((4 * Math.PI * index) / (width - 1))) - (0.01168 * Math.Cos((6 * Math.PI * index) / (width - 1))));

        public static readonly WindowFunction None = (index, width) => 1.0f;
    }
}
