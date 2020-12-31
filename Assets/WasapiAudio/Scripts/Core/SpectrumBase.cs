using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using CSCore;
using CSCore.DSP;

namespace Assets.WasapiAudio.Scripts.Core
{
    internal abstract class SpectrumBase
    {
        private const int ScaleFactorLinear = 9;
        protected const int ScaleFactorSqr = 2;
        protected const double MinDbValue = -90;
        protected const double MaxDbValue = 0;
        protected const double DbScale = (MaxDbValue - MinDbValue);

        private int _fftSize;
        private bool _isXLogScale;
        private int _maxFftIndex;
        private int _minFrequency;
        private int _minimumFrequencyIndex;
        private int _maxFrequency;
        private int _maximumFrequencyIndex;
        private int[] _spectrumIndexMax;
        private int[] _spectrumLogScaleIndexMax;
        private ISpectrumProvider _spectrumProvider;

        protected int SpectrumResolution;
        private bool _useAverage;

        [Browsable(false)]
        public ISpectrumProvider SpectrumProvider
        {
            get => _spectrumProvider;
            set => _spectrumProvider = value ?? throw new ArgumentNullException(nameof(value));
        }

        public bool IsXLogScale
        {
            get => _isXLogScale;
            set
            {
                _isXLogScale = value;
                UpdateFrequencyMapping();
            }
        }

        public ScalingStrategy ScalingStrategy { get; set; }

        public bool UseAverage
        {
            get => _useAverage;
            set => _useAverage = value;
        }

        [Browsable(false)]
        public FftSize FftSize
        {
            get => (FftSize) _fftSize;
            protected set
            {
                if ((int) Math.Log((int) value, 2) % 1 != 0)
                    throw new ArgumentOutOfRangeException(nameof(value));

                _fftSize = (int) value;
                _maxFftIndex = _fftSize / 2 - 1;
            }
        }

        public SpectrumBase(int minFrequency, int maxFrequency)
        {
            _minFrequency = minFrequency;
            _maxFrequency = maxFrequency;
        }

        protected virtual void UpdateFrequencyMapping()
        {
            _minimumFrequencyIndex = Math.Min(_spectrumProvider.GetFftBandIndex(_minFrequency), _maxFftIndex);
            _maximumFrequencyIndex = Math.Min(_spectrumProvider.GetFftBandIndex(_maxFrequency) + 1, _maxFftIndex);

            var actualResolution = SpectrumResolution;

            var indexCount = _maximumFrequencyIndex - _minimumFrequencyIndex;
            var linearIndexBucketSize = Math.Round(indexCount / (double) actualResolution, 3);

            _spectrumIndexMax = _spectrumIndexMax.CheckBuffer(actualResolution, true);
            _spectrumLogScaleIndexMax = _spectrumLogScaleIndexMax.CheckBuffer(actualResolution, true);

            var maxLog = Math.Log(actualResolution, actualResolution);
            
            for (int i = 1; i < actualResolution; i++)
            {
                var logIndex =
                    (int) ((maxLog - Math.Log((actualResolution + 1) - i, (actualResolution + 1))) * indexCount) +
                    _minimumFrequencyIndex;

                _spectrumIndexMax[i - 1] = _minimumFrequencyIndex + (int) (i * linearIndexBucketSize);
                _spectrumLogScaleIndexMax[i - 1] = logIndex;
            }

            if (actualResolution > 0)
            {
                _spectrumIndexMax[_spectrumIndexMax.Length - 1] =
                    _spectrumLogScaleIndexMax[_spectrumLogScaleIndexMax.Length - 1] = _maximumFrequencyIndex;
            }
        }

        protected virtual SpectrumPointData[] CalculateSpectrumPoints(double maxValue, float[] fftBuffer)
        {
            var dataPoints = new List<SpectrumPointData>();

            double value0 = 0, value = 0;
            double lastValue = 0;
            double actualMaxValue = maxValue;
            var spectrumPointIndex = 0;

            for (int i = _minimumFrequencyIndex; i <= _maximumFrequencyIndex; i++)
            {
                switch (ScalingStrategy)
                {
                    case ScalingStrategy.Decibel:
                        value0 = (((20 * Math.Log10(fftBuffer[i])) - MinDbValue) / DbScale) * actualMaxValue;
                        break;
                    case ScalingStrategy.Linear:
                        value0 = (fftBuffer[i] * ScaleFactorLinear) * actualMaxValue;
                        break;
                    case ScalingStrategy.Sqrt:
                        value0 = ((Math.Sqrt(fftBuffer[i])) * ScaleFactorSqr) * actualMaxValue;
                        break;
                }

                var recalc = true;

                value = Math.Max(0, Math.Max(value0, value));

                while (spectrumPointIndex <= _spectrumIndexMax.Length - 1 &&
                       i ==
                       (IsXLogScale
                           ? _spectrumLogScaleIndexMax[spectrumPointIndex]
                           : _spectrumIndexMax[spectrumPointIndex]))
                {
                    if (!recalc)
                    {
                        value = lastValue;
                    }

                    if (value > maxValue)
                    {
                        value = maxValue;
                    }

                    if (_useAverage && spectrumPointIndex > 0)
                    {
                        value = (lastValue + value) / 2.0;
                    }

                    dataPoints.Add(new SpectrumPointData {SpectrumPointIndex = spectrumPointIndex, Value = value});

                    lastValue = value;
                    value = 0.0;
                    spectrumPointIndex++;
                    recalc = false;
                }
            }

            return dataPoints.ToArray();
        }

        [DebuggerDisplay("{" + nameof(Value) + "}")]
        protected struct SpectrumPointData
        {
            public int SpectrumPointIndex;
            public double Value;
        }
    }
}