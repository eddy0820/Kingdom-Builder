using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingCategoryScreen : MonoBehaviour
{
    public void HouseBuildingInit(List<GridPlaceableObjectSO> placeableObjects)
    {
        foreach(BuildingTypesSO buildingTypeSO in PlayerSpawner.Instance.GridBuildingInfo.BuildingTypesDatabase.BuildingTypes)
        {
            
        }
    }

    public void PropInit(List<LooseObjectSO> loosePlaceableObjects)
    {
    
    }
}
