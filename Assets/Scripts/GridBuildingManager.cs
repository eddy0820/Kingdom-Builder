using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuildingManager : MonoBehaviour
{
    public static GridBuildingManager Instance { get; private set; }
    [SerializeField] List<GridObjectSO> gridObjectSOList;
    [SerializeField] List<EdgeObjectSO> edgeObjectSOList;

    [Space(15)]
    [SerializeField] int gridWidth = 10;
    [SerializeField] int gridHeight = 10;
    [SerializeField] float cellSize = 10f;

    [Space(15)]
    [SerializeField] bool debug;
    [SerializeField] int debugFontSize = 100;
    [ReadOnly, SerializeField] GridObjectSO.Dir currentDirection = GridObjectSO.Dir.Down;
    [ReadOnly, SerializeField] PlacedObjectType placedObjectType = PlacedObjectType.GridObject;

    GridXZ<GridBuildingCell> grid;
    GridObjectSO currentGridObjectSO;
    EdgeObjectSO currentEdgeObjectSO;

    public event EventHandler OnSelectedChanged;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        grid = new GridXZ<GridBuildingCell>(gridWidth, gridHeight, cellSize, Vector3.zero, (GridXZ<GridBuildingCell> g, int x, int z) => new GridBuildingCell(g, x, z), debug, debugFontSize);
        
        SelectGridObjectSO(gridObjectSOList[0]);
        SelectEdgeObjectSO(edgeObjectSOList[0]);
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Place();
        }

        if(Input.GetMouseButtonDown(1))
        {
            DestroyPlacedObject();
        }

        if(Input.GetKeyDown(KeyCode.R))
        {
            currentDirection = GridObjectSO.GetNextDir(currentDirection);
            Debug.Log("Direction: " + currentDirection);
        }

        if(Input.GetKeyDown(KeyCode.Alpha1)) {SelectGridObjectSO(gridObjectSOList[0]);}
        if(Input.GetKeyDown(KeyCode.Alpha2)) {SelectGridObjectSO(gridObjectSOList[1]);}
        if(Input.GetKeyDown(KeyCode.Alpha3)) {SelectGridObjectSO(gridObjectSOList[2]);}
    }

    private void Place()
    {
        grid.GetXZ(Mouse3D.GetMouseWorldPosition(), out int x, out int z);

        List<Vector2Int> gridObjectPositionList = currentGridObjectSO.GetGridPositionList(new Vector2Int(x, z), currentDirection);
        
        bool canBuild = true;

        foreach(Vector2Int gridObjectPosition in gridObjectPositionList)
        {
            if(!grid.GetGridObject(gridObjectPosition.x, gridObjectPosition.y).CanBuild())
            {
                canBuild = false; 
                break;
            }
        }
        
        if(canBuild)
        {
            Vector2Int rotationOffset = currentGridObjectSO.GetRotationOffset(currentDirection);
            Vector3 gridObjectWorldPosition = grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();

            GridObject gridObject = GridObject.Create(gridObjectWorldPosition, new Vector2Int(x, z), currentDirection, currentGridObjectSO);
            
            foreach(Vector2Int gridObjectPosition in gridObjectPositionList)
            {
                grid.GetGridObject(gridObjectPosition.x, gridObjectPosition.y).SetGridObject(gridObject);
            }
        }
        else
        {
            Debug.Log("Can't Build Here");
        } 
    }

    private void DestroyPlacedObject()
    {
        GridBuildingCell gridBuildingCell = grid.GetGridObject(Mouse3D.GetMouseWorldPosition());
        GridObject gridObject = gridBuildingCell.GetGridObject();

        if(gridObject != null)
        {
            gridObject.DestroySelf(); 

            List<Vector2Int> gridObjectPositionList = gridObject.GetGridPositionList();
        
            foreach(Vector2Int gridObjectPosition in gridObjectPositionList)
            {
                grid.GetGridObject(gridObjectPosition.x, gridObjectPosition.y).ClearGridObject();
            }
        }
    }

    public void SelectGridObjectSO(GridObjectSO gridObjectSO) 
    {
        placedObjectType = PlacedObjectType.GridObject;
        currentGridObjectSO = gridObjectSO;
        RefreshSelectedObjectType();
    }

    public void SelectEdgeObjectSO(EdgeObjectSO edgeObjectSO) 
    {
        placedObjectType = PlacedObjectType.EdgeObject;
        currentEdgeObjectSO = edgeObjectSO;
        RefreshSelectedObjectType();
    }

    private void RefreshSelectedObjectType()
    {
        OnSelectedChanged?.Invoke(this, EventArgs.Empty);
    }

    public Vector3 GetMouseWorldSnappedPosition() 
    {
        Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();
        grid.GetXZ(mousePosition, out int x, out int z);

        if (currentGridObjectSO != null) 
        {
            Vector2Int rotationOffset = currentGridObjectSO.GetRotationOffset(currentDirection);
            Vector3 gridObjectWorldPosition = grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();
            return gridObjectWorldPosition;
        } 
        else 
        {
            return mousePosition;
        }
    }

    public Quaternion GetGridObjectRotation() 
    {
        if (currentGridObjectSO != null) 
        {
            return Quaternion.Euler(0, currentGridObjectSO.GetRotationAngle(currentDirection), 0);
        } 
        else 
        {
            return Quaternion.identity;
        }
    }

    public GridObjectSO GetGridObjectType() 
    {
        return currentGridObjectSO;
    }

    [System.Serializable]
    public enum PlacedObjectType 
    {
        GridObject,
        EdgeObject,
        LooseObject,
    }
}
