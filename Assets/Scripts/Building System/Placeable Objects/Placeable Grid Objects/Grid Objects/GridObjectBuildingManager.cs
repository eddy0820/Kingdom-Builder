using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class GridObjectBuildingManager : AbstractPlaceableObjectBuildingManager
{
    [ReadOnly, SerializeField] Direction currentDirection = Direction.Down;
    public Direction CurrentDirection => currentDirection;

    protected override void OnAwake() 
    {
        currentDirection = Direction.Down;
    }

    public override bool PlaceObject(out PlaceableObject placeableObject)
    {
        GridXZ<GridBuildingCell> selectedGrid = GridBuildingManager.SelectedGrid;
        GridObjectSO currentGridObjectSO = (GridObjectSO) GridBuildingManager.CurrentPlaceableObjectSO;

        if(CanPlaceObject(currentGridObjectSO, selectedGrid, out List<Vector2Int> gridObjectPositionList, out int x, out int z))
        {
            // Get actual World Position with rotation direction accounted for
            Vector2Int rotationOffset = currentGridObjectSO.GetRotationOffset(currentDirection);
            Vector3 currentGridObjectActualWorldPosition = selectedGrid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * selectedGrid.GetCellSize();

            // OBJECT CREATION
            GridObject gridObject = GridObject.Create(currentGridObjectActualWorldPosition, new Vector2Int(x, z), currentDirection, currentGridObjectSO);
            
            // Set the Grid Positions to have this object
            foreach(Vector2Int gridObjectPosition in gridObjectPositionList)
            {
                selectedGrid.GetGridObject(gridObjectPosition.x, gridObjectPosition.y).SetGridObject(gridObject);
            }

            placeableObject = gridObject;
            return true;
        }

        Debug.Log("Can't Build Here");
        placeableObject = null;
        return false;  
    }

    private bool CanBuildGridObjectInGridPositions(List<Vector2Int> gridObjectPositionList)
    {
        foreach(Vector2Int gridObjectPosition in gridObjectPositionList)
        {
            if(!GridBuildingManager.SelectedGrid.GetGridObject(gridObjectPosition.x, gridObjectPosition.y).CanBuild())
            {  
                return false;
            }
        }

        return true;
    }

    public override void Demolish(PlaceableObject placeableObject)
    {
        GridObject gridObject = (GridObject) placeableObject;
        gridObject.DestroySelf(); 

        List<Vector2Int> gridObjectPositionList = gridObject.GetGridPositionList();

        foreach(Vector2Int gridObjectPosition in gridObjectPositionList)
        {
            GridBuildingManager.SelectedGrid.GetGridObject(gridObjectPosition.x, gridObjectPosition.y).ClearGridObject();
        }
    }

    public override void Rotate()
    {   
        currentDirection = GridObjectSO.GetNextDirection(currentDirection);
        Debug.Log("Direction: " + currentDirection); 
    }

    private bool IsFloatingPlacement(List<Vector2Int> gridObjectAdjacentPositionList)
    {
        // I am not looking at Placeable Collider which means it can't be floating
        if(!Mouse3D.Instance.IsLookingAtLayer("Placeable Collider"))
        {
            return false;
        }
        
        Vector3 firstHitPos = Mouse3D.Instance.GetMouseWorldPosition(Mouse3D.Instance.MouseColliderLayerMaskNoPlaceableCollider);
        Vector3 secondHitPos = Mouse3D.Instance.GetMouseWorldPosition(Mouse3D.Instance.PlaceableColliderLayer);

        // The distance between 
        //
        //      The position of where the first raycast hits when only aiming the Placeable Collider
        //                                              AND
        //      The position of where the second raycast hits when not aiming for a Placeable Collider
        //
        // is very small, therefore this is most likely a Placeable Collider that is part of a Grid Object on the floor,
        // which means that it's not floating
        if(Vector3.Distance(firstHitPos, secondHitPos) < 1)
        {
            return false;
        }

        // I am getting the grid cells around the current position, if all of them are buildable it means that it's a floating placement
        foreach(Vector2Int gridObjectAdjacentPosition in gridObjectAdjacentPositionList)
        {
            if(!GridBuildingManager.SelectedGrid.GetGridObject(gridObjectAdjacentPosition.x, gridObjectAdjacentPosition.y).CanBuild())
            {
                return false;
            }
        }


        // If the fake ghost collider is colliding with an Edge Object it cannot be floating
        if(GridBuildingManager.BuildingGhost.GridObjectBuildingGhost.IsFakeGhostCollidingWithEdgeObject())
        {
            return false;
        }


        // If none of the previous then yes it is a floating placement
        return true;

    }

    private List<Vector2Int> ShiftGridObjectToNewSpot(List<Vector2Int> gridObjectPositionList, int x, int z, out int x2, out int z2)
    {
        List<Vector2Int> newGridObjectPositionList = new List<Vector2Int>();
        GridXZ<GridBuildingCell> selectedGrid = GridBuildingManager.SelectedGrid;

        // Get the Center Pivot of the GridObject as a Vector2
        Vector3 gridObjectCenterPivotPosV3 = Mouse3D.Instance.GetMouseGameObject().GetComponentInParent<PlaceableObject>().CenterPivot.position;
        Vector2 gridObjectCenterPivotPosV2 = new Vector2(gridObjectCenterPivotPosV3.x, gridObjectCenterPivotPosV3.z);

        // Get the World Position of where you are trying to place the GridObject
        Vector3 mouseWorldPos = Mouse3D.Instance.GetMouseWorldPosition();
        GridBuildingCell gridBuildingCellAtMouse = selectedGrid.GetGridObject(mouseWorldPos);
        Vector2Int gridBuildingCellGridPos = new Vector2Int(gridBuildingCellAtMouse.X, gridBuildingCellAtMouse.Z);
        Vector3 gridBuildingCellWorldPosV3 = selectedGrid.GetWorldPosition(gridBuildingCellGridPos.x, gridBuildingCellGridPos.y);
        Vector2 gridBuildingCellWorldPosV2 = new Vector2(gridBuildingCellWorldPosV3.x, gridBuildingCellWorldPosV3.z);
        gridBuildingCellWorldPosV2 = gridBuildingCellWorldPosV2 + new Vector2(GridBuildingManager.CellSize/2, GridBuildingManager.CellSize/2);

        // Get the direction vector of the placement;
        Vector2 directionVector = (gridBuildingCellWorldPosV2 - gridObjectCenterPivotPosV2).normalized;

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
        else if(Mathf.Abs(directionVector.x) > 0.5)
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

        return newGridObjectPositionList;
    }

    public Vector3 GetMouseWorldSnappedPosition() 
    {
        GridXZ<GridBuildingCell> selectedGrid = GridBuildingManager.SelectedGrid;
        GridObjectSO currentGridObjectSO = (GridObjectSO) GridBuildingManager.CurrentPlaceableObjectSO;

        selectedGrid.GetXZ(Mouse3D.Instance.GetMouseWorldPosition(), out int x, out int z);

        Vector2Int rotationOffset = currentGridObjectSO.GetRotationOffset(currentDirection);
        return selectedGrid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * selectedGrid.GetCellSize();
    }

    public Vector3 GetClosestMouseWorldSnappedPosition()
    {
        GridXZ<GridBuildingCell> selectedGrid = GridBuildingManager.SelectedGrid;
        GridObjectSO currentGridObjectSO = (GridObjectSO) GridBuildingManager.CurrentPlaceableObjectSO;

        selectedGrid.GetClosestXZ(Mouse3D.Instance.GetMouseWorldPosition(), out int x, out int z);

        Vector2Int rotationOffset = currentGridObjectSO.GetRotationOffset(currentDirection);
        return selectedGrid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * selectedGrid.GetCellSize();
    }

    public Vector3 GetGridObjectPosition()
    {
        GridXZ<GridBuildingCell> selectedGrid = GridBuildingManager.SelectedGrid;
        GridObjectSO currentGridObjectSO = (GridObjectSO) GridBuildingManager.CurrentPlaceableObjectSO;
        
        // Get the Grid XZ
        int x = 0, z = 0;
        selectedGrid.GetXZ(Mouse3D.Instance.GetMouseWorldPosition(), out x, out z);

        // Get Grid Positions List in given Grid XZ
        List<Vector2Int> gridObjectPositionList = currentGridObjectSO.GetGridPositionList(new Vector2Int(x, z), currentDirection);
        // Get Adjacent Grid Positons List
        List<Vector2Int> gridObjectAdjacentPositionList = currentGridObjectSO.GetGridAdjacentPositionList(gridObjectPositionList); 

        if(IsFloatingPlacement(gridObjectAdjacentPositionList))
        {
            ShiftGridObjectToNewSpot(gridObjectPositionList, x, z, out x, out z);
        }

        Vector2Int rotationOffset = currentGridObjectSO.GetRotationOffset(currentDirection);

        return selectedGrid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * selectedGrid.GetCellSize();
    }

    public Vector3 GetClosestGridObjectPosition()
    {
        GridXZ<GridBuildingCell> selectedGrid = GridBuildingManager.SelectedGrid;
        GridObjectSO currentGridObjectSO = (GridObjectSO) GridBuildingManager.CurrentPlaceableObjectSO;
        
        // Get the Grid XZ
        int x = 0, z = 0;
        selectedGrid.GetClosestXZ(Mouse3D.Instance.GetMouseWorldPosition(), out x, out z);

        // Get Grid Positions List in given Grid XZ
        List<Vector2Int> gridObjectPositionList = currentGridObjectSO.GetGridPositionList(new Vector2Int(x, z), currentDirection);
        // Get Adjacent Grid Positons List
        List<Vector2Int> gridObjectAdjacentPositionList = currentGridObjectSO.GetGridAdjacentPositionList(gridObjectPositionList); 

        if(IsFloatingPlacement(gridObjectAdjacentPositionList))
        {
            ShiftGridObjectToNewSpot(gridObjectPositionList, x, z, out x, out z);
        }

        Vector2Int rotationOffset = currentGridObjectSO.GetRotationOffset(currentDirection);

        return selectedGrid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * selectedGrid.GetCellSize();
    }
    
    public Quaternion GetGridObjectRotation() 
    {
        return Quaternion.Euler(0, ((GridObjectSO) GridBuildingManager.CurrentPlaceableObjectSO).GetRotationAngle(currentDirection), 0);
    }

    private bool CanPlaceObject(GridObjectSO currentGridObjectSO, GridXZ<GridBuildingCell> selectedGrid, out List<Vector2Int> gridObjectPositionList, out int x, out int z)
    {
        x = 0;
        z = 0;
        selectedGrid.GetXZ(Mouse3D.Instance.GetMouseWorldPosition(), out x, out z);

        // Get Grid Positions List in given Grid XZ
        gridObjectPositionList = currentGridObjectSO.GetGridPositionList(new Vector2Int(x, z), currentDirection);
        // Get Adjacent Grid Positons List
        List<Vector2Int> gridObjectAdjacentPositionList = currentGridObjectSO.GetGridAdjacentPositionList(gridObjectPositionList); 

        if(IsFloatingPlacement(gridObjectAdjacentPositionList))
        {
           gridObjectPositionList = ShiftGridObjectToNewSpot(gridObjectPositionList, x, z, out x, out z);
        }

        return CanBuildGridObjectInGridPositions(gridObjectPositionList);
    }

    public override bool CanPlace()
    {
        GridXZ<GridBuildingCell> selectedGrid = GridBuildingManager.SelectedGrid;
        GridObjectSO currentGridObjectSO = (GridObjectSO) GridBuildingManager.CurrentPlaceableObjectSO;

        return CanPlaceObject(currentGridObjectSO, selectedGrid, out List<Vector2Int> gridObjectPositionList, out int x, out int z);
    }

}
