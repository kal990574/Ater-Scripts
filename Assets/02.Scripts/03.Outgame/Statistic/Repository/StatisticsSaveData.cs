using System;
using UnityEngine;

[Serializable]
public class StatisticsSaveData
{
    [SerializeField] private PersistentStatistics _persistentStatistics = new PersistentStatistics();

    public PersistentStatistics PersistentStatistics => _persistentStatistics;

    public StatisticsSaveData(PersistentStatistics persistentStatistics)
    {
        _persistentStatistics = persistentStatistics ?? new PersistentStatistics();
    }
}
