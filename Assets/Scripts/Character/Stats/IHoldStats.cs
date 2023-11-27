using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHoldStats
{
    public CharacterStats Stats { get; }
    public IDamageable IDamageable { get; }
}
