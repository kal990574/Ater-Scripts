using System;
using System.IO;

namespace _02.Scripts.AIHint.Presentation
{
  public static class WavEncoder
  {
      public static byte[] Encode(float[] samples, int sampleRate, int channels = 1)
      {
          int trimmedLength = TrimSilence(samples);
          short[] pcmData = ConvertToPcm16(samples, trimmedLength);

          using var stream = new MemoryStream();
          using var writer = new BinaryWriter(stream);

          int dataSize = pcmData.Length * 2;
          int fileSize = 44 + dataSize;

          // RIFF 헤더
          writer.Write(new[] { 'R', 'I', 'F', 'F' });
          writer.Write(fileSize - 8);
          writer.Write(new[] { 'W', 'A', 'V', 'E' });

          // fmt 청크
          writer.Write(new[] { 'f', 'm', 't', ' ' });
          writer.Write(16);                              // 청크 크기
          writer.Write((short)1);                        // PCM 포맷
          writer.Write((short)channels);                 // 채널 수
          writer.Write(sampleRate);                      // 샘플레이트
          writer.Write(sampleRate * channels * 2);       // 바이트레이트
          writer.Write((short)(channels * 2));           // 블록 정렬
          writer.Write((short)16);                       // 비트 깊이

          // data 청크
          writer.Write(new[] { 'd', 'a', 't', 'a' });
          writer.Write(dataSize);

          foreach (short sample in pcmData)
          {
              writer.Write(sample);
          }

          return stream.ToArray();
      }

      private static short[] ConvertToPcm16(float[] samples, int length)
      {
          var pcm = new short[length];

          for (int i = 0; i < length; i++)
          {
              float clamped = Math.Clamp(samples[i], -1f, 1f);
              pcm[i] = (short)(clamped * short.MaxValue);
          }

          return pcm;
      }

      private static int TrimSilence(float[] samples, float threshold = 0.01f)
      {
          int lastNonSilent = samples.Length - 1;

          while (lastNonSilent > 0 && Math.Abs(samples[lastNonSilent]) < threshold)
          {
              lastNonSilent--;
          }

          return lastNonSilent + 1;
      }
  }
}
