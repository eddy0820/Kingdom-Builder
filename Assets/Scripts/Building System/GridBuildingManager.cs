using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
////////////// U NEED TO CHECK ALL Physics.Raycast YOU ARE USING THE COLLIDER MASK WRONG
////////////// LOOSE PLACEMENT BUG WHEN MOVING MOUSE FAST COULD BE BECAUSE OF ^^
////////////// Fix being able to place objects that can't be placed for a frame
////////////// fix edge object flipping maybe?
public class GridBuildingManager : MonoBehaviour
{
    public static GridBuildingManager Instance { get; private set; }

    [ReadOnly, SerializeField] List<PlaceableObjectSO> placeableObjectSOList;

    [Space(15)]

    [ReadOnly, SerializeField] int gridWidth = 10;
    [ReadOnly, SerializeField] int gridLength = 10;
    [ReadOnly, SerializeField] float cellSize = 10f;

    [Space(5)]

    [ReadOnly, SerializeField] float gridHeight = 10f;
    [ReadOnly, SerializeField] int gridVerticalCount = 5;

    [Space(5)]

    [ReadOnly, SerializeField] float maxBuildDistance = 15f;

    [Space(15)]

    [ReadOnly, SerializeField] LayerMask edgeColliderLayerMask;
    [ReadOnly, SerializeField] LayerMask stairEdgeColliderLayerMask;
    [ReadOnly, SerializeField] LayerMask placeableObjectsColliderLayerMask;

    [Space(15)]

    [ReadOnly, SerializeField] bool debug;
    [ReadOnly, SerializeField] int debugFontSize = 100;
    [ReadOnly, SerializeField] Direction currentDirection = Direction.Down;
    public Direction CurrentDirection => currentDirection;
    [ReadOnly, SerializeField] bool currentEdgeFlipMode = false;
    public bool CurrentEdgeFlipMode => currentEdgeFlipMode;

    List<GridXZ<GridBuildingCell>> gridList;
    GridXZ<GridBuildingCell> selectedGrid;

    PlaceableObjectSO currentPlaceableObjectSO;
    public PlaceableObjectSO CurrentPlaceableObjectSO => currentPlaceableObjectSO;

    float looseObjectEulerY;
    public float LooseObjectEulerY => looseObjectEulerY;

    BuildingGhost buildingGhost;
    public BuildingGhost BuildingGhost => buildingGhost;

    bool enableMouse3DDebug;

    [SerializeField] GameObject debugHolder;
    public GameObject DebugHolder => debugHolder;

    bool looseObjectRotate = false;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        buildingGhost = GetComponent<BuildingGhost>();

