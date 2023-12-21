using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EWeaponTypeExtensions
{
    public static EAnimatorWeaponType ToAnimatorWeaponType(this EWeaponTypes weaponType)
    {
        // need to extend this more when adding new weapon types
        return (EAnimatorWeaponType) weaponType;
    }
}
