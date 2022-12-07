using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingGhost : MonoBehaviour {

    [SerializeField] Material ghostMaterial;
    private Transform visual;

    private void Start() 
    {
        RefreshVisual();

        GridBuildingManager.Instance.OnSelectedChanged += Instance_OnSelectedChanged;
    }

    private void Instance_OnSelectedChanged(object sender, System.EventArgs e) 
    {
        RefreshVisual();
    }

    private void LateUpdate() 
    {
        Vector3 targetPosition = GridBuildingManager.Instance.GetMouseWorldSnappedPosition();
        targetPosition.y = 1f;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 15f);

        transform.rotation = Quaternion.Lerp(transform.rotation, GridBuildingManager.Instance.GetGridObjectRotation(), Time.deltaTime * 15f);
    }

    private void RefreshVisual() 
    {
        if (visual != null) 
        {
            Destroy(visual.gameObject);
            visual = null;
        }

        GridObjectSO gridObjectSO = GridBuildingManager.Instance.GetGridObjectType();

        if (gridObjectSO != null) 
        {
            visual = Instantiate(gridObjectSO.visual, Vector3.zero, Quaternion.identity);
            visual.parent = transform;
            visual.localPosition = Vector3.zero;
            visual.localEulerAngles = Vector3.zero;
            SetLayerRecursive(visual.gameObject, ghostMaterial);
        }
    }

    private void SetLayerRecursive(GameObject targetGameObject, Material mat) 
    {
        MeshRenderer meshRenderer;
        targetGameObject.TryGetComponent<MeshRenderer>(out meshRenderer);

        if(meshRenderer != null)
        {
            meshRenderer.material = mat;
        }
        
        foreach (Transform child in targetGameObject.transform) 
        {
            SetLayerRecursive(child.gameObject, mat);
        }
    }

}

