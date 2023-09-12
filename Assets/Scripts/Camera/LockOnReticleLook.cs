using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOnReticleLook : MonoBehaviour
{
    private void Update()
    {
        transform.LookAt(Camera.main.transform.position);
        transform.Rotate(0, 0, 45);
    }
}
