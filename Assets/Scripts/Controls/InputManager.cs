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

    PlayerController playerController;

    private void Awake()
    {
        controls = new PlayerControls();
        groundMovement = controls.GroundMovement;

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
