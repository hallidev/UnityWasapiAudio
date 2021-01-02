using System.Collections.Generic;
using System.Linq;

namespace Assets.WasapiAudio.Scripts.Core
{
    public class SpectrumSmoother
    {
        private long _iteration;

        private readonly int _spectrumSize;
        private readonly float[] _smoothedSpectrum;
        private readonly List<float[]> _spectrumHistory = new List<float[]>();
        
        public byte SmoothingIterations { get; }

        public SpectrumSmoother(int spectrumSize, byte smoothingIterations)
        {
            _spectrumSize = spectrumSize;

            SmoothingIterations = smoothingIterations;

            _smoothedSpectrum = new float[_spectrumSize];

            for (int i = 0; i < _spectrumSize; i++)
            {
                _spectrumHistory.Add(new float[SmoothingIterations]);
            }
        }

        public float[] GetSpectrumData(float[] spectrum)
        {
            _iteration++;
            
            // Record and average last N frames
            for (var i = 0; i < _spectrumSize; i++)
            {
                var historyIndex = _iteration % SmoothingIterations;

                var audioData = spectrum[i];
                _spectrumHistory[i][historyIndex] = audioData;

                _smoothedSpectrum[i] = _spectrumHistory[i].Average();
            }

            return _smoothedSpectrum;
        }
    }
}
