using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuildingManager : MonoBehaviour
{
    public static GridBuildingManager Instance { get; private set; }
    [SerializeField] List<PlacedObjectTypeSO> placedObjectSOList;

    [Space(15)]
    [SerializeField] int gridWidth = 10;
    [SerializeField] int gridHeight = 10;
    [SerializeField] float cellSize = 10f;

    [Space(15)]
    [SerializeField] bool debug;
    [SerializeField] int debugFontSize = 100;
    [ReadOnly, SerializeField] PlacedObjectTypeSO.Dir currentDirection = PlacedObjectTypeSO.Dir.Down;

    GridXZ<GridBuildingCell> grid;
    PlacedObjectTypeSO currentPlacedObjectSO;

    public event EventHandler OnSelectedChanged;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        grid = new GridXZ<GridBuildingCell>(gridWidth, gridHeight, cellSize, Vector3.zero, (GridXZ<GridBuildingCell> g, int x, int z) => new GridBuildingCell(g, x, z), debug, debugFontSize);
        
        currentPlacedObjectSO = placedObjectSOList[0];
        RefreshSelectedObjectType();
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
            currentDirection = PlacedObjectTypeSO.GetNextDir(currentDirection);
            Debug.Log("Direction: " + currentDirection);
        }

        if(Input.GetKeyDown(KeyCode.Alpha1)) {currentPlacedObjectSO = placedObjectSOList[0]; RefreshSelectedObjectType();}
        if(Input.GetKeyDown(KeyCode.Alpha2)) {currentPlacedObjectSO = placedObjectSOList[1]; RefreshSelectedObjectType();}
        if(Input.GetKeyDown(KeyCode.Alpha3)) {currentPlacedObjectSO = placedObjectSOList[2]; RefreshSelectedObjectType();}
    }

    private void Place()
    {
        grid.GetXZ(Mouse3D.GetMouseWorldPosition(), out int x, out int z);

        List<Vector2Int> placedObjectPositionList = currentPlacedObjectSO.GetGridPositionList(new Vector2Int(x, z), currentDirection);
        
        bool canBuild = true;

        foreach(Vector2Int placedObjectPosition in placedObjectPositionList)
        {
            if(!grid.GetGridObject(placedObjectPosition.x, placedObjectPosition.y).CanBuild())
            {
                canBuild = false; 
                break;
            }
        }

        GridBuildingCell gridCell = grid.GetGridObject(x, z);
        
        if(canBuild)
        {
            Vector2Int rotationOffset = currentPlacedObjectSO.GetRotationOffset(currentDirection);
            Vector3 placeObjectWorldPosition = grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();

            PlacedObject placedObject = PlacedObject.Create(placeObjectWorldPosition, new Vector2Int(x, z), currentDirection, currentPlacedObjectSO);
            
            foreach(Vector2Int placedObjectPosition in placedObjectPositionList)
            {
                grid.GetGridObject(placedObjectPosition.x, placedObjectPosition.y).SetPlacedObject(placedObject);
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
        PlacedObject placedObject = gridBuildingCell.GetPlacedObject();

        if(placedObject != null)
        {
            placedObject.DestroySelf(); 

            List<Vector2Int> placedObjectPositionList = placedObject.GetGridPositionList();
        
            foreach(Vector2Int placedObjectPosition in placedObjectPositionList)
            {
                grid.GetGridObject(placedObjectPosition.x, placedObjectPosition.y).ClearPlacedObject();
            }
        }
    }

    private void RefreshSelectedObjectType()
    {
        OnSelectedChanged?.Invoke(this, EventArgs.Empty);
    }

    public Vector3 GetMouseWorldSnappedPosition() 
    {
        Vector3 mousePosition = Mouse3D.GetMouseWorldPosition();
        grid.GetXZ(mousePosition, out int x, out int z);

        if (currentPlacedObjectSO != null) 
        {
            Vector2Int rotationOffset = currentPlacedObjectSO.GetRotationOffset(currentDirection);
            Vector3 placedObjectWorldPosition = grid.GetWorldPosition(x, z) + new Vector3(rotationOffset.x, 0, rotationOffset.y) * grid.GetCellSize();
            return placedObjectWorldPosition;
        } 
        else 
        {
            return mousePosition;
        }
    }

    public Quaternion GetPlacedObjectRotation() 
    {
        if (currentPlacedObjectSO != null) 
        {
            return Quaternion.Euler(0, currentPlacedObjectSO.GetRotationAngle(currentDirection), 0);
        } 
        else 
        {
            return Quaternion.identity;
        }
    }

    public PlacedObjectTypeSO GetPlacedObjectTypeSO() 
    {
        return currentPlacedObjectSO;
    }
}
