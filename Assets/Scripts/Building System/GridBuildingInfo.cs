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

    [HorizontalLine]

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
    [ShowIf("enableBuilding"), SerializeField] LayerMask placeableObjectsColliderLayerMask;
    public LayerMask PlaceableObjectsColliderLayerMask => placeableObjectsColliderLayerMask;

    [Header("UI")]

    [ShowIf("enableBuilding"), SerializeField] float uIIconAnimationDelay = 1;
    public float UIIconAnimationDelay => uIIconAnimationDelay;
    [ShowIf("enableBuilding"), SerializeField] float uIconAnimationSpeed = 0.04f;
    public float UIIconAnimationSpeed => uIconAnimationSpeed;

    [Space(15)]

    [ShowIf("enableBuilding"), SerializeField] bool debug;
    public bool Debug => debug;

    [HorizontalLine]

    [ShowIf(EConditionOperator.And, "debug", "enableBuilding"), SerializeField] bool enableGridDebug;
    public bool EnableGridDebug => enableGridDebug;
    [ShowIf(EConditionOperator.And, "debug", "enableBuilding", "enableGridDebug"), SerializeField] int debugFontSize = 100;
    public int DebugFontSize => debugFontSize;

    [Space(10)]

    [ShowIf(EConditionOperator.And, "debug", "enableBuilding"), SerializeField] bool enableMouse3DDebug;
    public bool EnableMouse3DDebug => enableMouse3DDebug;
    [ShowIf(EConditionOperator.And, "debug", "enableBuilding", "enableMouse3DDebug"), SerializeField] Material mouse3DDebugMaterial;
    public Material Mouse3DDebugMaterial => mouse3DDebugMaterial;

    [Space(10)]

    [ShowIf(EConditionOperator.And, "debug", "enableBuilding"), SerializeField] bool enableFakeVisualDebug;
    public bool EnableFakeVisualDebug => enableFakeVisualDebug;
    [ShowIf(EConditionOperator.And, "debug", "enableBuilding", "enableFakeVisualDebug"), SerializeField] Material fakeVisualMaterial;
    public Material FakeVisualMaterial => fakeVisualMaterial;
    [ShowIf(EConditionOperator.And, "debug", "enableBuilding", "enableFakeVisualDebug"), SerializeField] List<PlaceableObjectTypes> placeableObjectTypesFakeVisualBlacklist;
    public List<PlaceableObjectTypes> PlaceableObjectTypesFakeVisualBlacklist => placeableObjectTypesFakeVisualBlacklist;
    [ShowIf(EConditionOperator.And, "debug", "enableBuilding", "enableFakeVisualDebug"), SerializeField] List<BuildingTypes> buildingTypesFakeVisualBlacklist;
    public List<BuildingTypes> BuildingTypesFakeVisualBlacklist => buildingTypesFakeVisualBlacklist;

    [Space(10)]

    [ShowIf(EConditionOperator.And, "debug", "enableBuilding"), SerializeField] bool enableVisualAnchorDebug;
    public bool EnableVisualAnchorDebug => enableVisualAnchorDebug;
    [ShowIf(EConditionOperator.And, "debug", "enableBuilding", "enableVisualAnchorDebug"), SerializeField] Material visualAnchorDebugMaterial;
    public Material VisualAnchorDebugMaterial => visualAnchorDebugMaterial;
    [Tag]
    [ShowIf(EConditionOperator.And, "debug", "enableBuilding", "enableVisualAnchorDebug"), SerializeField] string identifierTag;
    public string IdentifierTag => identifierTag;
}
