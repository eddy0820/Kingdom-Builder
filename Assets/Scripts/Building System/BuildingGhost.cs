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

    GridObjectBuildingGhost gridObjectBuildingGhost;
    public GridObjectBuildingGhost GridObjectBuildingGhost => gridObjectBuildingGhost;
    EdgeObjectBuildingGhost edgeObjectBuildingGhost;
    public EdgeObjectBuildingGhost EdgeObjectBuildingGhost => edgeObjectBuildingGhost;
    LooseObjectBuildingGhost looseObjectBuildingGhost;
    public LooseObjectBuildingGhost LooseObjectBuildingGhost => looseObjectBuildingGhost;

    AbstractPlaceableObjectBuildingGhost currentBuildingGhost;
    public AbstractPlaceableObjectBuildingGhost CurrentBuildingGhost => currentBuildingGhost;

    private void Awake()
    {
        gridObjectBuildingGhost = GetComponentInChildren<GridObjectBuildingGhost>();
        edgeObjectBuildingGhost = GetComponentInChildren<EdgeObjectBuildingGhost>();
        looseObjectBuildingGhost = GetComponentInChildren<LooseObjectBuildingGhost>();
    }

    private void Start() 
    {
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
            if(!GridBuildingManager.Instance.IsWithinMaxBuildDistance())
            {
                SetGhostValidityState(GhostValidityState.FarAway);
            }
            else if(!GridBuildingManager.Instance.CurrentBuildingManager.CanPlace())
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
            switch(GridBuildingManager.Instance.CurrentPlaceableObjectSO.ObjectType)
            {
                case PlaceableObjectTypes.GridObject:
                    
                    currentBuildingGhost.DoVisibleGhostMovement(visual);
                    currentBuildingGhost.DoFakeGhostMovement(fakeVisual);

                break;

                case PlaceableObjectTypes.EdgeObject:
                    currentBuildingGhost.DoVisibleGhostMovement(visual);
                    currentBuildingGhost.DoFakeGhostMovement(fakeVisual);
                break;

                case PlaceableObjectTypes.StairEdgeObject:
                    StairEdgePosition stairEdgePosition = GridBuildingManager.Instance.GetMouseStairEdgePosition();
                    if(stairEdgePosition != null) 
                    {
                        visual.transform.position = Vector3.Lerp(visual.transform.position, stairEdgePosition.transform.GetChild(0).position, Time.deltaTime * 15f);
                        visual.transform.rotation = Quaternion.Lerp(visual.transform.rotation, stairEdgePosition.transform.GetChild(0).rotation, Time.deltaTime * 25f);

                        //fakeVisual.transform.position = visual.transform.position;
                        //fakeVisual.transform.rotation = visual.transform.rotation;
                    } 
                    else
                    {
                        visual.transform.position = Vector3.Lerp(visual.transform.position, Mouse3D.Instance.GetMouseWorldPosition(), Time.deltaTime * 15f);
                        visual.transform.rotation = Quaternion.Lerp(visual.transform.rotation, Quaternion.identity, Time.deltaTime * 25f);

                        //fakeVisual.transform.position = visual.transform.position;
                        //fakeVisual.transform.rotation = visual.transform.rotation;
                    }
                break;

                case PlaceableObjectTypes.LooseObject:
                    currentBuildingGhost.DoVisibleGhostMovement(visual);
                    currentBuildingGhost.DoFakeGhostMovement(fakeVisual);
                break;
            }

            lastBuildingGhostPos = visual.transform.position;
            lastBuildingGhostRot = visual.transform.rotation;
        }
    }

    public void RefreshVisual() 
    {
        if(PlayerController.Instance.BuildModeEnabled && GridBuildingManager.Instance.CurrentPlaceableObjectSO != null)
        {
            if(visual != null) 
            {
                Destroy(visual.gameObject);
                visual = null;

                Destroy(fakeVisual.gameObject);
                fakeVisual = null;
            }

            PlaceableObjectSO placeableObjectSO = GridBuildingManager.Instance.CurrentPlaceableObjectSO;

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
            // add more debug stuff
            //DisableMeshRendererRecursive(fakeVisual.gameObject);
            SetLayerAndMatRecursive(fakeVisual.gameObject, farAwayGhostMaterial, ignoreMaskName);

            if(currentBuildingGhost == gridObjectBuildingGhost || currentBuildingGhost == edgeObjectBuildingGhost ||
             currentBuildingGhost == looseObjectBuildingGhost) currentBuildingGhost.RemoveColliderScriptFromVisibleGhost();
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
        switch(GridBuildingManager.Instance.CurrentPlaceableObjectSO.ObjectType)
        {
            case PlaceableObjectTypes.GridObject:
                currentBuildingGhost = gridObjectBuildingGhost;
            break;

            case PlaceableObjectTypes.EdgeObject:
                currentBuildingGhost = edgeObjectBuildingGhost;
            break;

            case PlaceableObjectTypes.StairEdgeObject:
                
            break;

            case PlaceableObjectTypes.LooseObject:
                currentBuildingGhost = looseObjectBuildingGhost;
            break;
        }

        RefreshVisual();
    }

    public enum GhostValidityState
    {
        Valid,
        Invalid,
        FarAway
    }

}



