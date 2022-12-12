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

                if(PlayerSpawner.Instance.GridBuildingInfo.EnableStrictPlacement && Mouse3D.Instance.GetMouseWorldLayer() == LayerMask.NameToLayer("Placeable Collider") && Mouse3D.Instance.GetMouseGameObject().GetComponentInParent<GridObject>() != null)
                {
                    PlaceableColliderPosition[] list = Mouse3D.Instance.GetMouseGameObject().GetComponentsInChildren<PlaceableColliderPosition>();
                    
                    Direction targetDirection = GetGhostPlaceableColliderDirection();

                    foreach(PlaceableColliderPosition b in list)
                    { 
                        if(b.PosDir == targetDirection)
                        {
                            targetPosition = b.transform.position;
                            break;
                        }
                    }
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

    public Direction GetGhostPlaceableColliderDirection()
    {
        Direction targetDirection = GridBuildingManager.Instance.CurrentDirection;

        switch(Mouse3D.Instance.GetMouseGameObject().GetComponentInParent<GridObject>().Direction)
        {
            case Direction.Up:
                switch(GridBuildingManager.Instance.CurrentDirection)
                {
                    case Direction.Up:
                        targetDirection = Direction.Down;
                        break;
                    case Direction.Down:
                        targetDirection = Direction.Up;
                        break;
                    case Direction.Left:
                        // NOTHING
                        break;
                    case Direction.Right:
                        // NOTHING
                        break;
                }
                break;
            case Direction.Down:
                switch(GridBuildingManager.Instance.CurrentDirection)
                {
                    case Direction.Up:
                        // NOTHING
                        break;
                    case Direction.Down:
                        // NOTHING
                        break;
                    case Direction.Left:
                        targetDirection = Direction.Right;
                        break;
                    case Direction.Right:
                        targetDirection = Direction.Left;
                        break;
                }
                break;
            case Direction.Left:
                switch(GridBuildingManager.Instance.CurrentDirection)
                {
                    case Direction.Up:
                        targetDirection = Direction.Right;
                        break;
                    case Direction.Down:
                        targetDirection = Direction.Left;
                        break;
                    case Direction.Left:
                        targetDirection = Direction.Down;
                        break;
                    case Direction.Right:
                        targetDirection = Direction.Up;
                        break;
                }
                break;
            case Direction.Right:
                switch(GridBuildingManager.Instance.CurrentDirection)
                {
                    case Direction.Up:
                        targetDirection = Direction.Left;
                        break;
                    case Direction.Down:
                        targetDirection = Direction.Right;
                        break;
                    case Direction.Left:
                        targetDirection = Direction.Up;
                        break;
                    case Direction.Right:
                        targetDirection = Direction.Down;
                        break;
                }
                break;
        }

        return targetDirection;
    }

    public enum GhostValidityState
    {
        Valid,
        Invalid,
        FarAway
    }

}



