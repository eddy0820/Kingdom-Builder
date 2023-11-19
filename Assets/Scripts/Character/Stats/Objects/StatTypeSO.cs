using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stat Type", menuName = "Stats/Stat Type")]
public class StatTypeSO : ScriptableObject
{
    [SerializeField] new string name = "New Stat Type Name";
    public string Name => name;
    [SerializeField] float defaultValue = 0f;
    public float DefaultValue => defaultValue;

    [Space(15)]
    [SerializeField] float defaultMinValue = -1f;
    public float DefaultMinValue => defaultMinValue;
    [SerializeField] float defaultMaxValue = -1f;
    public float DefaultMaxValue => defaultMaxValue;
}
