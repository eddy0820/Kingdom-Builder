using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class GridBuildingInfo : MonoBehaviour
{
    [Scene]
    [SerializeField] string gridBuildingScene;
    public string GridBuildingScene => gridBuildingScene;
    [SerializeField] Transform gridOriginPoint;
    public Transform GridOriginPoint => gridOriginPoint;

    [Space(15)]

    [SerializeField] bool enableBuilding = true;
    public bool EnableBuilding => enableBuilding;

    [Space(15)]

    [ShowIf("enableBuilding"), SerializeField] PlaceableObjectsDatabaseSO placeableObjectsDatabase;
    public PlaceableObjectsDatabaseSO PlaceableObjectsDatabase => placeableObjectsDatabase;
    [ShowIf("enableBuilding"), SerializeField] BuildingTypesDatabaseSO buildingTypesDatabase;
    public BuildingTypesDatabaseSO BuildingTypesDatabase => buildingTypesDatabase;

    [Space(15)]

    [ShowIf("enableBuilding"), SerializeField] int gridWidth = 10;
    public int GridWidth => gridWidth;
    [ShowIf("enableBuilding"), SerializeField] int gridLength = 10;
    public int GridLength => gridLength;
    [ShowIf("enableBuilding"), SerializeField] float cellSize = 10f;
    public float CellSize => cellSize;

    [Space(5)]

    [ShowIf("enableBuilding"), SerializeField] float gridHeight = 10f;
    public float GridHeight => gridHeight;

    [ShowIf("enableBuilding"), SerializeField] int gridVerticalCount = 5;
    public int GridVerticalCount => gridVerticalCount;

    [Space(5)]

    [ShowIf("enableBuilding"), SerializeField] float maxBuildDistance = 15f;
    public float MaxBuildDistance => maxBuildDistance;

    [Space(15)]

    [ShowIf("enableBuilding"), SerializeField] LayerMask edgeColliderLayerMask;
    public LayerMask EdgeColliderLayerMask => edgeColliderLayerMask;
    [ShowIf("enableBuilding"), SerializeField] LayerMask stairEdgeColliderLayerMask;
    public LayerMask StairEdgeColliderLayerMask => stairEdgeColliderLayerMask;
    [ShowIf("enableBuilding"), SerializeField] LayerMask placeableObjectsColliderLayerMask;
    public LayerMask PlaceableObjectsColliderLayerMask => placeableObjectsColliderLayerMask;

    [Space(15)]

    [SerializeField] bool debug;
    public bool Debug => debug;
    [ShowIf(EConditionOperator.And, "debug", "enableBuilding"), SerializeField] int debugFontSize = 100;
    public int DebugFontSize => debugFontSize;
    [ShowIf(EConditionOperator.And, "debug", "enableBuilding"), SerializeField] bool enableMouse3DDebug;
    public bool EnableMouse3DDebug => enableMouse3DDebug;
}
