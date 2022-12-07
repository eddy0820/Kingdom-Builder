using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Edge Object Type", menuName = "Building System/Edge Object Type")]
public class EdgeObjectSO : ScriptableObject 
{
    public Transform prefab;
    public Transform visual;
}
