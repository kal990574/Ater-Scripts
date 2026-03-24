using UnityEngine;

[CreateAssetMenu(fileName = "RaycastConfig", menuName = "Ater/Interact/RaycastConfig")]
public class InteractRaycastConfigSO : ScriptableObject
{
    [SerializeField] private RaycastSetting _query = new(5f, ~0, QueryTriggerInteraction.Ignore);

    public RaycastSetting Query => _query;
}
