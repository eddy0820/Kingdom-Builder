using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MovementSpeedSettings
{
    [SerializeField] float speed;
    public float Speed => speed;

    [SerializeField] float acceleration;
    public float Acceleration => acceleration;
}
