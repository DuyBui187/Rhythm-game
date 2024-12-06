using System;
using UnityEngine;

namespace RhythmTool
{
    /// <summary>
    /// The BeatTracker estimates at which points beats occur in the song. 
    /// The beat represents the rhythm of the song.
    /// </summary>
    [DisallowMultipleComponent, AddComponentMenu("RhythmTool/Beat Tracker")]
    public class BeatTracker : Analysis<Beat>
    {
        public override string name
        {
            get
            {
                return "Beats";
            }
        }

        [Range(60, 190), Tooltip("Indicate a tempo to for the beat tracker to prefer.")]
        public int bpmHint = 120;

        [Range(0, 1), Tooltip("Determines how strongly the beat tracker should prefer tempos around the hint. A value of 0 ignores the hint, while a value of 1 will focus on a small range of tempos around the hint.")]
        public float hintStrength = 0;

        private float[] signalBuffer;

        private float[] signal;
        private float[] smoothedSignal;

        private float[] signalWindow;
        private float[] smoothingKernel;

        private float[] autoCorrelation;
        private float[] combFilter;

        private float[] lengthScore;
        private float[] offsetScore;

        private float[] lengthWeight;
        private float[] offsetWeight;

        private float[] prevMagnitude;
        private float prevSpectralFlux;

        private int maxBeatLength;
        private int minBeatLength;
        private int beatLength;
        private int prevBeatLength;

        private int beatOffset;
        private int updateOffset;

        private int bufferSize;

        private int resolution = 10;
        private int combElements = 8;

        public override void Initialize(int sampleRate, int frameSize, int hopSize)
        {
            base.Initialize(sampleRate, frameSize, hopSize);

            float framesPerMinute = (sampleRate * 60f) / hopSize;

            float minBPM = bpmHint / 1.5f;
            float maxBPM = bpmHint / .75f;

            maxBeatLength = Mathf.RoundToInt(framesPerMinute / minBPM);
            minBeatLength = Mathf.RoundToInt(framesPerMinute / maxBPM);

            bufferSize = maxBeatLength * combElements * 2;

            signalBuffer = new float[bufferSize];

            signal = new float[bufferSize];
            smoothedSignal = new float[bufferSize];

            autoCorrelation = new float[bufferSize];
            combFilter = new float[maxBeatLength * 2 * resolution];

            lengthScore = new float[maxBeatLength * resolution];
            offsetScore = new float[maxBeatLength * resolution];

            signalWindow = new float[bufferSize / 2];
            for (int i = 0; i < bufferSize / 2; i++)
                signalWindow[i] = Util.HannWindow(i, bufferSize);

            smoothingKernel = Util.HannWindow(8);

            offsetWeight = new float[maxBeatLength * resolution];

            lengthWeight = new float[maxBeatLength * resolution];

            float m = Mathf.RoundToInt(framesPerMinute / bpmHint * resolution);
            float s = (1.1f - hintStrength) * lengthWeight.Length;

            for (int i = 0; i < lengthWeight.Length; i++)
                lengthWeight[i] = 5 * Mathf.Exp(-(Mathf.Pow(i - m, 2) / Mathf.Pow(s, 2)));

            for (int i = 0; i < lengthWeight.Length; i++)
                lengthWeight[i] = (1 - hintStrength) + lengthWeight[i] * hintStrength;

            //float f = 1f / lengthWeight.Length;

            //for (int i = 0; i < lengthWeight.Length; i++)
            //    lengthWeight[i] += -f * i + 1f;

            prevMagnitude = new float[frameSize / 2];
            prevSpectralFlux = 0;

            prevBeatLength = 0;
            beatLength = (minBeatLength + minBeatLength / 2) * resolution;
            updateOffset = maxBeatLength;
            beatOffset = -1;
        }

        public override void Process(float[] samples, float[] magnitude, int frameIndex)
        {
            base.Process(samples, magnitude, frameIndex);

            float sample = GetSample(magnitude);

            signalBuffer[frameIndex % bufferSize] = sample;

            beatOffset--;
            updateOffset--;

            if (updateOffset == 0)
            {
                UpdateSignal();
                UpdateLength();
                UpdateOffset();
            }

            if (beatOffset == 0)
            {
                Beat beat = new Beat()
                {
                    timestamp = FrameIndexToSeconds(frameIndex),
                    bpm = 60 / FrameIndexToSeconds((float)beatLength / resolution),
                };

                track.Add(beat);
            }
        }

