using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Gameplay Settings", menuName = "GameSettings/Gameplay/Gameplay Settings")]
public class GameplaySettingsSO : ScriptableObject
{
    [SerializeField] Vector3 globalGravity = new Vector3(0, -30f, 0);
    public Vector3 GlobalGravity => globalGravity;
}
