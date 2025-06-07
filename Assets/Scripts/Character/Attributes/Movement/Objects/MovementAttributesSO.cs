using UnityEngine;

[CreateAssetMenu(fileName = "New Movement Attributes", menuName = "Attributes/Movement")]
public class MovementAttributesSO : ScriptableObject
{
    [SerializeField] MovementAttributes attributes;
    public MovementAttributes Attributes => attributes;
}
