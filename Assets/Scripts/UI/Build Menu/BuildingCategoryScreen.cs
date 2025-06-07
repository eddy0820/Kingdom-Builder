using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingCategoryScreen : MonoBehaviour
{
    [SerializeField] RectTransform subScreensParent;
    [SerializeField] GameObject buildingTypeGridHolderPrefab;

    public void HouseBuildingInit(List<PlaceableGridObjectSO> placeableObjects)
    {
        GameObject buildingTypeGridHolder = Instantiate(buildingTypeGridHolderPrefab, buildingTypeGridHolderPrefab.transform.localPosition, buildingTypeGridHolderPrefab.transform.rotation, transform);
        buildingTypeGridHolder.GetComponent<RectTransform>().anchoredPosition = new Vector2(25, 0);
//        buildingTypeGridHolder.GetComponent<BuildingTypesInterface>().Init(placeableObjects, subScreensParent);
    }

    public void PropInit(List<PlaceableLooseObjectSO> loosePlaceableObjects)
    {
    
    }
}
