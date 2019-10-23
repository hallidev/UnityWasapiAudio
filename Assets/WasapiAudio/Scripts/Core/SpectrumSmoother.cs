using System.Collections.Generic;
using System.Linq;

namespace Assets.WasapiAudio.Scripts.Core
{
    public class SpectrumSmoother
    {
        private int _frameCount;

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

        public float[] Smooth(float[] spectrum)
        {
            // Logic for smoothing the audio over N frames
            if (_frameCount == int.MaxValue) { _frameCount = 0; }

            _frameCount++;

            // Record and average last N frames
            for (int i = 0; i < _spectrumSize; i++)
            {
                int historyIndex = _frameCount % _smoothingIterations;

                float audioData = spectrum[i];
                _spectrumHistory[i][historyIndex] = audioData;

                _smoothedSpectrum[i] = _spectrumHistory[i].Average();
            }

            return _smoothedSpectrum;
        }
    }
}
