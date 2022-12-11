using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse3D : MonoBehaviour {

    public static Mouse3D Instance { get; private set; }

    [SerializeField] LayerMask mouseColliderLayerMask = new LayerMask();
    public LayerMask MouseColliderLayerMask => mouseColliderLayerMask;

    [SerializeField] LayerMask mouseColliderLayerMaskNoPlaceableCollider = new LayerMask();
    public LayerMask MouseColliderLayerMaskNoPlaceableCollider => mouseColliderLayerMaskNoPlaceableCollider;
    [SerializeField] LayerMask placeableColliderLayer = new LayerMask();
    
    LayerMask currentLayerMask;
    public LayerMask CurrentLayerMask => currentLayerMask;
    public Transform debugVisual;

    private void Awake() 
    {
        Instance = this;
        currentLayerMask = mouseColliderLayerMask;
    }

    private void Update() 
    {   
        HandleIfLookingAtPlaceableColliderTop();

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        
        if(Physics.Raycast(ray, out RaycastHit raycastHit, 999f, currentLayerMask)) 
        {
            transform.position = raycastHit.point;
        }

        if(debugVisual != null) debugVisual.position = GetMouseWorldPosition();
    }

    private void HandleIfLookingAtPlaceableColliderTop()
    {
        GameObject mouseObj = GetMouseGameObject(placeableColliderLayer);

        if(GridBuildingManager.Instance.CurrentPlaceableObjectSO is LooseObjectSO || (mouseObj != null && CheckIfLookingAtTopOrSides(placeableColliderLayer) && PlayerController.Instance.Character.transform.position.y + PlayerController.Instance.Character.Motor.Capsule.height < GetMouseWorldPosition(placeableColliderLayer).y))
        {
            currentLayerMask = mouseColliderLayerMaskNoPlaceableCollider;
        }
        else
        {
            currentLayerMask = mouseColliderLayerMask;
        }
    }

    public bool CheckIfLookingAtTopOrSides(LayerMask mask)
    {
        switch(GetMouseBoxColliderSurfaceDirection(mask))
        {
            case BoxColliderSurface.Up:
                return true;
            case BoxColliderSurface.North:
                return true;
            case BoxColliderSurface.South:
                return true;    
            case BoxColliderSurface.West:
                return true;
            case BoxColliderSurface.East:
                return true;
            default:
                return false;
        }
    }

    public int GetMouseWorldLayer()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Physics.Raycast(ray, out RaycastHit raycastHit, 999f, currentLayerMask);
        
        return raycastHit.transform.gameObject.layer;
    }

    public Vector3 GetMouseWorldPosition() 
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out RaycastHit raycastHit, 999f, currentLayerMask)) 
        {
            return raycastHit.point;
        } 
        else 
        {
            return Vector3.zero;
        }
    }

    public Vector3 GetMouseWorldPosition(LayerMask mask) 
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out RaycastHit raycastHit, 999f, mask)) 
        {
            return raycastHit.point;
        } 
        else 
        {
            return Vector3.zero;
        }
    }

    public GameObject GetMouseGameObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Physics.Raycast(ray, out RaycastHit raycastHit, 999f, currentLayerMask);
        
        if(raycastHit.transform != null)
            return raycastHit.transform.gameObject;
        else
            return null;
    }

    public GameObject GetMouseGameObject(LayerMask mask)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Physics.Raycast(ray, out RaycastHit raycastHit, 999f, mask);
        
        if(raycastHit.transform != null)
            return raycastHit.transform.gameObject;
        else
            return null;
    }

    public BoxColliderSurface GetMouseBoxColliderSurfaceDirection(LayerMask mask)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Physics.Raycast(ray, out RaycastHit raycastHit, 999f, mask);
        
        return GetHitFace(raycastHit);
    }

    public enum BoxColliderSurface
    {
        None,
        Up,
        Down,
        East,
        West,
        North,
        South
    }
 
    public BoxColliderSurface GetHitFace(RaycastHit hit)
    {
        Vector3 incomingVec = hit.normal - Vector3.up;
 
        if (incomingVec == new Vector3(0, -1, -1))
            return BoxColliderSurface.South;
 
        if (incomingVec == new Vector3(0, -1, 1))
            return BoxColliderSurface.North;
 
        if (incomingVec == new Vector3(0, 0, 0))
            return BoxColliderSurface.Up;
 
        if (incomingVec == new Vector3(0, -2, 0))
            return BoxColliderSurface.Down;
 
        if (incomingVec == new Vector3(-1, -1, 0))
            return BoxColliderSurface.West;
 
        if (incomingVec == new Vector3(1, -1, 0))
            return BoxColliderSurface.East;
 
        return BoxColliderSurface.None;
    }
}    