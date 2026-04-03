using System;

public enum ESubJumpScareType
{
    None = 0,
    FakeEnemy = 1,
    Sound = 2,
    PostProcess = 3
}

public enum ESubJumpScareIntensity
{
    None = 0,
    Weak = 1,
    Medium = 2,
    Strong = 3
}

public enum ERelativeDirection
{
    Front = 0,
    Back = 1,
    Left = 2,
    Right = 3,
    Random = 4
}

public enum ESubJumpScareTriggerType
{
    None = 0,
    Periodic = 1,
    Sonar = 2
}