﻿using CSCore;
using CSCore.Codecs;
using CSCore.DSP;
using System;
using System.Runtime.Intrinsics.X86;

namespace EngineSynth.Model.Audio
{
    internal class Sample
    {
        public float[] Buffer;
        public int Length, SampleRate, LoopStart, LoopEnd;

        public Sample(int len, int sampleRate)
        {
            Buffer = new float[len];
            Length = len;
            SampleRate = sampleRate;
        }

        public float this[int i]
        {
            get => Buffer[i];
            set => Buffer[i] = value;
        }

        public unsafe Sample(string file)
        {
            ISampleSource source = CodecFactory.Instance.GetCodec(file).ToSampleSource();

            int ch = source.WaveFormat.Channels;
            SampleRate = source.WaveFormat.SampleRate;
            Length = (int)(source.Length / ch);
            Buffer = new float[Length];

            float[] tmp = new float[source.Length];
            source.Read(tmp, 0, (int)source.Length);

            for (int i = 0; i <= Length - ch; i++)
            {
                Buffer[i] = tmp[i * ch] * .2f;
            }

            source.Dispose();
        }

        public Sample RemoveSilence()
        {
            float max = 0;

            for (int i = 0; i < Length; i++)
            {
                max = MathF.Max(MathF.Abs(Buffer[i]), max);
            }

            // -40db
            float treshhold = max / 100;

            for (int i = 0; i < Length; i++)
            {
                if(MathF.Abs(Buffer[i]) > treshhold)
                {
                    LoopStart = i;
                    break;
                }
            }

            for (int i = Length - 1; i >= 0; i--)
            {
                if (MathF.Abs(Buffer[i]) > treshhold)
                {
                    LoopEnd = i;
                    break;
                }
            }

            Loop();

            return this;
        }

        public Sample LowPass(float freq, float res, float gain)
        {
            LowpassFilter filter = new LowpassFilter(SampleRate, Math.Min(SampleRate / 2F, freq));
            filter.Q = 1 / res;
            for (int i = 0; i < Length; i++)
            {
                Buffer[i] = filter.Process(Buffer[i]) * gain;
            }

            return this;
        }

        public Sample Stretch(float factor)
        {
            int newLength = (int)(Length / factor);
            float[] outBuffer = new float[newLength];

            for (int i = 0; i < newLength; i++)
            {
                //https://stackoverflow.com/q/1125666
                float realPos = i * factor;
                int sampleStart = (int)realPos;
                int sampleEnd = sampleStart + 1;
                float t = realPos - (float)sampleStart;
                float x0 = sampleStart - 1 > 0 ? Buffer[sampleStart - 1] : 0;
                float x1 = Buffer[sampleStart];
                float x2 = sampleEnd < Buffer.Length ? Buffer[sampleEnd] : 0;
                float x3 = sampleEnd + 1 < Buffer.Length ? Buffer[sampleEnd + 1] : 0;


                float c0 = x1;
                float c1 = .5F * (x2 - x0);
                float c2 = x0 - (2.5F * x1) + (2 * x2) - (.5F * x3);
                float c3 = (.5F * (x3 - x0)) + (1.5F * (x1 - x2));
                outBuffer[i] = (((((c3 * t) + c2) * t) + c1) * t) + c0;
            }

            Length = newLength;
            Buffer = outBuffer;

            return this;
        }

        public float GetSampleAt(float realPos)
        {
            int sampleStart = (int)realPos;
            int sampleEnd = sampleStart + 1;
            float t = realPos - (float)sampleStart;
            float x0 = Buffer[sampleStart - 1];
            float x1 = Buffer[sampleStart];
            float x2 = Buffer[sampleEnd];
            float x3 = Buffer[sampleEnd + 1];


            float c1 = .5F * (x2 - x0);
            float c2 = x0 - (2.5F * x1) + (2 * x2) - (.5F * x3);
            float c3 = (.5F * (x3 - x0)) + (1.5F * (x1 - x2));

            return (((((c3 * t) + c2) * t) + c1) * t) + x1;
        }

        public Sample Clone()
        {
            Sample sample = new Sample(Length, SampleRate);
            Array.Copy(Buffer, sample.Buffer, Length);

            return sample;
        }

        public Sample Filter(BiQuad filter)
        {
            filter.Process(Buffer);

            return this;
        }

        public unsafe bool Add(int destSample, float[] source)
        {
            if (destSample >= Length) return true;
            fixed (float* src = source, dst = &Buffer[destSample])
            {
                //ignore lost samples just because
                if (Avx.IsSupported)
                {
                    int len = Math.Min(Length - destSample, source.Length) - 7;
                    for (int j = 0; j < len; j += 8)
                    {
                        Avx.Store(dst + j, Avx.Add(Avx.LoadVector256(dst + j), Avx.LoadVector256(src + j)));
                    }
                }
                else if (Sse.IsSupported)
                {
                    int len = Math.Min(Length - destSample, source.Length) - 3;
                    for (int j = 0; j < len; j += 4)
                    {
                        Sse.Store(dst + j, Sse.Add(Sse.LoadVector128(dst + j), Sse.LoadVector128(src + j)));
                    }
                }
                else
                {
                    int len = Math.Min(Length - destSample, source.Length);
                    for (int i = 0; i < len; i++)
                    {
                        *(dst + i) += *(src + i);
                    }
                }
            }

            return Length - destSample < source.Length;
        }

        public void Loop()
        {
            int len = LoopEnd - LoopStart;
            float[] newBuffer = new float[len];

            Array.Copy(Buffer, LoopStart, newBuffer, 0, len);

            Length = len;
            Buffer = newBuffer;
        }
    }
}
