using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlaceableObject : MonoBehaviour
{
    [SerializeField] Transform centerPivot;
    public Transform CenterPivot => centerPivot;
    public abstract void DestroySelf();
}
