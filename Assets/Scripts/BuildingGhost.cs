using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGhost : MonoBehaviour {

    [SerializeField] Material validGhostMaterial;
    [SerializeField] Material inValidGhostMaterial;
    [SerializeField] Material farAwayGhostMaterial;
    [SerializeField] string ignoreMaskName;
    Transform visual;
    Material currentGhostMaterial;
    GridBuildingInfo gridBuildingInfo;

    bool doOverlapBox;

    private void Awake()
    {
        gridBuildingInfo = PlayerSpawner.Instance.GridBuildingInfo;
    }
    private void Start() 
    {
        currentGhostMaterial = validGhostMaterial;

        RefreshVisual();
    }

    private void Update()
    {
        if(GridBuildingManager.Instance.AmILookingAtCollider())
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
            }
        }

        if(visual != null)
        {
            if(Vector3.Distance(PlayerController.Instance.Character.transform.position, Mouse3D.Instance.GetMouseWorldPosition()) > gridBuildingInfo.MaxBuildDistance)
            {
                SetGhostValidityState(GhostValidityState.FarAway);
            }
            else if(!GridBuildingManager.Instance.CanPlaceObject())
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
        Vector3 targetPosition = GridBuildingManager.Instance.GetMouseWorldSnappedPosition();

        switch(GridBuildingManager.Instance.CurrentPlaceableObjectSO.ObjectType)
        {
            case PlaceableObjectTypes.GridObject:

                Vector3 newPos = GridBuildingManager.Instance.GetNewGridObjectPosition();

                if(newPos != Vector3.zero)
                {
                    targetPosition = newPos;
                    doOverlapBox = false;
                }
                else
                {
                    doOverlapBox = true;
                }
                
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 15f);
                transform.rotation = Quaternion.Lerp(transform.rotation, GridBuildingManager.Instance.GetGridObjectRotation(), Time.deltaTime * 15f);
            break;

            case PlaceableObjectTypes.EdgeObject:
                EdgePosition edgePosition = GridBuildingManager.Instance.GetMouseEdgePosition();
                if(edgePosition != null) 
                {
                    transform.position = Vector3.Lerp(transform.position, edgePosition.transform.position, Time.deltaTime * 15f);
                    transform.rotation = Quaternion.Lerp(transform.rotation, edgePosition.transform.rotation, Time.deltaTime * 25f);
                } 
                else
                {
                    transform.position = Vector3.Lerp(transform.position, Mouse3D.Instance.GetMouseWorldPosition(), Time.deltaTime * 15f);
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, Time.deltaTime * 25f);
                }
            break;

            case PlaceableObjectTypes.LooseObject:
                transform.position = Vector3.Lerp(transform.position, Mouse3D.Instance.GetMouseWorldPosition(), Time.deltaTime * 15f);
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, GridBuildingManager.Instance.LooseObjectEulerY, 0), Time.deltaTime * 25f);
            break;
        }
        
    }

    public void RefreshVisual() 
    {
        if(PlayerController.Instance.BuildModeEnabled)
        {
            if (visual != null) 
            {
                Destroy(visual.gameObject);
                visual = null;
            }

            PlaceableObjectSO placeableObjectSO = GridBuildingManager.Instance.CurrentPlaceableObjectSO;

            visual = Instantiate(placeableObjectSO.Visual, Vector3.zero, Quaternion.identity);
            visual.parent = transform;
            visual.localPosition = Vector3.zero;
            visual.localEulerAngles = Vector3.zero;
            SetLayerAndMatRecursive(visual.gameObject, currentGhostMaterial, ignoreMaskName);
        }
        else
        {
            if(visual != null)
            {
                Destroy(visual.gameObject);
                visual = null;
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

    public bool GetIfGhostisColliding()
    {
        if(visual != null)
        {
            LooseObjectVisual looseObjectVisual = visual.GetComponent<LooseObjectVisual>();

            if(looseObjectVisual.Colliding)
            {
                return true;
            }
        }

        return false;
    }

    public bool GhostOverlapBoxEdgeObject()
    {
        if(doOverlapBox && visual != null && GridBuildingManager.Instance.CurrentPlaceableObjectSO is GridObjectSO)
        {
            BoxCollider boxCollider = visual.GetComponent<BoxCollider>();

            Vector2Int rotationOffset = ((GridObjectSO) GridBuildingManager.Instance.CurrentPlaceableObjectSO).GetRotationOffset(GridBuildingManager.Instance.CurrentDirection);
            Vector2 rotationOffsetV2 = new Vector2(rotationOffset.x, rotationOffset.y);
            
            if(((GridObjectSO) GridBuildingManager.Instance.CurrentPlaceableObjectSO).width != ((GridObjectSO) GridBuildingManager.Instance.CurrentPlaceableObjectSO).height)
            {
                //These are hardcoded but i think they can be derived by diving 0.2 or the whatever the added scale is
                // Up
                if(rotationOffsetV2.x > 0 && rotationOffsetV2.y > 0)
                {
                    rotationOffsetV2.x = rotationOffsetV2.x + 0.5f;
                    rotationOffsetV2.y = rotationOffsetV2.y + 0.25f;
                }
                // Right
                else if(rotationOffsetV2.x > 0)
                {
                    rotationOffsetV2.x = rotationOffsetV2.x + 0.875f;
                    rotationOffsetV2.y = rotationOffsetV2.y - 0.625f;
                }
                // Left
                else if(rotationOffsetV2.y > 0)
                {
                    rotationOffsetV2.x = rotationOffsetV2.x + 0.625f;
                    rotationOffsetV2.y = rotationOffsetV2.y - 0.125f;

                }
            }
            else
            {
                if(rotationOffsetV2.x > 0)
                {
                    rotationOffsetV2.x = rotationOffsetV2.x + 0.4f;
                }
                if(rotationOffsetV2.y > 0)
                {
                    rotationOffsetV2.y = rotationOffsetV2.y + 0.4f;
                }
            }

            Vector3 rotationOffsetV3 = new Vector3(rotationOffsetV2.x, 0, rotationOffsetV2.y);

            Collider[] colliders = Physics.OverlapBox(boxCollider.center + visual.position - rotationOffsetV3, (boxCollider.size / 2) + new Vector3(0.2f, 0.2f, 0.2f), GridBuildingManager.Instance.GetGridObjectRotation(), Mouse3D.Instance.MouseColliderLayerMaskNoPlaceableCollider);
            
            foreach(Collider collider in colliders)
            {
                if(collider.GetComponentInParent<EdgeObject>() != null)
                {
                    return true;
                }
            }
        }

        return false;
    }

    void OnDrawGizmos()
    {
        if(PlayerSpawner.Instance.GridBuildingInfo.Debug && visual != null && GridBuildingManager.Instance.CurrentPlaceableObjectSO is GridObjectSO)
        {
            BoxCollider boxCollider = visual.GetComponent<BoxCollider>();
            Gizmos.color = Color.yellow;

            Vector2Int rotationOffset = ((GridObjectSO) GridBuildingManager.Instance.CurrentPlaceableObjectSO).GetRotationOffset(GridBuildingManager.Instance.CurrentDirection);
            Vector2 rotationOffsetV2 = new Vector2(rotationOffset.x, rotationOffset.y);
            
            if(((GridObjectSO) GridBuildingManager.Instance.CurrentPlaceableObjectSO).width != ((GridObjectSO) GridBuildingManager.Instance.CurrentPlaceableObjectSO).height)
            {
                //These are hardcoded but i think they can be derived by diving 0.2 or the whatever the added scale is
                // Up
                if(rotationOffsetV2.x > 0 && rotationOffsetV2.y > 0)
                {
                    rotationOffsetV2.x = rotationOffsetV2.x + 0.5f;
                    rotationOffsetV2.y = rotationOffsetV2.y + 0.25f;
                }
                // Right
                else if(rotationOffsetV2.x > 0)
                {
                    rotationOffsetV2.x = rotationOffsetV2.x + 0.875f;
                    rotationOffsetV2.y = rotationOffsetV2.y - 0.625f;
                }
                // Left
                else if(rotationOffsetV2.y > 0)
                {
                    rotationOffsetV2.x = rotationOffsetV2.x + 0.625f;
                    rotationOffsetV2.y = rotationOffsetV2.y - 0.125f;

                }
            }
            else
            {
                if(rotationOffsetV2.x > 0)
                {
                    rotationOffsetV2.x = rotationOffsetV2.x + 0.4f;
                }
                if(rotationOffsetV2.y > 0)
                {
                    rotationOffsetV2.y = rotationOffsetV2.y + 0.4f;
                }
            }
            

            

            Vector3 rotationOffsetV3 = new Vector3(rotationOffsetV2.x, 0, rotationOffsetV2.y);

            Matrix4x4 prevMatrix = Gizmos.matrix;
            Gizmos.matrix = transform.localToWorldMatrix; 

            Vector3 pos = boxCollider.center + visual.position - rotationOffsetV3;
            pos = transform.InverseTransformPoint(pos);

            Gizmos.DrawCube(pos, boxCollider.size + new Vector3(0.2f, 0.2f, 0.2f));

            Gizmos.matrix = prevMatrix;
        }
    }

    public enum GhostValidityState
    {
        Valid,
        Invalid,
        FarAway
    }

}



