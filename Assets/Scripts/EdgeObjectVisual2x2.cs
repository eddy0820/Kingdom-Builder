using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeObjectVisual2x2 : MonoBehaviour
{
    bool colliding;
    public bool Colliding => colliding;

    Collider otherCollider;

    private void Update()
    {
        if(colliding && otherCollider == null)
        {
            colliding = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            colliding = true;
            otherCollider = other;
        }
 
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            colliding = true;   
            otherCollider = other;  
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            colliding = false;
        }
    }
}
