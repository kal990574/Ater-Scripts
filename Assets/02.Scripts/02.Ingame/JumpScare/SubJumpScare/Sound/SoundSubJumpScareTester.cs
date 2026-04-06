using Sirenix.OdinInspector;
using UnityEngine;

public class SoundSubJumpScareTester : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SoundSubJumpScareExecutor _executor;
    [SerializeField] private Transform _playerRootTransform;
    [SerializeField] private Transform _playerCameraTransform;

    [Header("Test Data")]
    [SerializeField] private SoundSubJumpScareDefinitionSO _definition;

    [Header("Debug")]
    [SerializeField] private bool _enableDebugLog = true;

    [Button("Test Execute")]
    public void TestExecute()
    {
        if (_executor == null)
        {
            Debug.LogWarning("[SoundSubJumpScareTester] Executor가 비어 있습니다.", this);
            return;
        }

        if (SoundManager.Instance == null)
        {
            Debug.LogWarning("[SoundSubJumpScareTester] SoundManager.Instance가 없습니다.", this);
            return;
        }

        SoundJumpScareExecutionRequest request = new SoundJumpScareExecutionRequest(
            _definition,
            _playerRootTransform,
            _playerCameraTransform,
            SoundManager.Instance);

        SoundJumpScareExecutionResult result = _executor.TryExecute(request);

        if (_enableDebugLog == true)
        {
            Debug.Log(string.Format(
                    "[SoundSubJumpScareTester] Result | Success={0}, Reason={1}",
                    result.IsSuccess,
                    result.Reason),
                this);
        }
    }
}