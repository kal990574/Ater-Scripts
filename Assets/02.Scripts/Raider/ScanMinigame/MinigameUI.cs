using UnityEngine;

public class MinigameUI : MonoBehaviour
{
    [SerializeField] private GameObject _root;
    [SerializeField] private RectTransform _needle;
    [SerializeField] private RectTransform _successZone;
    [SerializeField] private RectTransform _greatZone;

    public void Show()
    {
        if (_root != null)
        {
            _root.SetActive(true);
        }
    }

    public void Hide()
    {
        if (_root != null)
        {
            _root.SetActive(false);
        }
    }

    public void SetNeedleAngle(float angle)
    {
        if (_needle != null)
        {
            _needle.localRotation = Quaternion.Euler(0.0f, 0.0f, -angle);
        }
    }

    public void SetZones(float successStart, float successEnd, float greatStart, float greatEnd)
    {
        SetZoneRotation(_successZone, successStart, successEnd);
        SetZoneRotation(_greatZone, greatStart, greatEnd);
    }

    private void SetZoneRotation(RectTransform target, float startAngle, float endAngle)
    {
        if (target == null)
        {
            return;
        }

        float size = GetArcSize(startAngle, endAngle);
        float center = GetArcCenter(startAngle, endAngle);

        target.localRotation = Quaternion.Euler(0.0f, 0.0f, -center);

        Vector2 currentSize = target.sizeDelta;
        currentSize.x = size;
        target.sizeDelta = currentSize;
    }

    private float GetArcSize(float startAngle, float endAngle)
    {
        if (startAngle <= endAngle)
        {
            return endAngle - startAngle;
        }

        return (360.0f - startAngle) + endAngle;
    }

    private float GetArcCenter(float startAngle, float endAngle)
    {
        float size = GetArcSize(startAngle, endAngle);
        return NormalizeAngle(startAngle + (size * 0.5f));
    }

    private float NormalizeAngle(float angle)
    {
        angle %= 360.0f;

        if (angle < 0.0f)
        {
            angle += 360.0f;
        }

        return angle;
    }
}
