using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuildingCell
{
    GridXZ<GridBuildingCell> grid;
    int x;
    public int X => x;
    int z;
    public int Z => z;
    GridObject gridObject;

    public GridBuildingCell(GridXZ<GridBuildingCell> _grid, int _x, int _z)
    {
        grid = _grid;
        x = _x;
        z = _z;
    }

    public void SetGridObject(GridObject _gridObject)
    {
        gridObject = _gridObject;
        grid.TriggerGridObjectChanged(x, z);
    }

    public GridObject GetGridObject()
    {
        return gridObject;
    }

    public void ClearGridObject()
    {
        gridObject = null;
        grid.TriggerGridObjectChanged(x, z);
    }

    public bool CanBuild()
    {
        return gridObject == null;
    }

    public override string ToString()
    {
        return x + ", " + z + "\n" + gridObject;
    }

    public bool CompareGridCords(GridBuildingCell other)
    {
        if(x == other.x && z == other.z)
        {
            return true;
        }

        return false;
    }
}
