using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Placeable Object Database", menuName = "Databases/Placeable Object Database")]
public class PlaceableObjectsDatabaseSO : ScriptableObject
{
    [SerializeField] List<PlaceableObjectSO> placeableObjects;
    public List<PlaceableObjectSO> PlaceableObjects => placeableObjects;
}
