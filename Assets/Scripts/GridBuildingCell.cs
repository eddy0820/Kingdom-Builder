using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBuildingCell
{
    GridXZ<GridBuildingCell> grid;
    int x;
    int z;
    PlacedObject placedObject;

    public GridBuildingCell(GridXZ<GridBuildingCell> _grid, int _x, int _z)
    {
        grid = _grid;
        x = _x;
        z = _z;
    }

    public void SetPlacedObject(PlacedObject _placedObject)
    {
        placedObject = _placedObject;
        grid.TriggerGridObjectChanged(x, z);
    }

    public PlacedObject GetPlacedObject()
    {
        return placedObject;
    }

    public void ClearPlacedObject()
    {
        placedObject = null;
        grid.TriggerGridObjectChanged(x, z);
    }

    public bool CanBuild()
    {
        return placedObject == null;
    }

    public override string ToString()
    {
        return x + ", " + z + "\n" + placedObject;
    }
}
