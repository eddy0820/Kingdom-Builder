using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.InputSystem;
using UnityEngine.Events;

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

    [HideInInspector] public InputManagerEvent OnNumberKeyPressed;

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

        OnNumberKeyPressed = new InputManagerEvent();

        playerController = GetComponent<PlayerController>();

        //////////////// Horizonal Movement ////////////////
        groundMovement.HorizontalMovement.performed += ctx => 
            horizontalInput = ctx.ReadValue<Vector2>(); 

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

        //////////////// Player Mechanics ////////////////
        playerMechanics.MouseX.performed += ctx =>
            mouseX = ctx.ReadValue<float>();
        playerMechanics.MouseY.performed += ctx =>
            mouseY = ctx.ReadValue<float>();

        playerMechanics.CameraSwitch.performed += ctx =>
            playerController.DoCameraSwitch();

        if(PlayerSpawner.Instance.GridBuildingInfo.EnableBuilding)
        {
            playerMechanics.ToggleBuildMode.performed += _ =>
                playerController.ToggleBuildMode(); // do function
        }

        playerMechanics.MouseScroll.performed += ctx =>
            DoMouseScrollControl(ctx.ReadValue<float>());

        SetupNumberKeys();
    }

    private void SetupNumberKeys()
    {
        playerMechanics.NumberKey1.performed += _ =>
            DistributeNumber(1);
        playerMechanics.NumberKey2.performed += _ =>
            DistributeNumber(2);
        playerMechanics.NumberKey3.performed += _ =>
            DistributeNumber(3);
        playerMechanics.NumberKey4.performed += _ =>
            DistributeNumber(4);
        playerMechanics.NumberKey5.performed += _ =>
            DistributeNumber(5);
        playerMechanics.NumberKey6.performed += _ =>
            DistributeNumber(6);
        playerMechanics.NumberKey7.performed += _ =>
            DistributeNumber(7);
        playerMechanics.NumberKey8.performed += _ =>
            DistributeNumber(8);
        playerMechanics.NumberKey9.performed += _ =>
            DistributeNumber(9);
    }

    private void Start()
    {
        if(PlayerSpawner.Instance.GridBuildingInfo.EnableBuilding)
        {
            //////////////// Grid Building ////////////////
            gridBuilding.Build.performed += _ =>
                GridBuildingManager.Instance.PlaceObject();

            gridBuilding.Demolish.performed += _ =>
                GridBuildingManager.Instance.DemolishPlacedObject();
                
            gridBuilding.Rotate.performed += ctx =>
                GridBuildingManager.Instance.Rotate(ctx.ReadValue<float>());
            
            gridBuilding.ToggleBuildMenu.performed += ctx =>
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
        gridBuilding.Disable();
    }

    private void DistributeNumber(int num)
    {
        OnNumberKeyPressed.Invoke(num);
    }

    private void DoMouseScrollControl(float _mouseScroll)
    {
        if(!PlayerController.Instance.BuildModeEnabled)
        {
            mouseScroll = _mouseScroll;
        }
    }

    [System.Serializable]
    public class InputManagerEvent : UnityEvent<int> {}
}
