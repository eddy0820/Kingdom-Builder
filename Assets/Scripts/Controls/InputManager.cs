using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    
    [Header("Inputs")]
    [ReadOnly, SerializeField] Vector2 horizontalInput;
    [ReadOnly, SerializeField] float mouseX;
    [ReadOnly, SerializeField] float mouseY;
    [ReadOnly, SerializeField] float mouseScroll;

    PlayerControls controls;
    PlayerControls.GroundMovementActions groundMovement;
    PlayerControls.PlayerMechanicsActions playerMechanics;
    PlayerControls.GridBuildingActions gridBuilding;
    public PlayerControls.GridBuildingActions GridBuilding => gridBuilding;

    PlayerController playerController;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        controls = new PlayerControls();
        groundMovement = controls.GroundMovement;
        playerMechanics = controls.PlayerMechanics;
        gridBuilding = controls.GridBuilding;

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
            gridBuilding.Build.performed += _ =>
                GridBuildingManager.Instance.PlaceObject();

            gridBuilding.Demolish.performed += _ =>
                GridBuildingManager.Instance.DemolishPlacedObject();
                
            gridBuilding.Rotate.performed += ctx =>
                GridBuildingManager.Instance.Rotate(ctx.ReadValue<float>());
            
            gridBuilding.BuildMenu.performed += ctx =>
                playerController.UICanvas.ToggleBuildMenu();
        }
    }

    private void Update()
    {
        playerController.SetInputs(horizontalInput.y, horizontalInput.x, mouseX, mouseY, mouseScroll);
    }

    private void OnEnable()
    {
        groundMovement.Enable();
        playerMechanics.Enable();
    }

    private void OnDisable()
    {
        groundMovement.Disable();
        playerMechanics.Disable();
    }
}
