using System;

namespace _02.Scripts.Sonar
{
    public interface ISonarDetectable
    {
        event Action<float> OnSonarWaveReached;
    }
}
