using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingCategoryScreen : MonoBehaviour
{
    [SerializeField] RectTransform subScreensParent;

    public void HouseBuildingInit(List<GridPlaceableObjectSO> placeableObjects, GameObject buildingTypeButtonPrefab, GameObject buildingTypeScreenPrefab, GameObject buildingTypeGridHolderPrefab)
    {
        GameObject buildingTypeGridHolder = Instantiate(buildingTypeGridHolderPrefab, buildingTypeGridHolderPrefab.transform.localPosition, buildingTypeGridHolderPrefab.transform.rotation, transform);
        buildingTypeGridHolder.GetComponent<RectTransform>().anchoredPosition = new Vector2(25, 0);
        buildingTypeGridHolder.GetComponent<BuildingTypesInterface>().Init(buildingTypeButtonPrefab, buildingTypeScreenPrefab, subScreensParent);
    }

    public void PropInit(List<LooseObjectSO> loosePlaceableObjects)
    {
    
    }
}
