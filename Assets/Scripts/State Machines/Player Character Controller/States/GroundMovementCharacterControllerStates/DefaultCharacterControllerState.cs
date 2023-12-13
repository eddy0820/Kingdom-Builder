using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultCharacterControllerState : GroundMovementCharacterControllerState
{
    [SerializeField] StatTypeSO interactionRangeStatType;
    [Space(10)]
    [SerializeField] LayerMask interactionLayerMask;

    PlayerCanvas PlayerCanvas => PlayerController.UICanvas;
    Stat InteractionRangeStat => PlayerController.PlayerStats.GetStatFromType[interactionRangeStatType];

    bool canInteractWithSomething = false;
    public bool CanInteractWithSomething => canInteractWithSomething;

    public override void OnEnterState(PlayerCharacterControllerState fromState)
    {

    }

    public override void OnExitState(PlayerCharacterControllerState toState)
    {
        canInteractWithSomething = false;

        PlayerCanvas.ToggleInteractionCrosshair(false);
        PlayerCanvas.HideInteractions();
    }

    public override void OnFixedUpdateState()
    {
        if(PlayerController.PlayerStats.IsDead()) return;

        Vector3 screenCenter = new(Screen.width / 2f, Screen.height / 2f, 0f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);

        if(Physics.Raycast(ray, out RaycastHit hit, InteractionRangeStat.Value, interactionLayerMask, QueryTriggerInteraction.Collide))
        {
            if(hit.collider.TryGetComponent(out Interactable interactable))
            {
                if(!canInteractWithSomething) ToggleInteractions(true, interactable, interactable.InteractionTypes);
            }
            else
            {
                if(canInteractWithSomething) ToggleInteractions(false);
            }      
        } 
        else
        {
            if(canInteractWithSomething) ToggleInteractions(false);
        }
    }

    private void ToggleInteractions(bool toggle, Interactable interactable = null, List<InteractionTypeSO> interactionTypes = null)
    {
        canInteractWithSomething = toggle;
        PlayerCanvas.ToggleInteractionCrosshair(toggle);

        if(toggle)
            PlayerCanvas.ShowInteractions(interactable, interactionTypes);
        else
            PlayerCanvas.HideInteractions(); 
    }
}
