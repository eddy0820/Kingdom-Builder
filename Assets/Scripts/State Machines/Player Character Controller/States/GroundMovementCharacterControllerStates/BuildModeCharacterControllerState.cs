using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildModeCharacterControllerState : GroundMovementCharacterControllerState
{
    InputManager inputManager => InputManager.Instance;
    PlayerCanvas playerCanvas => PlayerController.Instance.UICanvas;
    GridBuildingManager gridBuildingManager => GridBuildingManager.Instance;
    BuildingGhost buildingGhost => gridBuildingManager.BuildingGhost;
    GridBuildingSoundController gridBuildingSoundController => gridBuildingManager.SoundController;

    DefaultCharacterControllerState defaultState;

    public override void OnAwake()
    {
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
        inputManager.GridBuilding.Enable();
        PlayerCamera.DoBuildModeCamera(true);

        playerCanvas.ToggleCrosshair(true);
        playerCanvas.ToggleBuildHotbar(true);

        buildingGhost.RefreshVisual();
        gridBuildingSoundController.PlayToggleBuildingSound(true);
    }

    public override void OnExitState(PlayerCharacterControllerState toState)
    {
        inputManager.GridBuilding.Disable();
        PlayerCamera.DoBuildModeCamera(false);

        playerCanvas.ToggleBuildHotbar(false);
        playerCanvas.ToggleBuildMenu(false);
        playerCanvas.ToggleCrosshair(false);

        buildingGhost.RefreshVisual();
        gridBuildingSoundController.PlayToggleBuildingSound(false);
    }
}
