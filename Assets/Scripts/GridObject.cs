using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject : MonoBehaviour
{
    GridObjectSO gridObjectSO;
    Vector2Int origin;
    GridObjectSO.Dir direction;

    public static GridObject Create(Vector3 worldPosition, Vector2Int _origin, GridObjectSO.Dir _direction, GridObjectSO _gridObjectSO) 
    {
        Transform placedObjectTransform = Instantiate(_gridObjectSO.prefab, worldPosition, Quaternion.Euler(0, _gridObjectSO.GetRotationAngle(_direction), 0));

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

    public virtual void DestroySelf()
    {
        Destroy(gameObject);
    }
}
