using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
////////////// U NEED TO CHECK ALL Physics.Raycast YOU ARE USING THE COLLIDER MASK WRONG
////////////// LOOSE PLACEMENT BUG WHEN MOVING MOUSE FAST COULD BE BECAUSE OF ^^
////////////// Fix being able to place objects that can't be placed for a frame
////////////// fix edge object flipping maybe?
public class GridBuildingManager : MonoBehaviour
{
    public static GridBuildingManager Instance { get; private set; }

    [ReadOnly, SerializeField] int gridWidth = 10;
    [ReadOnly, SerializeField] int gridLength = 10;
    [ReadOnly, SerializeField] float cellSize = 10f;
    public float CellSize => cellSize;

    [Space(5)]

    [ReadOnly, SerializeField] float gridHeight = 10f;
    [ReadOnly, SerializeField] int gridVerticalCount = 5;

    [Space(5)]

    [ReadOnly, SerializeField] float maxBuildDistance = 15f;

    [Space(15)]

    [ReadOnly, SerializeField] LayerMask edgeColliderLayerMask;
    public LayerMask EdgeColliderLayerMask => edgeColliderLayerMask;
    [ReadOnly, SerializeField] LayerMask placeableObjectsColliderLayerMask;
    public LayerMask PlaceableObjectsColliderLayerMask =>placeableObjectsColliderLayerMask;

    [Header("UI")]
    [ReadOnly, SerializeField] float uIIconAnimationDelay;
    public float UIIconAnimationDelay => uIIconAnimationDelay;
    [ReadOnly, SerializeField] float uIIconAnimationSpeed;
    public float UIIconAnimationSpeed => uIIconAnimationSpeed;

    [Space(15)]

    [ReadOnly, SerializeField] bool debug;
    [ReadOnly, SerializeField] bool enableGridDebug;
    [ReadOnly, SerializeField] int debugFontSize = 100;
    [ReadOnly, SerializeField] bool enableMouse3DDebug;
    Material mouse3DDebugMaterial;
    [ReadOnly, SerializeField] bool enableFakeVisualDebug;
    Material fakeVisualMaterial;
    [ReadOnly, SerializeField] List<PlaceableObjectTypes> placeableObjectTypesFakeVisualBlacklist;
    [ReadOnly, SerializeField] List<BuildingTypes> buildingTypesFakeVisualBlacklist;
    [ReadOnly, SerializeField] bool enableVisualAnchorDebug;
    Material visualAnchorDebugMaterial;
    string identifierTag;

    [SerializeField] GameObject debugHolder;
    public GameObject DebugHolder => debugHolder; 

    List<GridXZ<GridBuildingCell>> gridList;

    GridXZ<GridBuildingCell> selectedGrid;
    public GridXZ<GridBuildingCell> SelectedGrid => selectedGrid;

    PlaceableObjectSO currentPlaceableObjectSO;
    public PlaceableObjectSO CurrentPlaceableObjectSO => currentPlaceableObjectSO;

    BuildingGhost buildingGhost;
    public BuildingGhost BuildingGhost => buildingGhost;

    GridObjectBuildingManager gridObjectBuildingManager;
    public GridObjectBuildingManager GridObjectBuildingManager => gridObjectBuildingManager;
    EdgeObjectBuildingManager edgeObjectBuildingManager;
    public EdgeObjectBuildingManager EdgeObjectBuildingManager => edgeObjectBuildingManager;
    LooseObjectBuildingManager looseObjectBuildingManager;
    public LooseObjectBuildingManager LooseObjectBuildingManager => looseObjectBuildingManager;
    
    AbstractPlaceableObjectBuildingManager currentBuildingManager;
    public AbstractPlaceableObjectBuildingManager CurrentBuildingManager => currentBuildingManager;

    public PlaceableLooseObjectSO test;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        buildingGhost = FindObjectOfType<BuildingGhost>();

