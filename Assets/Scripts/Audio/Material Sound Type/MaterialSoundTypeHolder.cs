using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSoundTypeHolder : MonoBehaviour, IMateralSoundType
{
    [SerializeField] EMaterialSoundType materialSoundType;

    public EMaterialSoundType GetMaterialSoundType()
    {
        return materialSoundType;
    }

    public void SetMaterialSoundType(EMaterialSoundType _materialSoundType)
    {
        materialSoundType = _materialSoundType;
    }
}
