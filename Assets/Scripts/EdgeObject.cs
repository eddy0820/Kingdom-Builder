using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeObject : MonoBehaviour 
{
    [SerializeField] private EdgeObjectSO edgeObjectSO;

    public string GetEdgeObjectSOName() 
    {
        return edgeObjectSO.name;
    }
}
