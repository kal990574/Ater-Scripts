//디버깅을 위한 콘텍스트다.

using System;
using UnityEngine;

[Serializable]
public struct GameEventContext
{
    [field: SerializeField] public int Frame { get; private set; }   //발생프레임 = Time.frameCount
    [field: SerializeField] public float Time { get; private set; }   //발생시간
    [field: SerializeField] public int Sequence { get; private set; }     //같은 프레임에 이벤트가 발생했을때의 순번
    [field: SerializeField] public int SourceId { get; private set; }    //어떤 객체가 해당 이벤트를 발생시켰는가
    [field: SerializeField] public string SourceName { get; private set; }
    
    public GameEventContext(int frame, float time, int sequence, int sourceId, string sourceName)
    {
        Frame = frame;
        Time = time;
        Sequence = sequence;
        SourceId = sourceId;
        SourceName = sourceName;
    }

    public override string ToString()
    {
        return $"Frame: {Frame}, Time: {Time:F3}, Sequence: {Sequence}, SourceId: {SourceId}, SourceName: {SourceName}";
    }
}