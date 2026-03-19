using System;
using UnityEngine;

namespace _02.Scripts.AIHint.Presentation
{
    public static class WavDecoder
    {
        public static AudioClip Decode(byte[] wavData, string clipName = "tts_clip")
        {
            int channels = BitConverter.ToInt16(wavData, 22);
            int sampleRate = BitConverter.ToInt32(wavData, 24);
            int bitsPerSample = BitConverter.ToInt16(wavData, 34);
            int dataStartIndex = FindDataChunkOffset(wavData);
            int dataSize = BitConverter.ToInt32(wavData, dataStartIndex + 4);
            int sampleCount = dataSize / (bitsPerSample / 8) / channels;

            float[] samples = new float[sampleCount * channels];
            int byteIndex = dataStartIndex + 8;

            for (int i = 0; i < samples.Length; i++)
            {
                if (bitsPerSample == 16)
                {
                    short sample = BitConverter.ToInt16(wavData, byteIndex);
                    samples[i] = sample / 32768f;
                    byteIndex += 2;
                }
            }

            var clip = AudioClip.Create(clipName, sampleCount, channels, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
        }

        private static int FindDataChunkOffset(byte[] wavData)
        {
            for (int i = 12; i < wavData.Length - 8; i++)
            {
                if (wavData[i] == 'd' && wavData[i + 1] == 'a'
                                      && wavData[i + 2] == 't' && wavData[i + 3] == 'a')
                {
                    return i;
                }
            }

            throw new Exception("[WavDecoder] WAV 데이터에서 'data' 청크를 찾을 수 없습니다.");
        }
    }
}