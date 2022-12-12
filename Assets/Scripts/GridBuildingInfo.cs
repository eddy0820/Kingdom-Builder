using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField] bool enableStrictPlacement = true;
    public bool EnableStrictPlacement => enableStrictPlacement;

    [Space(15)]

    [SerializeField] List<PlaceableObjectSO> placeableObjectSOList;
    public List<PlaceableObjectSO> PlaceableObjectSOList => placeableObjectSOList;

    [Space(15)]

    [SerializeField] int gridWidth = 10;
    public int GridWidth => gridWidth;
    [SerializeField] int gridLength = 10;
    public int GridLength => gridLength;
    [SerializeField] float cellSize = 10f;
    public float CellSize => cellSize;

    [Space(5)]

    [SerializeField] float gridHeight = 10f;
    public float GridHeight => gridHeight;

    [SerializeField] int gridVerticalCount = 5;
    public int GridVerticalCount => gridVerticalCount;

    [Space(5)]

    [SerializeField] float maxBuildDistance = 15f;
    public float MaxBuildDistance => maxBuildDistance;

    [Space(15)]

    [SerializeField] LayerMask edgeColliderLayerMask;
    public LayerMask EdgeColliderLayerMask => edgeColliderLayerMask;
    [SerializeField] LayerMask placeableObjectsColliderLayerMask;
    public LayerMask PlaceableObjectsColliderLayerMask => placeableObjectsColliderLayerMask;

    [Space(15)]

    [SerializeField] bool debug;
    public bool Debug => debug;
    [SerializeField] int debugFontSize = 100;
    public int DebugFontSize => debugFontSize;
    [SerializeField] bool enableMouse3DDebug;
    public bool EnableMouse3DDebug => enableMouse3DDebug;
}
