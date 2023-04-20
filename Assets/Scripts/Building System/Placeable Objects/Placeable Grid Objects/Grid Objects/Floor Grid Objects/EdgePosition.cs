using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgePosition : MonoBehaviour 
{
    public Edge edge;

    [SerializeField] Transform pivotTransform;
    public Transform PivotTransform => pivotTransform;
}
