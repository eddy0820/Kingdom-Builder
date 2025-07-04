using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgePosition : MonoBehaviour 
{
    [SerializeField] Edge edge;
    public Edge Edge => edge;

    [SerializeField] Transform pivotTransform;
    public Transform PivotTransform => pivotTransform;
}
