using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractPlaceableObjectBuildingManager : MonoBehaviour
{
    GridBuildingManager gridBuildingManager;
    protected GridBuildingManager GridBuildingManager => gridBuildingManager;

    private void Awake()
    {
        gridBuildingManager = GetComponentInParent<GridBuildingManager>();
        OnAwake();
    }

    protected abstract void OnAwake();

    public abstract void PlaceObject();
    public abstract bool CanPlace();

    public abstract void Rotate();

    public abstract void Demolish(PlaceableObject placeableObject);
}
