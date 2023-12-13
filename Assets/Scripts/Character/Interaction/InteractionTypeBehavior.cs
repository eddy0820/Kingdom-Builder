using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractionTypeBehavior : MonoBehaviour
{
    protected PlayerController PlayerController => PlayerController.Instance;

    public abstract void Interact(Interactable interactable);
}
