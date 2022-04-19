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

        public static T[] CheckBuffer<T>(this T[] inst, long size, bool exactSize = false)
        {
            if (inst == null || (!exactSize && inst.Length < size) || (exactSize && inst.Length != size))
                return new T[size];
            return inst;
        }
    }
}
