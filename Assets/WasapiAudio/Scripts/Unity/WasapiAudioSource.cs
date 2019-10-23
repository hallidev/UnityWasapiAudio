using System;
using System.Collections.Generic;
using Assets.WasapiAudio.Scripts.Core;
using Assets.WasapiAudio.Scripts.Wasapi;
using UnityEngine;

namespace Assets.WasapiAudio.Scripts.Unity
{
    [ExecuteInEditMode]
    public class WasapiAudioSource : MonoBehaviour
    {
        private readonly Dictionary<string, SpectrumSmoother> _spectrumSmoothers = new Dictionary<string, SpectrumSmoother>();

        private Wasapi.WasapiAudio _wasapiAudio;
        private float[] _spectrumData;

        // Inspector Properties
        public WasapiCaptureType CaptureType = WasapiCaptureType.Loopback;
        public int SpectrumSize = 32;
        public ScalingStrategy ScalingStrategy = ScalingStrategy.Sqrt;
        public int MinFrequency = 100;
        public int MaxFrequency = 20000;

        public void Awake()
        {
            // Setup loopback audio and start listening
            _wasapiAudio = new Wasapi.WasapiAudio(CaptureType, SpectrumSize, ScalingStrategy, MinFrequency, MaxFrequency, spectrumData =>
            {
                _spectrumData = spectrumData;
            });

            _wasapiAudio.StartListen();
        }

        public void Update()
        {
            foreach (var smoother in _spectrumSmoothers.Values)
            {
                smoother.AdvanceFrame();
            }
        }

        public float[] GetSpectrumData(AudioVisualizationStrategy strategy, bool smoothed, AudioVisualizationProfile profile)
        {
            var scaledSpectrumData = new float[SpectrumSize];
            var scaledMinMaxSpectrumData = new float[SpectrumSize];

            // Apply AudioVisualizationProfile
            var scaledMax = 0.0f;
            var scaledAverage = 0.0f;
            var scaledTotal = 0.0f;
            var scaleStep = 1.0f / SpectrumSize;

            // 2: Scaled. Scales against animation curve
            for (int i = 0; i < SpectrumSize; i++)
            {
                var scaledValue = profile.ScaleCurve.Evaluate(scaleStep * i) * _spectrumData[i];
                scaledSpectrumData[i] = scaledValue;

                if (scaledSpectrumData[i] > scaledMax)
                {
                    scaledMax = scaledSpectrumData[i];
                }

                scaledTotal += scaledValue;
            }

            // 3: MinMax
            scaledAverage = scaledTotal / SpectrumSize;
            for (int i = 0; i < SpectrumSize; i++)
            {
                var scaledValue = scaledSpectrumData[i];
                var cutoff = scaledAverage * profile.MinMaxThreshold;

                if (scaledValue <= cutoff)
                {
                    scaledValue *= profile.MinScale;
                }
                else if (scaledValue >= cutoff)
                {
                    scaledValue *= profile.MaxScale;
                }

                scaledMinMaxSpectrumData[i] = scaledValue;
            }

            // 4: Smoothed

            // We need a smoother for each combination of SpectrumSize/Iteration/Strategy
            var smootherId = $"{SpectrumSize}-{profile.AudioSmoothingIterations}-{strategy}";
            if (!_spectrumSmoothers.ContainsKey(smootherId))
            {
                _spectrumSmoothers.Add(smootherId, new SpectrumSmoother(SpectrumSize, profile.AudioSmoothingIterations));
            }

            var smoother = _spectrumSmoothers[smootherId];

            switch (strategy)
            {
                case AudioVisualizationStrategy.Raw:
                    if (smoothed)
                    {
                        return smoother.GetSpectrumData(_spectrumData);
                    }
                    return _spectrumData;
                case AudioVisualizationStrategy.Scaled:
                    if (smoothed)
                    {
                        return smoother.GetSpectrumData(scaledSpectrumData);
                    }
                    return scaledSpectrumData;
                case AudioVisualizationStrategy.ScaledMinMax:
                    if (smoothed)
                    {
                        return smoother.GetSpectrumData(scaledMinMaxSpectrumData);
                    }
                    return scaledMinMaxSpectrumData;
                default:
                    throw new InvalidOperationException($"Invalid strategy: {strategy}");
            }
        }

        public void OnApplicationQuit()
        {
            _wasapiAudio.StopListen();
        }
    }
}
