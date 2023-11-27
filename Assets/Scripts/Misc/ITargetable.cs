using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface ITargetable
{
    public Transform LockOnLocation { get; }

    public string GetDamageableName();
}
