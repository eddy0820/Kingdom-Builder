using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractPlaceableObjectBuildingGhost : MonoBehaviour
{
    BuildingGhost buildingGhost;
    public BuildingGhost BuildingGhost => buildingGhost;
   
    private void Awake()
    {
        buildingGhost = GetComponentInParent<BuildingGhost>();
        OnAwake();
    }

    protected abstract void OnAwake();

    public abstract void DoVisibleGhostMovement(Transform visual);

    public abstract void DoFakeGhostMovement(Transform fakeVisual);

    public abstract void RemoveColliderScriptFromVisibleGhost();
    
}
