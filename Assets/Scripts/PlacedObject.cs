using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacedObject : MonoBehaviour
{
    PlacedObjectTypeSO placedObjectTypeSO;
    Vector2Int origin;
    PlacedObjectTypeSO.Dir direction;

    public static PlacedObject Create(Vector3 worldPosition, Vector2Int origin, PlacedObjectTypeSO.Dir _direction, PlacedObjectTypeSO placedObjectTypeSO) 
    {
        Transform placedObjectTransform = Instantiate(placedObjectTypeSO.prefab, worldPosition, Quaternion.Euler(0, placedObjectTypeSO.GetRotationAngle(_direction), 0));

        PlacedObject placedObject = placedObjectTransform.GetComponent<PlacedObject>();
        
        placedObject.placedObjectTypeSO = placedObjectTypeSO;
        placedObject.origin = origin;
        placedObject.direction = _direction;

        return placedObject;
    }

    public List<Vector2Int> GetGridPositionList()
    {
        return placedObjectTypeSO.GetGridPositionList(origin, direction);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