        currentDirection = Direction.Down;
    }

    public void Init(List<PlaceableObjectSO> _placeableObjectSOList, int _gridWidth, int _gridLength, float _cellSize, float _gridHeight, int _gridVerticalCount, float _maxBuildDistance, LayerMask _edgeColliderLayerMask, LayerMask _stairEdgeColliderLayerMask, LayerMask _placeableObjectsColliderLayerMask, bool _debug, int _debugFontSize, bool _enableMouse3DDebug)
    {
        placeableObjectSOList = _placeableObjectSOList;
        gridWidth = _gridWidth;
        gridLength = _gridLength;
        cellSize = _cellSize;
        gridHeight = _gridHeight;
        gridVerticalCount = _gridVerticalCount;
        maxBuildDistance = _maxBuildDistance;
        edgeColliderLayerMask = _edgeColliderLayerMask;
        stairEdgeColliderLayerMask = _stairEdgeColliderLayerMask;
        placeableObjectsColliderLayerMask = _placeableObjectsColliderLayerMask;
        debug = _debug;
        debugFontSize = _debugFontSize;
        enableMouse3DDebug = _enableMouse3DDebug;
    }

    public void Setup(Vector3 origin)
    {
        gridList = new List<GridXZ<GridBuildingCell>>();
        
        for(int i = 0; i < gridVerticalCount; i++)
        {
            Vector3 originPos = new Vector3(origin.x, origin.y + gridHeight * i, origin.z);
            GridXZ<GridBuildingCell> grid = new GridXZ<GridBuildingCell>(gridWidth, gridLength, cellSize, originPos, (GridXZ<GridBuildingCell> g, int x, int z) => new GridBuildingCell(g, x, z), debug, debugFontSize);
            gridList.Add(grid);
        }

        selectedGrid = gridList[0];
        
        SelectPlaceableObject(placeableObjectSOList[0]);

        if(enableMouse3DDebug)
        {
            Mouse3D.Instance.debugVisual.gameObject.SetActive(true);
        }
        else
        {
            Mouse3D.Instance.debugVisual.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        HandleGridSwitch();

        HandleLooseObjectRotation();

        if(Input.GetKeyDown(KeyCode.Alpha1)) {SelectPlaceableObject(placeableObjectSOList[0]);}
        if(Input.GetKeyDown(KeyCode.Alpha2)) {SelectPlaceableObject(placeableObjectSOList[1]);}
        if(Input.GetKeyDown(KeyCode.Alpha3)) {SelectPlaceableObject(placeableObjectSOList[2]);}
        if(Input.GetKeyDown(KeyCode.Alpha4)) {SelectPlaceableObject(placeableObjectSOList[3]);}
        if(Input.GetKeyDown(KeyCode.Alpha5)) {SelectPlaceableObject(placeableObjectSOList[4]);}
        if(Input.GetKeyDown(KeyCode.Alpha6)) {SelectPlaceableObject(placeableObjectSOList[5]);}
        if(Input.GetKeyDown(KeyCode.Alpha7)) {SelectPlaceableObject(placeableObjectSOList[6]);}
        if(Input.GetKeyDown(KeyCode.Alpha8)) {SelectPlaceableObject(placeableObjectSOList[7]);}
        if(Input.GetKeyDown(KeyCode.Alpha9)) {SelectPlaceableObject(placeableObjectSOList[8]);}
    }

    public void Rotate(float value)
    {
        if(!PlayerController.Instance.UICanvas.BuildMenuEnabled && value > 0.1f)
        {
            if(currentPlaceableObjectSO is EdgeObjectSO)
            {
                currentEdgeFlipMode = !currentEdgeFlipMode;
                buildingGhost.FlipEdgeObjectGhost(currentEdgeFlipMode);
            }
            else if(currentPlaceableObjectSO is GridObjectSO)
            {
                currentDirection = GridObjectSO.GetNextDirection(currentDirection);
                Debug.Log("Direction: " + currentDirection); 
            } 
            else if(currentPlaceableObjectSO is LooseObjectSO)
            {
                looseObjectRotate = true;
            }
        }
        else
        {
            looseObjectRotate = false;
        }
    }

    private void HandleLooseObjectRotation()
    {
        if(PlayerController.Instance.BuildModeEnabled && !PlayerController.Instance.UICanvas.BuildMenuEnabled && looseObjectRotate)
        {
            if(currentPlaceableObjectSO is LooseObjectSO)
            {
                looseObjectEulerY += Time.deltaTime * 90f;
            }
        }
    }

    private void HandleGridSwitch()
    {
        Vector3 mousePosition = Mouse3D.Instance.GetMouseWorldPosition();
        int newGridIndex = Mathf.Clamp(Mathf.RoundToInt(mousePosition.y / gridHeight), 0, gridList.Count - 1);
        selectedGrid = gridList[newGridIndex];
    }

    public void PlaceObject()
    {
        if(!PlayerController.Instance.UICanvas.BuildMenuEnabled && Vector3.Distance(PlayerController.Instance.Character.transform.position, Mouse3D.Instance.GetMouseWorldPosition()) <= maxBuildDistance)
        {
            if(AmILookingAtCollider())
            {
                switch(currentPlaceableObjectSO.ObjectType)
                {
                    case PlaceableObjectTypes.GridObject:
                        PlaceGridObject();
                    break;

                    case PlaceableObjectTypes.EdgeObject:
                        PlaceEdgeObject();
                    break;

                    case PlaceableObjectTypes.StairEdgeObject:
                        PlaceStairEdgeObject();
                    break;

                    case PlaceableObjectTypes.LooseObject:
                        PlaceLooseObject();
                    break;
                }
            } 
        }  
    }

    public bool CanPlaceObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        switch(currentPlaceableObjectSO.ObjectType)
        {
            case PlaceableObjectTypes.GridObject:

                GridObjectSO gridObjectSO = (GridObjectSO) currentPlaceableObjectSO;
                int x = 0, z = 0;
                List<Vector2Int> gridObjectPositionList = new List<Vector2Int>();

                selectedGrid.GetXZ(Mouse3D.Instance.GetMouseWorldPosition(), out x, out z);
                gridObjectPositionList = gridObjectSO.GetGridPositionList(new Vector2Int(x, z), currentDirection); 

                return CanPlaceGridObjectCheck1(gridObjectPositionList);

            case PlaceableObjectTypes.EdgeObject:

                EdgeObjectSO edgeObjectSO = (EdgeObjectSO) currentPlaceableObjectSO;

                if(Physics.Raycast(ray, out RaycastHit raycastHit, 999f, edgeColliderLayerMask)) 
                {
                    if(raycastHit.collider.TryGetComponent(out EdgePosition edgePosition)) 
                    {
                        if(raycastHit.collider.transform.parent.TryGetComponent(out FloorGridObject floorGridObject)) 
                        {
                            if(edgeObjectSO != null) 
                            {
                                EdgeObject currentEdgeObject = floorGridObject.GetEdgeObject(edgePosition.edge);

                                if(currentEdgeObject == null)
                                {
                                    if(edgeObjectSO.Width == EdgeObjectSO.EdgeWidth.Two && 
                                       floorGridObject.GetEdgeObject(floorGridObject.GetComplimentaryEdge(edgePosition.edge)) != null && 
                                       floorGridObject.IsWestEdge(edgePosition.edge))
                                    {
                                        return false;
                                    }

                                    if(!buildingGhost.GetIfGhostisCollidingEdgeObject())
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }

                return false;

            case PlaceableObjectTypes.StairEdgeObject:

                StairEdgeObjectSO stairEdgeObjectSO = (StairEdgeObjectSO) currentPlaceableObjectSO;

                if(Physics.Raycast(ray, out RaycastHit raycastHit2, 999f, stairEdgeColliderLayerMask)) 
                {
                    if(raycastHit2.collider.TryGetComponent(out StairEdgePosition stairEdgePosition)) 
                    {
                        if(raycastHit2.collider.transform.parent.TryGetComponent(out StairObject stairObject)) 
                        {
                            if(stairEdgeObjectSO != null) 
                            {
                                StairEdgeObject currentEdgeObject = stairObject.GetStairEdgeObject(stairEdgePosition.stairEdge);

                                if(currentEdgeObject == null)
                                {
                                    if(!buildingGhost.GetIfGhostisCollidingEdgeObject())
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }

                return false;

            case PlaceableObjectTypes.LooseObject:
                
                if(Physics.Raycast(ray, out RaycastHit raycastHit3, 999f, Mouse3D.Instance.MouseColliderLayerMaskNoPlaceableCollider)) 
                {
                    if(Physics.Raycast(ray, out RaycastHit dummyHit, raycastHit3.distance + 0.2f, placeableObjectsColliderLayerMask))
                    {
                        if(!buildingGhost.GetIfGhostisCollidingLooseObject())
                        {       
                            return true;
                        }
                    }  
                }

                return false;

            default:
                return true;
        }
    }

    private void PlaceGridObject()
    {
        GridObjectSO gridObjectSO = (GridObjectSO) currentPlaceableObjectSO;
        int x = 0, z = 0;
        List<Vector2Int> gridObjectPositionList = new List<Vector2Int>();
        
        selectedGrid.GetXZ(Mouse3D.Instance.GetMouseWorldPosition(), out x, out z);
        gridObjectPositionList = gridObjectSO.GetGridPositionList(new Vector2Int(x, z), currentDirection);

        List<Vector2Int> gridObjectAdjacentPositionList = gridObjectSO.GetGridAdjacentPositionList(gridObjectPositionList); 

        bool canPlace = CanPlaceGridObjectCheck1(gridObjectPositionList);
        gridObjectPositionList = GetNewGridObjectPositionList(gridObjectPositionList, gridObjectAdjacentPositionList, x, z, out x, out z);

        if(canPlace)
        {
            Vector2Int rotationOffset = gridObjectSO.GetRotationOffset(currentDirection);
            Vector3 gridObjectWorldPosition = selectedGrid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * selectedGrid.GetCellSize();

            GridObject gridObject = GridObject.Create(gridObjectWorldPosition, new Vector2Int(x, z), currentDirection, gridObjectSO);
            
            foreach(Vector2Int gridObjectPosition in gridObjectPositionList)
            {
                selectedGrid.GetGridObject(gridObjectPosition.x, gridObjectPosition.y).SetGridObject(gridObject);
            }
        }
        else
        {
            Debug.Log("Can't Build Here");
        } 
    }

    private bool CanPlaceGridObjectCheck1(List<Vector2Int> gridObjectPositionList)
    {
        foreach(Vector2Int gridObjectPosition in gridObjectPositionList)
        {
            if(!selectedGrid.GetGridObject(gridObjectPosition.x, gridObjectPosition.y).CanBuild())
            {  
                return false;
            }
        }

        return true;
    }

    private bool CanPlaceGridObjectCheck2(List<Vector2Int> gridObjectAdjacentPositionList)
    {
        if(!Mouse3D.Instance.GetMouseWorldLayerBool(Mouse3D.Instance.PlaceableColliderLayer))
        {
            return true;
        }
        else if(Mouse3D.Instance.GetMouseWorldLayerBool(Mouse3D.Instance.MouseColliderLayerMaskNoPlaceableCollider))
        {
            Vector3 firstHitPos = Mouse3D.Instance.GetMouseWorldPosition(Mouse3D.Instance.MouseColliderLayerMaskNoPlaceableCollider);
            Vector3 secondHitPos = Mouse3D.Instance.GetMouseWorldPosition(Mouse3D.Instance.PlaceableColliderLayer);

            if(Vector3.Distance(firstHitPos, secondHitPos) < 1)
            {
                return true;
            }

            // Case Placeable Collider is part of a Grid Object
            foreach(Vector2Int gridObjectAdjacentPosition in gridObjectAdjacentPositionList)
            {
                if(!selectedGrid.GetGridObject(gridObjectAdjacentPosition.x, gridObjectAdjacentPosition.y).CanBuild())
                {
                    return true;
                }
            }

            // Case Placeable Collider is part of an Edge Object
            if(buildingGhost.GhostOverlapBoxEdgeObject())
            {
                return true;
            }
            
            return false;
        }
        else
        {
            // Case Placeable Collider is part of a Grid Object
            foreach(Vector2Int gridObjectAdjacentPosition in gridObjectAdjacentPositionList)
            {
                if(!selectedGrid.GetGridObject(gridObjectAdjacentPosition.x, gridObjectAdjacentPosition.y).CanBuild())
                {
                    return true;
                }
            }

            // Case Placeable Collider is part of an Edge Object
            if(buildingGhost.GhostOverlapBoxEdgeObject())
            {
                return true;
            }
            
            return false;
        }
    }

    private List<Vector2Int> GetNewGridObjectPositionList(List<Vector2Int> gridObjectPositionList, List<Vector2Int> gridObjectAdjacentPositionList, int x, int z, out int x2, out int z2)
    {
        bool canPlace = CanPlaceGridObjectCheck2(gridObjectAdjacentPositionList);

        // Still cannot place means its a floating placement
        if(canPlace)
        {
            x2 = x;
            z2 = z;
            return gridObjectPositionList; 
        }
        else
        {
            List<Vector2Int> newGridObjectPositionList = new List<Vector2Int>();

            GetDirectionOffsets(gridObjectPositionList, newGridObjectPositionList, x, z, out x2, out z2);

            return newGridObjectPositionList;
        }
    }

    private void GetDirectionOffsets(List<Vector2Int> gridObjectPositionList, List<Vector2Int> newGridObjectPositionList, int x, int z, out int x2, out int z2)
    {
        if(Mouse3D.Instance.GetMouseGameObject().GetComponentInParent<PlaceableObject>() == null)
        {
            Debug.Log(Mouse3D.Instance.GetMouseGameObject().transform.parent.gameObject.name);
        }
        Vector3 objPosV3 = Mouse3D.Instance.GetMouseGameObject().GetComponentInParent<PlaceableObject>().CenterPivot.position;
        Vector2 objPosV2 = new Vector2(objPosV3.x, objPosV3.z);

        Vector3 mouseWorldPosition = Mouse3D.Instance.GetMouseWorldPosition();
        Vector2Int gridPos = new Vector2Int(selectedGrid.GetGridObject(mouseWorldPosition).X, selectedGrid.GetGridObject(mouseWorldPosition).Z);
        Vector2 mouseGridPosV2 = new Vector2(selectedGrid.GetWorldPosition(gridPos.x, gridPos.y).x, selectedGrid.GetWorldPosition(gridPos.x, gridPos.y).z);
        mouseGridPosV2 = mouseGridPosV2 + new Vector2(cellSize/2, cellSize/2);

        Vector2 directionVector = mouseGridPosV2 - objPosV2;
        directionVector = directionVector.normalized;
        
        x2 = x;
        z2 = z;

        if(Mathf.Abs(directionVector.y) > 0.5)
        {
            // Up
            if(directionVector.y > 0)
            {
                foreach(Vector2Int gridObjectPosition in gridObjectPositionList)
                {
                    newGridObjectPositionList.Add(new Vector2Int(gridObjectPosition.x, gridObjectPosition.y - 1));
                }

                z2 = z - 1;
            }
            // Down
            else if(directionVector.y < 0)
            {
                foreach(Vector2Int gridObjectPosition in gridObjectPositionList)
                {
                    newGridObjectPositionList.Add(new Vector2Int(gridObjectPosition.x, gridObjectPosition.y + 1));
                }

                z2 = z + 1;
            }
        }
        else if(Math.Abs(directionVector.x) > 0.5)
        {
            // Right
            if(directionVector.x > 0)
            {
                foreach(Vector2Int gridObjectPosition in gridObjectPositionList)
                {
                    newGridObjectPositionList.Add(new Vector2Int(gridObjectPosition.x - 1, gridObjectPosition.y));
                }

                x2 = x - 1;
            }
            // Left
            else if(directionVector.x < 0)
            {
            foreach(Vector2Int gridObjectPosition in gridObjectPositionList)
                {
                    newGridObjectPositionList.Add(new Vector2Int(gridObjectPosition.x + 1, gridObjectPosition.y));
                }

                x2 = x + 1;
            }
        }
    }

    private void PlaceEdgeObject()
    {
        EdgeObjectSO edgeObjectSO = (EdgeObjectSO) currentPlaceableObjectSO;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out RaycastHit raycastHit, 999f, edgeColliderLayerMask)) 
        {
            if(raycastHit.collider.TryGetComponent(out EdgePosition edgePosition)) 
            {
                if(raycastHit.collider.transform.parent.TryGetComponent(out FloorGridObject floorGridObject)) 
                {
                    if(edgeObjectSO != null) 
                    {
                        EdgeObject currentEdgeObject = floorGridObject.GetEdgeObject(edgePosition.edge);

                        if(currentEdgeObject == null)
                        {
                            if(edgeObjectSO.Width == EdgeObjectSO.EdgeWidth.Two && 
                               floorGridObject.GetEdgeObject(floorGridObject.GetComplimentaryEdge(edgePosition.edge)) != null && 
                               floorGridObject.IsWestEdge(edgePosition.edge))
                            {
                                Debug.Log("Can't Place Edge Object");
                                return;
                            }

                            if(!buildingGhost.GetIfGhostisCollidingEdgeObject())
                            {
                                floorGridObject.PlaceEdge(edgePosition.edge, edgeObjectSO, currentEdgeFlipMode);
                            }
                            else
                            {
                                Debug.Log("Can't Place Edge Object");
                            }
                        }
                        else
                        {
                            Debug.Log("Can't Place Edge Object");
                        }
                    }
                }
            }
        }
    }

    private void PlaceStairEdgeObject()
    {
        StairEdgeObjectSO stairEdgeObjectSO = (StairEdgeObjectSO) currentPlaceableObjectSO;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out RaycastHit raycastHit, 999f, stairEdgeColliderLayerMask)) 
        {
            if(raycastHit.collider.TryGetComponent(out StairEdgePosition stairEdgePosition)) 
            {
                if(raycastHit.collider.transform.parent.TryGetComponent(out StairObject stairObject)) 
                {
                    if(stairEdgeObjectSO != null) 
                    {
                        if(!buildingGhost.GetIfGhostisCollidingEdgeObject())
                        {   
                            stairObject.PlaceStairEdge(stairEdgePosition.stairEdge, stairEdgeObjectSO);
                        }
                        else
                        {
                            Debug.Log("Can't Place Stair Edge Object");
                        }
                    }
                    else
                    {
                        Debug.Log("Can't Place Stair Edge Object");
                    }
                }
            }
        }
    }

    private void PlaceLooseObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out RaycastHit raycastHit, 999f, Mouse3D.Instance.MouseColliderLayerMaskNoPlaceableCollider)) 
        {
            Debug.Log("1");
            if(Physics.Raycast(ray, out RaycastHit dummyHit, raycastHit.distance + 0.2f, placeableObjectsColliderLayerMask))
            {
                Debug.Log("2");
                if(!buildingGhost.GetIfGhostisCollidingLooseObject())
                {
                    Debug.Log("3");
                    Transform looseObjectTransform = Instantiate(currentPlaceableObjectSO.Prefab, dummyHit.point, Quaternion.Euler(0, looseObjectEulerY, 0));
                }
            }  
        }     
    }

    public void DemolishPlacedObject()
    {
        if(!PlayerController.Instance.UICanvas.BuildMenuEnabled)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out RaycastHit raycastHit, 999f, Mouse3D.Instance.MouseColliderLayerMaskNoPlaceableCollider))
            {
                if(raycastHit.collider.GetComponentInParent<GridObject>() != null)
                {
                    GridObject gridObject = raycastHit.collider.GetComponentInParent<GridObject>();
                    gridObject.DestroySelf(); 

                    List<Vector2Int> gridObjectPositionList = gridObject.GetGridPositionList();
                
                    foreach(Vector2Int gridObjectPosition in gridObjectPositionList)
                    {
                        selectedGrid.GetGridObject(gridObjectPosition.x, gridObjectPosition.y).ClearGridObject();
                    }
                }
                else if(raycastHit.collider.GetComponentInParent<EdgeObject>() != null)
                {
                    EdgeObject edgeObject = raycastHit.collider.GetComponentInParent<EdgeObject>();
                    edgeObject.DestroySelf();
                }
                else if(raycastHit.collider.GetComponentInParent<LooseObject>() != null)
                {
                    LooseObject looseObject = raycastHit.collider.GetComponentInParent<LooseObject>();
                    looseObject.DestroySelf();
                    buildingGhost.RefreshVisual();
                }
                else if(raycastHit.collider.GetComponentInParent<StairEdgeObject>() != null)
                {
                    StairEdgeObject stairEdgeObject = raycastHit.collider.GetComponentInParent<StairEdgeObject>();
                    stairEdgeObject.DestroySelf();
                }
            }
        } 
    }

    public void SelectPlaceableObject(PlaceableObjectSO placeableObject)
    {
        currentPlaceableObjectSO = placeableObject;
        buildingGhost.RefreshVisual();
    }

    public Vector3 GetMouseWorldSnappedPosition() 
    {
        Vector3 mousePosition = Mouse3D.Instance.GetMouseWorldPosition();

        selectedGrid.GetXZ(mousePosition, out int x, out int z);

        if(currentPlaceableObjectSO is GridObjectSO) 
        {
            Vector2Int rotationOffset = ((GridObjectSO) currentPlaceableObjectSO).GetRotationOffset(currentDirection);
            Vector3 gridObjectWorldPosition = selectedGrid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * selectedGrid.GetCellSize();
            return gridObjectWorldPosition;
        }
        else 
        {
            return mousePosition;
        }
    }

    public EdgePosition GetMouseEdgePosition() 
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out RaycastHit raycastHit, 999f, edgeColliderLayerMask)) 
        {
            if(raycastHit.collider.TryGetComponent(out EdgePosition edgePosition)) 
            {
                return edgePosition;
            }
        }

        return null;
    }

    public StairEdgePosition GetMouseStairEdgePosition() 
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out RaycastHit raycastHit, 999f, stairEdgeColliderLayerMask)) 
        {
            if(raycastHit.collider.TryGetComponent(out StairEdgePosition stairEdgePosition)) 
            {
                return stairEdgePosition;
            }
        }

        return null;
    }

    public Quaternion GetGridObjectRotation() 
    {
        if(currentPlaceableObjectSO is GridObjectSO)
        {
            return Quaternion.Euler(0, ((GridObjectSO) currentPlaceableObjectSO).GetRotationAngle(currentDirection), 0);
        }
        else
        {
            return Quaternion.identity;
        }
    }

    public bool AmILookingAtCollider()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out RaycastHit raycastHit, 999f, Mouse3D.Instance.CurrentLayerMask)) 
        {
            return true;
        } 
        else 
        {
            return false;
        }
    }

    public Vector3 GetNewGridObjectPosition()
    {
        GridObjectSO gridObjectSO = (GridObjectSO) currentPlaceableObjectSO;
        int x = 0, z = 0;
        List<Vector2Int> gridObjectPositionList = new List<Vector2Int>();
        
        selectedGrid.GetXZ(Mouse3D.Instance.GetMouseWorldPosition(), out x, out z);
        gridObjectPositionList = gridObjectSO.GetGridPositionList(new Vector2Int(x, z), currentDirection);

        List<Vector2Int> gridObjectAdjacentPositionList = gridObjectSO.GetGridAdjacentPositionList(gridObjectPositionList); 
        
        bool canPlace = CanPlaceGridObjectCheck2(gridObjectAdjacentPositionList);

        Vector2Int rotationOffset = gridObjectSO.GetRotationOffset(currentDirection);
        Vector3 gridObjectWorldPosition = selectedGrid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * selectedGrid.GetCellSize();

        // Still cannot place means its a floating placement
        if(!canPlace)
        {
            if(Mouse3D.Instance.GetMouseGameObject() != null && Mouse3D.Instance.GetMouseGameObject().GetComponentInParent<PlaceableObject>() != null)
            {
                List<Vector2Int> newGridObjectPositionList = new List<Vector2Int>();

                GetDirectionOffsets(gridObjectPositionList, newGridObjectPositionList, x, z, out x, out z);

                gridObjectPositionList = newGridObjectPositionList;

                gridObjectWorldPosition = selectedGrid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * selectedGrid.GetCellSize();
            }
        }

        return gridObjectWorldPosition;
    }
}