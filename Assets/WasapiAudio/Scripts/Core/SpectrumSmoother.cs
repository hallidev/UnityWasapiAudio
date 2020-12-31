using System.Collections.Generic;
using System.Linq;

namespace Assets.WasapiAudio.Scripts.Core
{
    public class SpectrumSmoother
    {
        private long _frameCount;

        private readonly int _spectrumSize;
        private readonly int _smoothingIterations;
        private readonly float[] _smoothedSpectrum;
        private readonly List<float[]> _spectrumHistory = new List<float[]>();

        public SpectrumSmoother(int spectrumSize, int smoothingIterations)
        {
            _spectrumSize = spectrumSize;
            _smoothingIterations = smoothingIterations;

            _smoothedSpectrum = new float[_spectrumSize];

            for (int i = 0; i < _spectrumSize; i++)
            {
                _spectrumHistory.Add(new float[_smoothingIterations]);
            }
        }

        public void AdvanceFrame()
        {
            _frameCount++;
        }

        public float[] GetSpectrumData(float[] spectrum)
        {
            // Record and average last N frames
            for (var i = 0; i < _spectrumSize; i++)
            {
                var historyIndex = _frameCount % _smoothingIterations;

                var audioData = spectrum[i];
                _spectrumHistory[i][historyIndex] = audioData;

                _smoothedSpectrum[i] = _spectrumHistory[i].Average();
            }

            return _smoothedSpectrum;
        }
    }
}
