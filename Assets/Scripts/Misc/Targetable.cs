using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Targetable : MonoBehaviour
{
    [SerializeField] Transform lockOnLocation;
    public Transform LockOnLocation => lockOnLocation;
}
