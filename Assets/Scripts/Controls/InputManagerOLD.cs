using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using EddyLib.GameSettingsSystem;

public class InputManagerOLD : MonoBehaviour
{
    public static InputManagerOLD Instance { get; private set; }
    
    [Header("Inputs")]
    [ReadOnly, SerializeField] Vector2 horizontalInput;
    [ReadOnly, SerializeField] float mouseX;
    [ReadOnly, SerializeField] float mouseY;
    [ReadOnly, SerializeField] float mouseScroll;

    [Header("Misc")]
    [SerializeField] string mouseKeyboardSchemeName = "Mouse + Keyboard";

    GameControls controls;
    GameControls.PlayerMovementActions groundMovement;
    GameControls.PlayerMechanicsActions playerMechanics;
    GameControls.GridBuildingActions gridBuilding;
    public GameControls.GridBuildingActions GridBuilding => gridBuilding;

    PlayerController playerController;
    PlayerCamera PlayerCamera => playerController.CharacterCamera;
    PlayerCanvas PlayerCanvas => playerController.UICanvas;
    PlayerCharacterController PlayerCharacterController => playerController.Character;
    EquippedWeaponStateMachine EquippedWeaponStateMachine => playerController.EquippedWeaponStateMachine;

    BuildModeCharacterControllerState buildModeState;
    LockedOnCharacterControllerState lockedOnState;

    bool EnableTargeting => GameSettings.GetSettings<TargetingSettings>().EnableTargeting;

    [HideInInspector] public InputManagerEvent OnNumberKeyPressed;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        controls = new GameControls();
        groundMovement = controls.PlayerMovement;
        playerMechanics = controls.PlayerMechanics;
        gridBuilding = controls.GridBuilding;

        OnNumberKeyPressed = new InputManagerEvent();

        playerController = GetComponent<PlayerController>();
    
        playerController.StateMachine.OnStateMachineInitialized += () =>
        {
            playerController.StateMachine.GetState(out buildModeState);
            playerController.StateMachine.GetState(out lockedOnState);
        };

        //////////////// Horizonal Movement ////////////////
        // groundMovement.HorizontalMovement.performed += ctx => 
        //     horizontalInput = ctx.ReadValue<Vector2>(); 

        groundMovement.Jump.performed += _ =>
            PlayerCharacterController.DoJump();
        
        groundMovement.Crouch.performed += _ =>
            PlayerCharacterController.DoCrouchDown();
        groundMovement.Crouch.canceled += _ =>
            PlayerCharacterController.DoCrouchUp();

        groundMovement.Walk.performed += _ =>
            PlayerCharacterController.SetIsWalking(true);
        groundMovement.Walk.canceled += _ =>
            PlayerCharacterController.SetIsWalking(false);

        groundMovement.Sprint.performed += _ =>
            PlayerCharacterController.SetIsSprinting(true);
        groundMovement.Sprint.canceled += _ =>
            PlayerCharacterController.SetIsSprinting(false);

        //////////////// Player Mechanics ////////////////
        // playerMechanics.MouseX.performed += ctx =>
        //     mouseX = ctx.ReadValue<float>();
        // playerMechanics.MouseY.performed += ctx =>
        //     mouseY = ctx.ReadValue<float>();

        // playerMechanics.CameraSwitch.performed += _ =>
        //     PlayerCamera.FlipCameraPerspective();

        // playerMechanics.FlipCameraAlignment.performed += _ =>
        //     PlayerCamera.FlipCameraAlignment();
        
        playerMechanics.PrimaryInteraction.performed += _ =>
            PlayerCanvas.GetInteractionEntryActionFromIndex(0)?.Invoke();

        playerMechanics.SecondaryInteraction.performed += _ =>
            PlayerCanvas.GetInteractionEntryActionFromIndex(1)?.Invoke();

        playerMechanics.TertiaryInteraction.performed += _ =>
            PlayerCanvas.GetInteractionEntryActionFromIndex(2)?.Invoke();

        playerMechanics.Sheath.performed += _ =>
            EquippedWeaponStateMachine.DecideSheath();



        // if(PlayerSpawnerOLD.Instance.GridBuildingInfo.EnableBuilding)
        // {
        //     playerMechanics.ToggleBuildMode.performed += _ =>
        //     {
        //         if(buildModeState != null)
        //             buildModeState.DecideBuildMode();
        //         else
        //             Debug.LogError("Build Mode State is null!");
        //     };
        // }

        // playerMechanics.MouseScroll.performed += ctx =>
        //     DoMouseScrollControl(ctx.ReadValue<float>());

        playerMechanics.LockOn.performed += _ =>
        {
            if(EnableTargeting)
                lockedOnState.DecideLockOn();
        };

        SetupNumberKeys();
    }

    private void SetupNumberKeys()
    {
        // playerMechanics.NumberKey1.performed += _ =>
        //     DistributeNumber(1);
        // playerMechanics.NumberKey2.performed += _ =>
        //     DistributeNumber(2);
        // playerMechanics.NumberKey3.performed += _ =>
        //     DistributeNumber(3);
        // playerMechanics.NumberKey4.performed += _ =>
        //     DistributeNumber(4);
        // playerMechanics.NumberKey5.performed += _ =>
        //     DistributeNumber(5);
        // playerMechanics.NumberKey6.performed += _ =>
        //     DistributeNumber(6);
        // playerMechanics.NumberKey7.performed += _ =>
        //     DistributeNumber(7);
        // playerMechanics.NumberKey8.performed += _ =>
        //     DistributeNumber(8);
        // playerMechanics.NumberKey9.performed += _ =>
        //     DistributeNumber(9);
    }

    private void Start()
    {
        // if(PlayerSpawnerOLD.Instance.GridBuildingInfo.EnableBuilding)
        // {
        //     //////////////// Grid Building ////////////////
        //     gridBuilding.Build.performed += _ =>
        //         GridBuildingManager.Instance.PlaceObject();

        //     gridBuilding.Demolish.performed += _ =>
        //         GridBuildingManager.Instance.DemolishPlacedObject();
                
        //     gridBuilding.Rotate.performed += ctx =>
        //         GridBuildingManager.Instance.Rotate(ctx.ReadValue<float>());
            
        //     gridBuilding.ToggleBuildMenu.performed += ctx =>
        //         PlayerCanvas.ToggleBuildMenu();
        // }
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

    public string GetEffectiveBindingPathForInteractionIndex(int index)
    {
        switch(index)
        {
            case 0:
                return GetEffectPathFromInteractionAction(playerMechanics.PrimaryInteraction);
            case 1:
                return GetEffectPathFromInteractionAction(playerMechanics.SecondaryInteraction);
            case 2:
                return GetEffectPathFromInteractionAction(playerMechanics.TertiaryInteraction);
            default:
                return "";
        }
    }

    private string GetEffectPathFromInteractionAction(InputAction action)
    {
        List<InputBinding> bindings = new();

        foreach(InputBinding binding in action.bindings)
        {
            if(binding.groups.Contains(mouseKeyboardSchemeName))
                bindings.Add(binding);
        }

        if(bindings.Count > 0)
            return bindings[0].effectivePath;
        else
            return "";
    }


    [System.Serializable]
    public class InputManagerEvent : UnityEvent<int> {}
}
