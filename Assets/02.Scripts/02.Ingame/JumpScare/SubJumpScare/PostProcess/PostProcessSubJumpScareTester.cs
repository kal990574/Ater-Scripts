using Sirenix.OdinInspector;
using UnityEngine;

public class PostProcessSubJumpScareTester : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PostProcessSubJumpScareExecutor _executor;

    [Header("Test Definition")]
    [SerializeField] private PostProcessSubJumpScareDefinitionSO _testDefinition;

    [Header("Optional Input")]
    [SerializeField] private bool _useKeyboardInput = true;
    [SerializeField] private KeyCode _executeKey = KeyCode.T;
    [SerializeField] private KeyCode _stopKey = KeyCode.Y;

    private void Update()
    {
        if (_useKeyboardInput == false)
        {
            return;
        }

        if (Input.GetKeyDown(_executeKey) == true)
        {
            ExecuteTest();
        }

        if (Input.GetKeyDown(_stopKey) == true)
        {
            StopTest();
        }
    }

    [Button("테스트 실행")]
    public void ExecuteTest()
    {
        if (_executor == null)
        {
            Debug.LogWarning("[PostProcessSubJumpScareTester] Executor가 할당되지 않았습니다.", this);
            return;
        }

        if (_testDefinition == null)
        {
            Debug.LogWarning("[PostProcessSubJumpScareTester] Test Definition이 할당되지 않았습니다.", this);
            return;
        }

        bool isSuccess = _executor.TryExecute(_testDefinition);

        Debug.Log(
            $"[PostProcessSubJumpScareTester] Execute 결과: {isSuccess}, Definition: {_testDefinition.name}",
            this
        );
    }

    [Button("테스트 중지")]
    public void StopTest()
    {
        if (_executor == null)
        {
            Debug.LogWarning("[PostProcessSubJumpScareTester] Executor가 할당되지 않았습니다.", this);
            return;
        }

        _executor.StopCurrentEffect();
        Debug.Log("[PostProcessSubJumpScareTester] 현재 포스트프로세스 효과를 중지했습니다.", this);
    }
}