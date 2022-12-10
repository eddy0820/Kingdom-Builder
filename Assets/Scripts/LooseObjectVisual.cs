using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LooseObjectVisual : MonoBehaviour
{
    bool colliding;
    public bool Colliding => colliding;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Placeable Objects Collider") ||
           other.gameObject.layer == LayerMask.NameToLayer("Placeable Collider"))
        {
            return;
        }

        colliding = true;
        
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Placeable Objects Collider") ||
           other.gameObject.layer == LayerMask.NameToLayer("Placeable Collider"))
        {
            return;
        }

        colliding = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Placeable Objects Collider") ||
           other.gameObject.layer == LayerMask.NameToLayer("Placeable Collider"))
        {
            return;
        }

        colliding = false;
        
    }
}
