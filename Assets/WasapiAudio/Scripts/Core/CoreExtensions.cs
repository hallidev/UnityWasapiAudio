using NAudio.Dsp;
using System;

namespace Assets.WasapiAudio.Scripts.Core
{
    internal static class CoreExtensions
    {
        public static float Value(this Complex complex)
        {
            return (float) Math.Sqrt(complex.X * complex.X + complex.Y * complex.Y);
        }
    }
}
