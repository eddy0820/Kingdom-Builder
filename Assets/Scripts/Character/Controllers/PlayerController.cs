using UnityEngine;
using KinematicCharacterController;
using UnityEngine.InputSystem;

// LINE 64, 93 LOCK ON

public class PlayerController : MonoBehaviour
{
    [SerializeField] PlayerCharacterController movementController;
    [SerializeField] PlayerCamera cameraController;
    [Space(10)]
    [SerializeField] PlayerModelController modelController;
    [Space(10)]
    [SerializeField] PlayerCharacterStateMachine stateMachine;

    private float verticalInput;
    private float horizontalInput;
    private float mouseXInput;
    private float mouseYInput;
    private float mouseScrollInput;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        cameraController.OnToggleFirstPerson += OnToggleFirstPerson;
    }
   
    private void Update()
    {
        HandleMovementInput();
    }

    private void LateUpdate()
    {
        HandleCameraInput();
    }

#region Character Movement Methods

    private void HandleMovementInput()
    {
        Quaternion cameraRotation = cameraController.DefaultCameraTransform.rotation;

        // if(lockedOn)
        //     cameraRotation = characterCamera.LockOnCameraTransform.rotation;

        // Apply inputs to character
        movementController.UpdateInputs(horizontalInput, verticalInput, cameraRotation);
    }

#endregion

#region Camera Methods

    private void HandleCameraInput()
    {
        // For rotating the camera along with physics movers
        Quaternion? rotationDeltaFromInterpolation = null;
        Vector3? characterUp = null;

        if(movementController.Motor.AttachedRigidbody != null)
        {
            rotationDeltaFromInterpolation = movementController.Motor.AttachedRigidbody.GetComponent<PhysicsMover>().RotationDeltaFromInterpolation;
            characterUp = movementController.Motor.CharacterUp;
        }

        // Create the look input vector for the camera
        float mouseLookAxisUp = mouseYInput;
        float mouseLookAxisRight = mouseXInput;
        Vector3 lookInputVector = new(mouseLookAxisRight, mouseLookAxisUp, 0f);

        // Prevent moving the camera while the cursor isn't locked
        if(Cursor.lockState != CursorLockMode.Locked)// || lockedOn)
        {
            lookInputVector = Vector3.zero;
        }

        // Input for zooming the camera (disabled in WebGL because it can cause problems)
        float scrollInput = -mouseScrollInput;

        #if UNITY_WEBGL
            scrollInput = 0f;
        #endif

        // Apply inputs to the camera
        cameraController.UpdateWithInput(rotationDeltaFromInterpolation, characterUp, Time.deltaTime, scrollInput, lookInputVector);
    }

#endregion

#region First Person Switch Callbacks

    private void OnToggleFirstPerson(bool inFirstPerson)
    {
        movementController.ChangeOrientationMethod(inFirstPerson ? OrientationMethod.TowardsCamera : OrientationMethod.TowardsMovement);
        modelController.Toggle3rdPersonModel(!inFirstPerson);
    }

#endregion

#region Input Listeners    

    public void SetHorizontalMovementInput(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        horizontalInput = input.x;
        verticalInput = input.y;
    }
    public void SetCameraX(InputAction.CallbackContext context) => mouseXInput = context.ReadValue<float>();
    public void SetCameraY(InputAction.CallbackContext context) => mouseYInput = context.ReadValue<float>();
    public void SetMouseScroll(InputAction.CallbackContext context) => mouseScrollInput = context.ReadValue<float>();
    public void OnFlipCameraPerspective() => cameraController.FlipCameraPerspective();
    public void OnFlipCameraAlignment() => cameraController.FlipCameraAlignment();

#endregion
}