        gridObjectBuildingManager = GetComponentInChildren<GridObjectBuildingManager>();
        edgeObjectBuildingManager = GetComponentInChildren<EdgeObjectBuildingManager>();
        looseObjectBuildingManager = GetComponentInChildren<LooseObjectBuildingManager>();
    }

    public void Init(int _gridWidth, int _gridLength, float _cellSize, float _gridHeight, int _gridVerticalCount, float _maxBuildDistance, LayerMask _edgeColliderLayerMask, LayerMask _placeableObjectsColliderLayerMask, float _uIIconAnimationDelay, float _uIIconAnimationSpeed, bool _debug, bool _enableGridDebug, int _debugFontSize, bool _enableMouse3DDebug, Material _mouse3DDebugMaterial, bool _enableFakeVisualDebug, Material _fakeVisualMaterial, List<PlaceableObjectTypes> _placeableObjectTypesFakeVisualBlacklist, List<BuildingTypes> _buildingTypesFakeVisualBlacklist, bool _enableVisualAnchorDebug, Material _visualAnchorDebugMaterial, string _identifierTag)
    {
        gridWidth = _gridWidth;
        gridLength = _gridLength;
        cellSize = _cellSize;
        gridHeight = _gridHeight;
        gridVerticalCount = _gridVerticalCount;
        maxBuildDistance = _maxBuildDistance;
        edgeColliderLayerMask = _edgeColliderLayerMask;
        placeableObjectsColliderLayerMask = _placeableObjectsColliderLayerMask;
        uIIconAnimationDelay = _uIIconAnimationDelay;
        uIIconAnimationSpeed = _uIIconAnimationSpeed;
        debug = _debug;
        enableGridDebug = _enableGridDebug;
        debugFontSize = _debugFontSize;
        enableMouse3DDebug = _enableMouse3DDebug;
        mouse3DDebugMaterial = _mouse3DDebugMaterial;
        enableFakeVisualDebug = _enableFakeVisualDebug;
        fakeVisualMaterial = _fakeVisualMaterial;
        placeableObjectTypesFakeVisualBlacklist = _placeableObjectTypesFakeVisualBlacklist;
        buildingTypesFakeVisualBlacklist = _buildingTypesFakeVisualBlacklist;
        enableVisualAnchorDebug = _enableVisualAnchorDebug;
        visualAnchorDebugMaterial = _visualAnchorDebugMaterial;
        identifierTag = _identifierTag;
    }

    public void Setup(Vector3 origin)
    {
        bool doGridDebug = false;
        if(debug && enableGridDebug) doGridDebug = true;

        gridList = new List<GridXZ<GridBuildingCell>>();
        
        for(int i = 0; i < gridVerticalCount; i++)
        {
            Vector3 originPos = new Vector3(origin.x, origin.y + gridHeight * i, origin.z);
            GridXZ<GridBuildingCell> grid = new GridXZ<GridBuildingCell>(gridWidth, gridLength, cellSize, originPos, (GridXZ<GridBuildingCell> g, int x, int z) => new GridBuildingCell(g, x, z), doGridDebug, debugFontSize);
            gridList.Add(grid);
        }

        selectedGrid = gridList[0];

        if(debug && enableMouse3DDebug)
        {
            Mouse3D.Instance.debugVisual.gameObject.SetActive(true);
            Mouse3D.Instance.debugVisual.GetComponent<MeshRenderer>().material = mouse3DDebugMaterial;
        }
        else
        {
            Mouse3D.Instance.debugVisual.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        HandleGridSwitch();

        if(Input.GetKeyDown(KeyCode.Equals)) SelectPlaceableObject(test);
    }

    public void Rotate(float value)
    {
        if(!PlayerController.Instance.UICanvas.BuildMenuEnabled && value > 0.1f)
        {
            currentBuildingManager.Rotate();
        }
        else
        {
            LooseObjectBuildingManager.SetRotateFalse();
        }
    }

    private void HandleGridSwitch()
    {
        Vector3 mousePosition = Mouse3D.Instance.GetMouseWorldPosition();
        int newGridIndex = Mathf.Clamp(Mathf.RoundToInt(mousePosition.y / gridHeight), 0, gridList.Count - 1);
        selectedGrid = gridList[newGridIndex];
    }

    public bool IsWithinMaxBuildDistance()
    {
        return Vector3.Distance(PlayerController.Instance.Character.transform.position, Mouse3D.Instance.GetMouseWorldPosition()) <= maxBuildDistance;
    }

    public void PlaceObject()
    {
        // Don't want to build if Build Menu Is Open
        if(!PlayerController.Instance.UICanvas.BuildMenuEnabled)
        {
            if(Mouse3D.Instance.AmILookingAtCollider() && IsWithinMaxBuildDistance() && currentPlaceableObjectSO != null)
            {
                GameObject placedObject = currentBuildingManager.PlaceObject();
                HandleVisualAnchorDebug(placedObject);
            } 
        }
    }

    public void DemolishPlacedObject()
    {
        if(!PlayerController.Instance.UICanvas.BuildMenuEnabled)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if(Physics.Raycast(ray, out RaycastHit raycastHit, 999f, Mouse3D.Instance.MouseColliderLayerMaskNoPlaceableCollider))
            {
                PlaceableObject hitObject = raycastHit.collider.GetComponentInParent<PlaceableObject>();

                if(hitObject != null)
                { 
                    currentBuildingManager.Demolish(hitObject);
                }
            }
        } 
    }

    public void SelectPlaceableObject(PlaceableObjectSO placeableObject)
    {
        if(placeableObject != null)
        {
            currentPlaceableObjectSO = placeableObject;

            switch(placeableObject.ObjectType)
            {
                case PlaceableObjectTypes.GridObject:
                    currentBuildingManager = gridObjectBuildingManager;
                break;

                case PlaceableObjectTypes.EdgeObject:
                    currentBuildingManager = edgeObjectBuildingManager;
                break;

                case PlaceableObjectTypes.LooseObject:
                    currentBuildingManager = looseObjectBuildingManager;
                break;
            }

            buildingGhost.SwitchBuildingGhost();
        }
    }

    public void GetFakeVisualDebugInfo(out bool _debug, out bool _enableFakeVisualDebug, out Material _fakeVisualMaterial, out List<PlaceableObjectTypes> _placeableObjectTypesFakeVisualBlacklist, out List<BuildingTypes> _buildingTypesFakeVisualBlacklist, out bool _enableVisualAnchorDebug, out Material _visualAnchorDebugMaterial, out string _identifierTag)
    {
        _debug = debug;
        _enableFakeVisualDebug = enableFakeVisualDebug;
        _fakeVisualMaterial = fakeVisualMaterial;
        _placeableObjectTypesFakeVisualBlacklist = placeableObjectTypesFakeVisualBlacklist;
        _buildingTypesFakeVisualBlacklist = buildingTypesFakeVisualBlacklist;
        _enableVisualAnchorDebug = enableVisualAnchorDebug;
        _visualAnchorDebugMaterial = visualAnchorDebugMaterial;
        _identifierTag = identifierTag;
    }

    private void HandleVisualAnchorDebug(GameObject gameObj)
    {
        if(gameObj == null) return;

        if(debug && enableVisualAnchorDebug)
        {
            GridBuildingUtil.SetMatRecursive(gameObj, visualAnchorDebugMaterial, identifierTag);
            return;
        }

        GridBuildingUtil.DisableMeshRendererRecursive(gameObj, identifierTag);
    }
}
