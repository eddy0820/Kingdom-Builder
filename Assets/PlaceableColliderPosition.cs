using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableColliderPosition : MonoBehaviour
{
    [SerializeField] Direction posDir;
    public Direction PosDir => posDir;
}
