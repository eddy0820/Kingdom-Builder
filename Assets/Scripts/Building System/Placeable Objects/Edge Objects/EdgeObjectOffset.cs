using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeObjectOffset : MonoBehaviour
{
    [SerializeField] Offset defaultOffset;
    [SerializeField] Offset flippedOffset;
    // flip anchor

    public virtual void ChangeOffset(bool offsetMode)
    {
        if(!offsetMode)
        {
            transform.localPosition = defaultOffset.Position;
            transform.localRotation = Quaternion.Euler(defaultOffset.Rotation.x, defaultOffset.Rotation.y, defaultOffset.Rotation.z);
        }
        else
        {
            transform.localPosition = flippedOffset.Position;
            transform.localRotation = Quaternion.Euler(flippedOffset.Rotation.x, flippedOffset.Rotation.y, flippedOffset.Rotation.z);
        }
    }

    [System.Serializable]
    public struct Offset
    {
        [SerializeField] Vector3 position;
        public Vector3 Position => position;
        [SerializeField] Vector3 rotation;
        public Vector3 Rotation => rotation;
    }
}