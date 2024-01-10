using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;

public class PlayerCanvas : MonoBehaviour
{
    [Header("Bottom Bar")]
    [SerializeField] TweenedUIComponent bottomBarHUD;

    [HorizontalLine]

    [Header("Interaction")]
    [SerializeField] TweenedUIComponent interactionCrosshair;
    [Space(10)]
    [SerializeField] List<InteractionEntry> interactionEntries;

    [HorizontalLine]

    [Header("Crosshair")]
    [SerializeField] TweenedUIComponent crosshair;

    [Header("Build Menu")]
    [SerializeField] TweenedUIComponent buildMenu;

    [Header("Build Hotbar")]
    [SerializeField] TweenedUIComponent buildHotbar;

    [Space(15)]

    [ReadOnly, SerializeField] bool buildMenuEnabled = false;
    public bool BuildMenuEnabled => buildMenuEnabled;

    BuildHotbarInterface buildHotbarInterface;
    public BuildHotbarInterface BuildHotbarInterface => buildHotbarInterface;

    PlayerStatUI playerStatUI;
    public PlayerStatUI PlayerStatUI => playerStatUI;

    private void Awake()
    {
        playerStatUI = GetComponent<PlayerStatUI>();
        buildHotbarInterface = buildHotbar.GameObj.GetComponent<BuildHotbarInterface>();

        buildMenu.GameObj.SetActive(false);
        buildHotbar.GameObj.SetActive(false);

        crosshair.GameObj.SetActive(false);
        crosshair.RectTransform.localScale = Vector3.zero;

        interactionCrosshair.GameObj.SetActive(false);
        HideInteractions();
    }

#region Interaction

    public Action GetInteractionEntryActionFromIndex(int index)
    {
        return interactionEntries[index].OnInteract;
    }
    
    public void ToggleInteractionCrosshair(bool b)
    {
        interactionCrosshair.TweenUIComponent(b);
    }

    public void ShowInteractions(Interactable interactable, List<InteractionTypeSO> interactionTypes)
    {
        for(int i = 0; i < interactionTypes.Count; i++)
        {
            InteractionTypeSO interactionType = interactionTypes[i];
            InteractionEntry interactionEntry = interactionEntries[i];

            interactionEntry.SetInteraction(InputManager.Instance.GetEffectiveBindingPathForInteractionIndex(i), interactionType.Name, () => interactionType.Interact(interactable));
        }
    }

    public void HideInteractions()
    {
        interactionEntries.ForEach(entry => entry.HideInteraction());
    }

#endregion

#region Build Mode

    public void ToggleBuildMenu()
    {
        ToggleBuildMenu(!buildMenuEnabled);
        PlayerController.Instance.SoundController.PlayBuildMenuAppearanceSound(!buildMenuEnabled);
    }

    public void ToggleBuildMenu(bool b)
    {
        buildMenuEnabled = b;
        buildMenu.TweenUIComponent(b);
        ToggleBuildModeCrosshair(!b);
        bottomBarHUD.GameObj.SetActive(!b);


        if(b) Cursor.lockState = CursorLockMode.None;
        else Cursor.lockState = CursorLockMode.Locked;
    }

    public void ToggleBuildHotbar(bool b)
    {
        buildHotbar.TweenUIComponent(b);
        bottomBarHUD.TweenUIComponent(b, new(){ETweenType.MoveY}, false);
    }

    public void ToggleBuildModeCrosshair(bool b)
    {
        crosshair.TweenUIComponent(b);
    }

#endregion

}
