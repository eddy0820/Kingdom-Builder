using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;
using NaughtyAttributes;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [SerializeField] PlayerCharacterController character;
    public PlayerCharacterController Character => character;
    [SerializeField] PlayerCharacterStateMachine stateMachine;
    public PlayerCharacterStateMachine StateMachine => stateMachine;
    [SerializeField] PlayerCamera characterCamera;
    public PlayerCamera CharacterCamera => characterCamera;
    [SerializeField] PlayerCanvas uiCanvas;
    public PlayerCanvas UICanvas => uiCanvas;
    [SerializeField] PlayerSoundController soundController;
    public PlayerSoundController SoundController => soundController;
    [SerializeField] PlayerAnimationController animationController;
    public PlayerAnimationController AnimationController => animationController;

    [Space(10)]

    [Expandable, SerializeField] BaseStatsSO baseStatsSO;
    public BaseStatsSO BaseStatsSO => baseStatsSO;

    PlayerStats playerStats;
    public PlayerStats PlayerStats => playerStats;
    public IDamageable PlayerStatsDamageable => playerStats;

    [Expandable, SerializeField] PlayerAttributesSO attributesSO;
    public PlayerAttributesSO AttributesSO => attributesSO;

    private float verticalInput;
    private float horizontalInput;
    private float mouseXInput;
    private float mouseYInput;
    private float mouseScrollInput;

    bool buildModeEnabled => stateMachine.CurrentState is BuildModeCharacterControllerState;
    public bool BuildModeEnabled => buildModeEnabled;

    bool lockedOn => stateMachine.CurrentState is LockedOnCharacterControllerState;
    public bool LockedOn => lockedOn;

    public Action OnEnterFirstPerson;
    public Action OnExitFirstPerson;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }

        playerStats = new PlayerStats(baseStatsSO);
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        // Tell camera to follow transform
        characterCamera.SetFollowTransform(character.CameraFollowPoint);

        // Ignore the character's collider(s) for camera obstruction checks
        characterCamera.IgnoredColliders.Clear();
        characterCamera.IgnoredColliders.AddRange(character.GetComponentsInChildren<Collider>());

        playerStats.SetHealth(playerStats.GetStatFromName[CommonStatTypeNames.MaxHealth].Value);
    }
    
    private void Update()
    {
        //if(Input.GetMouseButtonDown(0)) Cursor.lockState = CursorLockMode.Locked;

        HandleCharacterInput();

        CheckIfFirstPerson();

        if(Input.GetKeyUp(KeyCode.L))
        {
            PlayerStatsDamageable.TakeDamage(10);
        }
    }

    private void LateUpdate()
    {
        // Handle rotating the camera along with physics movers
        if(characterCamera.RotateWithPhysicsMover && character.Motor.AttachedRigidbody != null)
        {
            characterCamera.PlanarDirection = character.Motor.AttachedRigidbody.GetComponent<PhysicsMover>().RotationDeltaFromInterpolation * characterCamera.PlanarDirection;
            characterCamera.PlanarDirection = Vector3.ProjectOnPlane(characterCamera.PlanarDirection, character.Motor.CharacterUp).normalized;
        }

        HandleCameraInput();
    }

    private void HandleCameraInput()
    {
        // Create the look input vector for the camera
        float mouseLookAxisUp = mouseYInput;
        float mouseLookAxisRight = mouseXInput;
        Vector3 lookInputVector = new Vector3(mouseLookAxisRight, mouseLookAxisUp, 0f);

        // Prevent moving the camera while the cursor isn't locked
        if(Cursor.lockState != CursorLockMode.Locked || lockedOn)
        {
            lookInputVector = Vector3.zero;
        }

        // Input for zooming the camera (disabled in WebGL because it can cause problems)
        float scrollInput = -mouseScrollInput;

        #if UNITY_WEBGL
            scrollInput = 0f;
        #endif

        // Apply inputs to the camera
        characterCamera.UpdateWithInput(Time.deltaTime, scrollInput, lookInputVector);
    }

    private void HandleCharacterInput()
    {
        PlayerCharacterInputs characterInputs = new PlayerCharacterInputs
        {
            // Build the CharacterInputs struct
            MoveAxisForward = verticalInput,
            MoveAxisRight = horizontalInput
        };

        if(!lockedOn)
            characterInputs.CameraRotation = characterCamera.FollowCameraTransform.rotation;
        else
            characterInputs.CameraRotation = characterCamera.LockOnCameraTransform.rotation;

        // Apply inputs to character
        character.SetInputs(ref characterInputs);
    }

    public void SetInputs(float _vertical, float _horizontal, float _mouseX, float _mouseY, float _mouseScroll)
    {
        verticalInput = _vertical;
        horizontalInput = _horizontal;
        mouseXInput = _mouseX * characterCamera.SensitivityX;
        mouseYInput = _mouseY * characterCamera.SensitivityY;
        mouseScrollInput = _mouseScroll / characterCamera.ScrollSensitivity;
    }

    public void DoCameraSwitch()
    {
        if(characterCamera.TargetDistance == 0f)
        {
            characterCamera.TargetDistance = characterCamera.DefaultDistance;
        }
        else
        {
            characterCamera.TargetDistance = 0f;
        }
    }

    private void CheckIfFirstPerson()
    {
        bool lastFrameFirstPersonState = characterCamera.inFirstPerson;

        if(characterCamera.TargetDistance == 0f)
        {
            characterCamera.inFirstPerson = true;
            character.ChangeOrientationMethod(OrientationMethod.TowardsCamera);
            character.MeshRoot.gameObject.SetActive(false);

            if(lastFrameFirstPersonState != characterCamera.inFirstPerson) OnEnterFirstPerson?.Invoke();
        }
        else
        {
            characterCamera.inFirstPerson = false;
            character.ChangeOrientationMethod(OrientationMethod.TowardsMovement);
            character.MeshRoot.gameObject.SetActive(true);
            
            if(lastFrameFirstPersonState != characterCamera.inFirstPerson) OnExitFirstPerson?.Invoke();
        }
    }
}
