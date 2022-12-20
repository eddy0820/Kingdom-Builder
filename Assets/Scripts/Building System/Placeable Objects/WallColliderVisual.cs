using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallColliderVisual : MonoBehaviour
{
    bool colliding;
    public bool Colliding => colliding;

    Collider otherCollider;

    private void Update()
    {
        if(colliding && otherCollider == null && gameObject.layer == LayerMask.NameToLayer("Building Ghost"))
        {
            colliding = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Wall") && gameObject.layer == LayerMask.NameToLayer("Building Ghost"))
        {
            colliding = true;
            otherCollider = other;
        }
 
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Wall") && gameObject.layer == LayerMask.NameToLayer("Building Ghost"))
        {
            colliding = true;   
            otherCollider = other;  
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Wall") && gameObject.layer == LayerMask.NameToLayer("Building Ghost"))
        {
            colliding = false;
        }
    }
}
