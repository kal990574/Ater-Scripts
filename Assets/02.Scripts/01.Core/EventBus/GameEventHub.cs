using System;
using UnityEngine;

//유니티 생명주기용
//싱글톤, 
[DefaultExecutionOrder(-1000)]
public class GameEventHub : MonoBehaviour
{
    public static GameEventHub Instance { get; private set; }

    [Header("Debug")] 
    [SerializeField] private bool dontDestroyOnLoad = true;
    [SerializeField] private bool enablePublishLog = false;

    private GameEventBus _eventBus;
    private int _sequence;

    public IGameEventBus Bus => _eventBus;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        _eventBus = new GameEventBus();
        _sequence = 0;

        if (dontDestroyOnLoad == true)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance != this)
        {
            return;
        }

        _eventBus?.Clear();
        Instance = null;
    }

    public IDisposable Subscribe<T>(Action<T> handler) where T : struct, IGameEvent
    {
        return _eventBus.Subscribe(handler);
    }

    public void Publish<T>(in T gameEvent) where T : struct, IGameEvent
    {
        if (enablePublishLog == true)
        {
            Debug.Log($"[GameEventHub] Publish<{typeof(T).Name}> :: {gameEvent.Context}");
        }

        _eventBus.Publish(in gameEvent);
    }

    public int NextSequence()
    {
        _sequence++;
        return _sequence;
    }
    
    public void ResetSequence()
    {
        _sequence = 0;
    }
}