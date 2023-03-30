using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class GridObject : PlaceableObject
{
    GridObjectSO gridObjectSO;
    public GridObjectSO GridObjectSO => gridObjectSO;
    Vector2Int origin;
    public Vector2Int Origin => origin;
    [ReadOnly, SerializeField] Direction direction;
    public Direction Direction => direction;

    public static GridObject Create(Vector3 worldPosition, Vector2Int _origin, Direction _direction, GridObjectSO _gridObjectSO) 
    {
        Transform placedObjectTransform = Instantiate(_gridObjectSO.Prefab, worldPosition, Quaternion.Euler(0, _gridObjectSO.GetRotationAngle(_direction), 0));

        GridObject placedObject = placedObjectTransform.GetComponent<GridObject>();
        
        placedObject.gridObjectSO = _gridObjectSO;
        placedObject.origin = _origin;
        placedObject.direction = _direction;

        return placedObject;
    }

    public List<Vector2Int> GetGridPositionList()
    {
        return gridObjectSO.GetGridPositionList(origin, direction);
    }

    public override void DestroySelf()
    {
        Destroy(gameObject);
    }
}
