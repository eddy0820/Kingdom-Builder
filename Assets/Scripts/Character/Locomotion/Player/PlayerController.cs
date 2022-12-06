using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;

public class PlayerController : MonoBehaviour
{
    public PlayerCharacterController Character;
    public PlayerCamera CharacterCamera;

    private float verticalInput;
    private float horizontalInput;
    private float mouseXInput;
    private float mouseYInput;
    private float mouseScrollInput;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        // Tell camera to follow transform
        CharacterCamera.SetFollowTransform(Character.CameraFollowPoint);

        // Ignore the character's collider(s) for camera obstruction checks
        CharacterCamera.IgnoredColliders.Clear();
        CharacterCamera.IgnoredColliders.AddRange(Character.GetComponentsInChildren<Collider>());
    }

    private void Update()
    {
        //if(Input.GetMouseButtonDown(0)) Cursor.lockState = CursorLockMode.Locked;

        HandleCharacterInput();

        CheckIfFirstPerson();
    }

    private void LateUpdate()
    {
        // Handle rotating the camera along with physics movers
        if(CharacterCamera.RotateWithPhysicsMover && Character.Motor.AttachedRigidbody != null)
        {
            CharacterCamera.PlanarDirection = Character.Motor.AttachedRigidbody.GetComponent<PhysicsMover>().RotationDeltaFromInterpolation * CharacterCamera.PlanarDirection;
            CharacterCamera.PlanarDirection = Vector3.ProjectOnPlane(CharacterCamera.PlanarDirection, Character.Motor.CharacterUp).normalized;
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
        if(Cursor.lockState != CursorLockMode.Locked)
        {
            lookInputVector = Vector3.zero;
        }

        // Input for zooming the camera (disabled in WebGL because it can cause problems)
        float scrollInput = -mouseScrollInput;

        #if UNITY_WEBGL
            scrollInput = 0f;
        #endif

        // Apply inputs to the camera
        CharacterCamera.UpdateWithInput(Time.deltaTime, scrollInput, lookInputVector);
    }

    private void HandleCharacterInput()
    {
        PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();

        // Build the CharacterInputs struct
        characterInputs.MoveAxisForward = verticalInput;
        characterInputs.MoveAxisRight = horizontalInput;
        characterInputs.CameraRotation = CharacterCamera.Transform.rotation;

        // Apply inputs to character
        Character.SetInputs(ref characterInputs);
    }

    public void SetInputs(float _vertical, float _horizontal, float _mouseX, float _mouseY, float _mouseScroll)
    {
        verticalInput = _vertical;
        horizontalInput = _horizontal;
        mouseXInput = _mouseX * CharacterCamera.SensitivityX;
        mouseYInput = _mouseY * CharacterCamera.SensitivityY;
        mouseScrollInput = _mouseScroll / CharacterCamera.ScrollSensitivity;
    }

    public void DoCameraSwitch()
    {
        if(CharacterCamera.TargetDistance == 0f)
        {
            CharacterCamera.TargetDistance = CharacterCamera.DefaultDistance;
        }
        else
        {
            CharacterCamera.TargetDistance = 0f;
        }
    }

    private void CheckIfFirstPerson()
    {
        if(CharacterCamera.TargetDistance == 0f)
        {
            CharacterCamera.inFirstPerson = true;
            Character.OrientationMethod = OrientationMethod.TowardsCamera;
            Character.MeshRoot.gameObject.SetActive(false);
        }
        else
        {
            CharacterCamera.inFirstPerson = false;
            Character.OrientationMethod = OrientationMethod.TowardsMovement;
            Character.MeshRoot.gameObject.SetActive(true);
        }
    }
}
