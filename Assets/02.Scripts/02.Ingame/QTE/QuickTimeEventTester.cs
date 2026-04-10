using UnityEngine;

public class QuickTimeEventTester : MonoBehaviour, IQTEInvoker
{
    [Header("Required References")]
    [SerializeField] private QTEManager _qteManager;
    [SerializeField] private QTEConfigSOBase _config;

    [Header("Input")]
    [SerializeField] private KeyCode _submitKey = KeyCode.Space;
    [SerializeField] private KeyCode _startKey = KeyCode.F;
    [SerializeField] private KeyCode _cancelKey = KeyCode.Escape;
    
    public GameObject Owner =>gameObject;
    private void Update()
    {
        if (Input.GetKeyDown(_startKey))
        {
            StartQte();
        }

        if (_qteManager == null)
        {
            return;
        }

        if (Input.GetKeyDown(_submitKey))
        {
            _qteManager.SubmitCurrent();
        }

        if (Input.GetKeyDown(_cancelKey))
        {
            _qteManager.CancelCurrent();
        }
    }

    private void StartQte()
    {
        if (_qteManager == null || _config == null)
        {
            return;
        }

        _qteManager.TryPlay(this, _config, HandleEnded);
    }

    private void HandleEnded(EQuickTimeEventResult result)
    {
        Debug.Log($"QTE Ended : {_config.QTEType}, Result : {result}");
    }

    

    public void ApplyQTEFailure()
    {
    }

    public void ApplyQTESuccess()
    {
    }

    public void ApplyQTEGreatSuccess()
    {
    }
}