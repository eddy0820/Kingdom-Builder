using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [Header("Inputs")]
    [ReadOnly, SerializeField] Vector2 horizontalInput;
    [ReadOnly, SerializeField] float mouseX;
    [ReadOnly, SerializeField] float mouseY;
    [ReadOnly, SerializeField] float mouseScroll;

    PlayerControls controls;
    PlayerControls.GroundMovementActions groundMovement;
    PlayerControls.PlayerMechanicsActions playerMechanics;

    PlayerController playerController;

    private void Awake()
    {
        controls = new PlayerControls();
        groundMovement = controls.GroundMovement;
        playerMechanics = controls.PlayerMechanics;

        playerController = GetComponent<PlayerController>();

        groundMovement.HorizontalMovement.performed += ctx => 
            horizontalInput = ctx.ReadValue<Vector2>(); 

        groundMovement.MouseX.performed += ctx =>
            mouseX = ctx.ReadValue<float>();
        groundMovement.MouseY.performed += ctx =>
            mouseY = ctx.ReadValue<float>();

        groundMovement.MouseScroll.performed += ctx =>
            mouseScroll = ctx.ReadValue<float>();

        groundMovement.CameraSwitch.performed += ctx =>
            playerController.DoCameraSwitch();

        groundMovement.Jump.performed += _ =>
            playerController.Character.DoJump();
        
        groundMovement.Crouch.performed += _ =>
            playerController.Character.DoCrouchDown();
        groundMovement.Crouch.canceled += _ =>
            playerController.Character.DoCrouchUp();

        groundMovement.Walk.performed += _ =>
            playerController.Character.SetIsWalking(true);
        groundMovement.Walk.canceled += _ =>
            playerController.Character.SetIsWalking(false);

        groundMovement.Sprint.performed += _ =>
            playerController.Character.SetIsSprinting(true);
        groundMovement.Sprint.canceled += _ =>
            playerController.Character.SetIsSprinting(false);

        if(PlayerSpawner.Instance.GridBuildingInfo.EnableBuilding)
        {
            playerMechanics.ToggleBuildMode.performed += _ =>
                playerController.ToggleBuildMode();
        }
        
    }

    private void Start()
    {
        if(PlayerSpawner.Instance.GridBuildingInfo.EnableBuilding)
        {
            playerMechanics.Build.performed += _ =>
                GridBuildingManager.Instance.PlaceObject();

            playerMechanics.Demolish.performed += _ =>
                GridBuildingManager.Instance.DemolishPlacedObject();
                
            playerMechanics.Rotate.performed += ctx =>
                GridBuildingManager.Instance.Rotate(ctx.ReadValue<float>());
        }
    }

    private void Update()
    {
        playerController.SetInputs(horizontalInput.y, horizontalInput.x, mouseX, mouseY, mouseScroll);
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
