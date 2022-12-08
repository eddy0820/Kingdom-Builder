using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LooseObjectVisual : MonoBehaviour
{
    bool colliding;
    public bool Colliding => colliding;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Placeable Objects Collider"))
        {
            return;
        }

        colliding = true;
        
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Placeable Objects Collider"))
        {
            return;
        }

        colliding = false;
        
    }
}
