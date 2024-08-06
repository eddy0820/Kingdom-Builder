using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildModeCharacterControllerState : GroundMovementCharacterControllerState
{
    InputManager InputManager => InputManager.Instance;
    PlayerCanvas PlayerCanvas => PlayerController.Instance.UICanvas;
    GridBuildingManager GridBuildingManager => GridBuildingManager.Instance;
    BuildingGhost BuildingGhost => GridBuildingManager.BuildingGhost;
    GridBuildingSoundController GridBuildingSoundController => GridBuildingManager.SoundController;

    DefaultCharacterControllerState defaultState;

    public override void OnAwake()
    {
        base.OnAwake();
        stateMachine.GetState(out defaultState);
    }

    public void DecideBuildMode()
    {
        if(stateMachine.CurrentState == this)
            stateMachine.SwitchState(defaultState);
        else
            stateMachine.SwitchState(this);
    }

    public override void OnEnterState(PlayerCharacterControllerState fromState)
    {
        InputManager.GridBuilding.Enable();

        if(!PlayerCamera.inFirstPerson) PlayerCamera.TweenToBuildModeFollowPointFraming();

        PlayerCanvas.ToggleBuildModeCrosshair(true);
        PlayerCanvas.ToggleBuildHotbar(true);

        BuildingGhost.RefreshVisual();
        GridBuildingSoundController.PlayToggleBuildingSound(true);
    }

    public override void OnExitState(PlayerCharacterControllerState toState)
    {
        InputManager.GridBuilding.Disable();
        
        if(!PlayerCamera.inFirstPerson) PlayerCamera.TweenToDefaultFollowPointFraming();

        PlayerCanvas.ToggleBuildHotbar(false);
        PlayerCanvas.ToggleBuildMenu(false);
        PlayerCanvas.ToggleBuildModeCrosshair(false);

        BuildingGhost.RefreshVisual();
        GridBuildingSoundController.PlayToggleBuildingSound(false);
    }
}
