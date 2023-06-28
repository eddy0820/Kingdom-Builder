using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGhost : MonoBehaviour {

    [SerializeField] Material validGhostMaterial;
    [SerializeField] Material inValidGhostMaterial;
    [SerializeField] Material farAwayGhostMaterial;
    [SerializeField] string ignoreMaskName;
    [SerializeField] string ignoreCollisionMaskName;

    Transform visual;
    public Transform Visual => visual;
    Transform fakeVisual;
    public Transform FakeVisual => fakeVisual;

    Material currentGhostMaterial;

    Vector3 lastBuildingGhostPos;
    Quaternion lastBuildingGhostRot;

    GridBuildingManager gridBuildingManager;

    GridObjectBuildingGhost gridObjectBuildingGhost;
    public GridObjectBuildingGhost GridObjectBuildingGhost => gridObjectBuildingGhost;
    EdgeObjectBuildingGhost edgeObjectBuildingGhost;
    public EdgeObjectBuildingGhost EdgeObjectBuildingGhost => edgeObjectBuildingGhost;
    LooseObjectBuildingGhost looseObjectBuildingGhost;
    public LooseObjectBuildingGhost LooseObjectBuildingGhost => looseObjectBuildingGhost;

    AbstractPlaceableObjectBuildingGhost currentBuildingGhost;
    public AbstractPlaceableObjectBuildingGhost CurrentBuildingGhost => currentBuildingGhost;

    bool debug;
    bool enableFakeVisualDebug;
    Material fakeVisualMaterial;
    List<PlaceableObjectTypes> placeableObjectTypesFakeVisualBlacklist;
    List<BuildingTypes> buildingTypesFakeVisualBlacklist;

    private void Awake()
    {
        gridBuildingManager = GridBuildingManager.Instance;

        gridObjectBuildingGhost = GetComponentInChildren<GridObjectBuildingGhost>();
        edgeObjectBuildingGhost = GetComponentInChildren<EdgeObjectBuildingGhost>();
        looseObjectBuildingGhost = GetComponentInChildren<LooseObjectBuildingGhost>();
    }

    private void Start() 
    {
        // This has to be in Start() because it needs to take place after Init() in the GridBuildingManager
        gridBuildingManager.GetFakeVisualDebugInfo(out debug, out enableFakeVisualDebug, out fakeVisualMaterial, out placeableObjectTypesFakeVisualBlacklist, out buildingTypesFakeVisualBlacklist);
        
        currentGhostMaterial = validGhostMaterial;

        RefreshVisual();
    }

    private void Update()
    {
        if(!PlayerController.Instance.UICanvas.BuildMenuEnabled)
        {
            HandleRefreshVisual();
            HandleGhostValidityState();
        }
    }

    private void HandleRefreshVisual()
    {
        if(Mouse3D.Instance.AmILookingAtCollider())
        {
            if(visual == null)
            {
                RefreshVisual();
            }
            
        }
        else
        {
            if(visual != null)
            {
                Destroy(visual.gameObject);
                visual = null;

                Destroy(fakeVisual.gameObject);
                fakeVisual = null;
            }
        }
    }

    private void HandleGhostValidityState()
    {
        if(visual != null)
        {
            if(!gridBuildingManager.IsWithinMaxBuildDistance())
            {
                SetGhostValidityState(GhostValidityState.FarAway);
            }
            else if(!gridBuildingManager.CurrentBuildingManager.CanPlace())
            {
                SetGhostValidityState(GhostValidityState.Invalid);
            }
            else
            {
                SetGhostValidityState(GhostValidityState.Valid);
            }
        }
    }

    private void LateUpdate() 
    {
        if(visual != null && !PlayerController.Instance.UICanvas.BuildMenuEnabled)
        {
            currentBuildingGhost.DoVisibleGhostMovement(visual);
            currentBuildingGhost.DoFakeGhostMovement(fakeVisual);

            lastBuildingGhostPos = visual.transform.position;
            lastBuildingGhostRot = visual.transform.rotation;
        }
    }

    public void RefreshVisual() 
    {
        if(PlayerController.Instance.BuildModeEnabled && gridBuildingManager.CurrentPlaceableObjectSO != null)
        {
            if(visual != null) 
            {
                Destroy(visual.gameObject);
                visual = null;

                Destroy(fakeVisual.gameObject);
                fakeVisual = null;
            }

            PlaceableObjectSO placeableObjectSO = gridBuildingManager.CurrentPlaceableObjectSO;

            Vector3 spawnPos;
            Quaternion spawnRot;

            if(lastBuildingGhostPos == Vector3.zero)
            {
                spawnPos = Vector3.zero;
            }
            else
            {
                spawnPos = lastBuildingGhostPos;
            }

            if(lastBuildingGhostRot == Quaternion.identity)
            {
                spawnRot = Quaternion.identity;
            }
            else
            {
                spawnRot = lastBuildingGhostRot;
            }

            visual = Instantiate(placeableObjectSO.Visual, spawnPos, spawnRot);
            visual.parent = transform;
            visual.localPosition = spawnPos;
            visual.localEulerAngles = spawnRot.eulerAngles;
            SetLayerAndMatRecursive(visual.gameObject, currentGhostMaterial, ignoreMaskName);

            fakeVisual = Instantiate(visual, visual.position, visual.rotation);
            fakeVisual.parent = transform;
            fakeVisual.localPosition = spawnPos;
            fakeVisual.localEulerAngles = spawnRot.eulerAngles;
            SetupFakeVisualDebug();
        
            currentBuildingGhost.RemoveColliderScriptFromVisibleGhost();
        }
        else
        {
            if(visual != null)
            {
                Destroy(visual.gameObject);
                visual = null;

                Destroy(fakeVisual.gameObject);
                fakeVisual = null;
            }
        }
        
    }

    private void SetLayerAndMatRecursive(GameObject targetGameObject, Material mat, string layerName) 
    {
        MeshRenderer meshRenderer;
        targetGameObject.TryGetComponent<MeshRenderer>(out meshRenderer);

        if(meshRenderer != null)
        {
            meshRenderer.material = mat;
        }

        targetGameObject.layer = LayerMask.NameToLayer(layerName);
        
        foreach(Transform child in targetGameObject.transform) 
        {
            SetLayerAndMatRecursive(child.gameObject, mat, layerName);
        }
    }  

    private void SetMatRecursive(GameObject targetGameObject, Material mat)
    {
        MeshRenderer meshRenderer;
        targetGameObject.TryGetComponent<MeshRenderer>(out meshRenderer);

        if(meshRenderer != null)
        {
            meshRenderer.material = mat;
        }

        foreach(Transform child in targetGameObject.transform) 
        {
            SetMatRecursive(child.gameObject, mat);
        }
    }

    private void DisableMeshRendererRecursive(GameObject targetGameObject)
    {
        MeshRenderer meshRenderer;
        targetGameObject.TryGetComponent<MeshRenderer>(out meshRenderer);

        if(meshRenderer != null)
        {
            meshRenderer.enabled = false;
        }

        foreach(Transform child in targetGameObject.transform) 
        {
            DisableMeshRendererRecursive(child.gameObject);
        }
    }

    private void SetGhostValidityState(GhostValidityState state)
    {
        switch(state)
        {
            case GhostValidityState.Valid:
                currentGhostMaterial = validGhostMaterial;
            break;

            case GhostValidityState.Invalid:
                currentGhostMaterial = inValidGhostMaterial;
            break;

            case GhostValidityState.FarAway:
                currentGhostMaterial = farAwayGhostMaterial;
            break;
        }

        SetMatRecursive(visual.gameObject, currentGhostMaterial);
    }
    
    public void SwitchBuildingGhost()
    {
        switch(gridBuildingManager.CurrentPlaceableObjectSO.ObjectType)
        {
            case PlaceableObjectTypes.GridObject:
                currentBuildingGhost = gridObjectBuildingGhost;
            break;

            case PlaceableObjectTypes.EdgeObject:
                currentBuildingGhost = edgeObjectBuildingGhost;
            break;

            case PlaceableObjectTypes.LooseObject:
                currentBuildingGhost = looseObjectBuildingGhost;
            break;
        }

        RefreshVisual();
    }

    private void SetupFakeVisualDebug()
    {
        if(debug && enableFakeVisualDebug)
        {
            bool doDebug = true;

            foreach(PlaceableObjectTypes placeableObjectType in placeableObjectTypesFakeVisualBlacklist)
            {
                if(gridBuildingManager.CurrentPlaceableObjectSO.ObjectType == placeableObjectType)
                {
                    doDebug = false;
                    break;
                }
            }

            if(doDebug && gridBuildingManager.CurrentPlaceableObjectSO is PlaceableGridObjectSO)
            {
                PlaceableGridObjectSO currentPlaceableGridObjectSO = gridBuildingManager.CurrentPlaceableObjectSO as PlaceableGridObjectSO;

                foreach(BuildingTypes buildingType in buildingTypesFakeVisualBlacklist)
                {
                    if(currentPlaceableGridObjectSO.BuildingType == buildingType)
                    {
                        doDebug = false;
                        break;
                    }
                }
            }

            if(doDebug) 
            {
                SetLayerAndMatRecursive(fakeVisual.gameObject, fakeVisualMaterial, ignoreMaskName);
                return;
            }
        }

        DisableMeshRendererRecursive(fakeVisual.gameObject);
    }

    public enum GhostValidityState
    {
        Valid,
        Invalid,
        FarAway
    }

}