        private float GetSample(float[] magnitude)
        {
            float spectralFlux = 0;

            for (int i = 0; i < magnitude.Length - 2; i++)
            //for (int i = 0; i < magnitude.Length; i++)
                spectralFlux += Mathf.Max(magnitude[i] - prevMagnitude[i], 0);

            Array.Copy(magnitude, prevMagnitude, magnitude.Length);

            float sample = spectralFlux - prevSpectralFlux;

            prevSpectralFlux = spectralFlux;

            return sample;
        }

        private void UpdateSignal()
        {
            for (int i = 0; i < bufferSize; i++)
                signal[i] = signalBuffer[(i + frameIndex + 1) % bufferSize];

            Array.Clear(signal, 0, 4);
            Array.Clear(signal, signal.Length - 4, 4);

            Util.Smooth(signal, smoothedSignal, smoothingKernel);

            for (int i = 0; i < signalWindow.Length; i++)
                smoothedSignal[i] *= signalWindow[i];
        }

        private void UpdateOffset()
        {
            float f = (float)beatLength / resolution;

            for (int i = 0; i < beatLength; i++)
            {
                float sum = 0;

                float offset = (float)i / resolution;
                offset = bufferSize - 1 - (f - offset);
                int n = Mathf.RoundToInt(offset / f);

                for (int j = 0; j < n; j++)
                    sum += Util.Interpolate(smoothedSignal, offset - j * f);

                float score = (sum / n) * offsetWeight[i];

                offsetScore[i] = Mathf.Lerp(offsetScore[i], score, .1f);
            }

            int max = Util.MaxIndex(offsetScore, 0, beatLength);

            beatOffset = Mathf.RoundToInt((float)max / resolution);
            //updateOffset = beatOffset + Mathf.RoundToInt(beatLength / 2f / resolution);
            updateOffset = beatOffset + Mathf.RoundToInt(beatLength / 2 / resolution);

            if (offsetScore[max] < .15f)
                beatOffset = -1;
        }

        private void UpdateLength()
        {
            UpdateAutoCorrelation();
            UpdateLengthScore();

            beatLength = Util.MaxIndex(lengthScore, minBeatLength * resolution);

            if (beatLength != prevBeatLength)
            {
                for (int i = 0; i < beatLength; i++)
                    offsetWeight[i] = .75f + Util.HannWindow(i, beatLength) * .25f;

                Array.Clear(offsetScore, beatLength, offsetScore.Length - beatLength);

                if ((float)Mathf.Abs(beatLength - prevBeatLength) / (prevBeatLength) > .1f)
                    Array.Clear(offsetScore, 0, offsetScore.Length);
            }

            prevBeatLength = beatLength;
        }

        private void UpdateAutoCorrelation()
        {
            for (int i = minBeatLength / 2; i < autoCorrelation.Length; i++)
            {
                float sum = 0;

                for (int j = 0; j < smoothedSignal.Length - i; j++)
                    sum += smoothedSignal[j] * smoothedSignal[j + i];

                autoCorrelation[i] = sum / (smoothedSignal.Length - i);
            }

            float max = Util.Max(autoCorrelation, minBeatLength / 2);

            if (max < 1)
                return;

            for (int i = 0; i < autoCorrelation.Length; i++)
                autoCorrelation[i] /= max;
        }

        private void UpdateLengthScore()
        {
            for (int i = minBeatLength * resolution / 2; i < combFilter.Length - 1; i++)
            {
                float f = (float)i / resolution;

                float sum = 0;

                for (int j = 0; j < combElements; j++)
                    sum += Util.Interpolate(autoCorrelation, (j + 1) * f);

                combFilter[i] = sum / combElements;
            }

            for (int i = minBeatLength * resolution; i < lengthScore.Length; i++)
            {
                float score = combFilter[i] + combFilter[i / 2] + combFilter[i * 2];

                score *= lengthWeight[i];

                lengthScore[i] = Mathf.Lerp(lengthScore[i], score, .1f);
            }
        }
    }
}