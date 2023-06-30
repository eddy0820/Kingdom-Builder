using UnityEngine;

public static class GridBuildingUtil
{
    public static void SetLayerAndMatRecursive(GameObject targetGameObject, Material mat, string layerName) 
    {
        MeshRenderer meshRenderer;
        targetGameObject.TryGetComponent<MeshRenderer>(out meshRenderer);

        if(meshRenderer != null)
        {
            meshRenderer.material = mat;
        }

        targetGameObject.layer = LayerMask.NameToLayer(layerName);
        
        foreach(Transform child in targetGameObject.transform) 
        {
            SetLayerAndMatRecursive(child.gameObject, mat, layerName);
        }
    }  

    public static void SetMatRecursive(GameObject targetGameObject, Material mat)
    {
        MeshRenderer meshRenderer;
        targetGameObject.TryGetComponent<MeshRenderer>(out meshRenderer);

        if(meshRenderer != null)
        {
            meshRenderer.material = mat;
        }

        foreach(Transform child in targetGameObject.transform) 
        {
            SetMatRecursive(child.gameObject, mat);
        }
    }

    public static void SetMatRecursive(GameObject targetGameObject, Material mat, string tag)
    {
        MeshRenderer meshRenderer;
        targetGameObject.TryGetComponent<MeshRenderer>(out meshRenderer);

        if(meshRenderer != null && targetGameObject.tag == tag)
        {
            meshRenderer.material = mat;
        }

        foreach(Transform child in targetGameObject.transform) 
        {
            SetMatRecursive(child.gameObject, mat, tag);
        }
    }

    public static void DisableMeshRendererRecursive(GameObject targetGameObject)
    {
        MeshRenderer meshRenderer;
        targetGameObject.TryGetComponent<MeshRenderer>(out meshRenderer);

        if(meshRenderer != null)
        {
            meshRenderer.enabled = false;
        }

        foreach(Transform child in targetGameObject.transform) 
        {
            DisableMeshRendererRecursive(child.gameObject);
        }
    }

    public static void DisableMeshRendererRecursive(GameObject targetGameObject, string tag)
    {
        MeshRenderer meshRenderer;
        targetGameObject.TryGetComponent<MeshRenderer>(out meshRenderer);

        if(meshRenderer != null && targetGameObject.tag == tag)
        {
            meshRenderer.enabled = false;
        }

        foreach(Transform child in targetGameObject.transform) 
        {
            DisableMeshRendererRecursive(child.gameObject, tag);
        }
    }
}
