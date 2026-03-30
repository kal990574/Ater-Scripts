using System.Globalization;
using TMPro;
using UnityEngine;

public class UI_ExplainText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _explainText;
    [SerializeField] private ExplainDataSO _explainData;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            Show("press_key_exit");
        if (Input.GetKeyDown(KeyCode.S))
            Hide();
    }

    public void Show(string id)
    {
        ExplainData data = _explainData.GetExplainData(id);

        if(data == null) return;

        _explainText.text = data.ExplainText;
        _explainText.enabled = true;
    }

    public void Hide()
    {
        _explainText.enabled = false;
    }
}
