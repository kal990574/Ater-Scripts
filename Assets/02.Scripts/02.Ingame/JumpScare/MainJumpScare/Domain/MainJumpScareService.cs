using System.Collections.Generic;
using UnityEngine;

public sealed class MainJumpScareService
{
    private readonly JumpScareManager _owner;
    private readonly Object _logContext;

    private readonly Dictionary<string, MainJumpScareBase> _mainJumpScaresById = new Dictionary<string, MainJumpScareBase>();
    private readonly HashSet<string> _playingMainJumpScareIds = new HashSet<string>();

    public bool IsAnyMainJumpScarePlaying => _playingMainJumpScareIds.Count > 0;

    public MainJumpScareService(JumpScareManager owner)
    {
        _owner = owner;
        _logContext = owner;
    }

    public void RegisterSceneMainJumpScares()
    {
        _mainJumpScaresById.Clear();
        _playingMainJumpScareIds.Clear();

        FindObjectsInactive findObjectsInactive = ShouldIncludeInactiveOnRegister()
            ? FindObjectsInactive.Include
            : FindObjectsInactive.Exclude;

        MainJumpScareBase[] mainJumpScares =
            Object.FindObjectsByType<MainJumpScareBase>(findObjectsInactive, FindObjectsSortMode.None);

        for (int index = 0; index < mainJumpScares.Length; index++)
        {
            MainJumpScareBase mainJumpScare = mainJumpScares[index];
            if (mainJumpScare == null)
            {
                continue;
            }

            RegisterMainJumpScare(mainJumpScare);
        }

        if (IsMainLogEnabled())
        {
            Debug.Log($"[MainJumpScareService] Registered {_mainJumpScaresById.Count} main jump scares.", _logContext);
        }
    }

    public void ExecuteMainJumpScare(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            Debug.LogError("[MainJumpScareService] Main jump scare id is required.", _logContext);
            return;
        }

        if (_mainJumpScaresById.TryGetValue(id, out MainJumpScareBase mainJumpScare) == false)
        {
            Debug.LogError($"[MainJumpScareService] Main jump scare [{id}] was not found.", _logContext);
            return;
        }

        if (mainJumpScare.CanActive == false)
        {
            if (IsMainLogEnabled())
            {
                Debug.LogWarning($"[MainJumpScareService] Main jump scare [{id}] is not active.", _logContext);
            }

            return;
        }

        if (mainJumpScare.State == EMainJumpScareState.Playing)
        {
            if (IsMainLogEnabled())
            {
                Debug.LogWarning($"[MainJumpScareService] Main jump scare [{id}] is already playing.", _logContext);
            }

            return;
        }

        if (mainJumpScare.State == EMainJumpScareState.Finished)
        {
            if (IsMainLogEnabled())
            {
                Debug.LogWarning($"[MainJumpScareService] Main jump scare [{id}] is already finished.", _logContext);
            }

            return;
        }

        _playingMainJumpScareIds.Add(id);

        if (IsMainLogEnabled())
        {
            Debug.Log($"[MainJumpScareService] Execute request for [{id}].", _logContext);
        }

        mainJumpScare.Execute();
    }

    public void SetMainJumpScareCanActive(string id, bool canActive)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            Debug.LogError("[MainJumpScareService] Main jump scare id is required.", _logContext);
            return;
        }

        if (_mainJumpScaresById.TryGetValue(id, out MainJumpScareBase mainJumpScare) == false)
        {
            Debug.LogError($"[MainJumpScareService] Main jump scare [{id}] was not found.", _logContext);
            return;
        }

        mainJumpScare.SetCanActive(canActive);

        if (IsMainLogEnabled())
        {
            Debug.Log($"[MainJumpScareService] Main jump scare [{id}] active changed to {canActive}.", _logContext);
        }
    }

    public void NotifyMainJumpScareFinished(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            Debug.LogError("[MainJumpScareService] Main jump scare id is required.", _logContext);
            return;
        }

        if (_mainJumpScaresById.ContainsKey(id) == false)
        {
            Debug.LogError($"[MainJumpScareService] Main jump scare [{id}] was not registered.", _logContext);
            return;
        }

        if (_playingMainJumpScareIds.Contains(id) == false)
        {
            Debug.LogWarning($"[MainJumpScareService] Main jump scare [{id}] was not tracked as playing.", _logContext);
            return;
        }

        _playingMainJumpScareIds.Remove(id);

        if (IsMainLogEnabled())
        {
            Debug.Log(
                $"[MainJumpScareService] Finished [{id}]. Remaining count: {_playingMainJumpScareIds.Count}.",
                _logContext);
        }
    }

    private void RegisterMainJumpScare(MainJumpScareBase mainJumpScare)
    {
        string id = mainJumpScare.Id;
        if (string.IsNullOrWhiteSpace(id))
        {
            Debug.LogError($"[MainJumpScareService] {mainJumpScare.name} has an empty id.", mainJumpScare);
            return;
        }

        if (_mainJumpScaresById.ContainsKey(id))
        {
            Debug.LogError(
                $"[MainJumpScareService] Duplicate main jump scare id [{id}] was found on {mainJumpScare.name}.",
                mainJumpScare);
            return;
        }

        _mainJumpScaresById.Add(id, mainJumpScare);

        if (IsMainLogEnabled())
        {
            Debug.Log($"[MainJumpScareService] Registered [{id}] -> {mainJumpScare.name}.", mainJumpScare);
        }
    }

    private bool IsMainLogEnabled()
    {
        if (_owner == null)
        {
            return false;
        }

        return _owner.EnableMainLog;
    }

    private bool ShouldIncludeInactiveOnRegister()
    {
        if (_owner == null)
        {
            return false;
        }

        return _owner.IncludeInactiveOnRegister;
    }
}
