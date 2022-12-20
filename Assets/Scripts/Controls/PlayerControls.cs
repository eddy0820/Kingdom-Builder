// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/Controls/PlayerControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""GroundMovement"",
            ""id"": ""13f97ea6-da82-46eb-8516-1ab019db8717"",
            ""actions"": [
                {
                    ""name"": ""MouseX"",
                    ""type"": ""PassThrough"",
                    ""id"": ""ca06ca1b-f092-4ce8-a8a8-6894be6699ed"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MouseY"",
                    ""type"": ""PassThrough"",
                    ""id"": ""cf0698c1-d9f9-406e-8e47-05a83c672d95"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""HorizontalMovement"",
                    ""type"": ""PassThrough"",
                    ""id"": ""096c913d-7a22-4888-890c-94c8d2ca2f68"",
                    ""expectedControlType"": ""Vector3"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MouseScroll"",
                    ""type"": ""PassThrough"",
                    ""id"": ""6274699b-8888-43cc-b65d-30f95033ba90"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""CameraSwitch"",
                    ""type"": ""Button"",
                    ""id"": ""4b1c6eee-6e74-4f5e-8ecf-752c6b06728d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""4a80ad8f-3859-4230-87d2-1fd6544e4228"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Crouch"",
                    ""type"": ""Button"",
                    ""id"": ""cc8a7675-c6fa-438d-9110-8df20801ddd2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Walk"",
                    ""type"": ""Button"",
                    ""id"": ""d6b30bc4-39b4-4edd-88f4-74b9cc85cf5d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Sprint"",
                    ""type"": ""Button"",
                    ""id"": ""dd9ff5b2-d21a-4092-944b-7a63cae316e2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""95fc3139-ce9a-4992-8779-e9cd02bc9f47"",
                    ""path"": ""<Mouse>/delta/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse + Keyboard"",
                    ""action"": ""MouseX"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3d5de099-3cfd-424f-911c-3094510f72c6"",
                    ""path"": ""<Mouse>/delta/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse + Keyboard"",
                    ""action"": ""MouseY"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""WASD"",
                    ""id"": ""bb73270e-c869-4794-b6dc-c17a13064bdf"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HorizontalMovement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""37806c69-0b26-4310-b417-643edc7d64ef"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse + Keyboard"",
                    ""action"": ""HorizontalMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""ca63bb5c-7c84-4d6e-a279-ca876d4b2433"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse + Keyboard"",
                    ""action"": ""HorizontalMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""5fb7eaa4-3e7a-4b36-b673-24d205a665cc"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse + Keyboard"",
                    ""action"": ""HorizontalMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""acd97f68-8e53-4247-83d3-d916d7484111"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse + Keyboard"",
                    ""action"": ""HorizontalMovement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""433d0dc9-7334-4ae2-8ef6-c3093556b62d"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse + Keyboard"",
                    ""action"": ""MouseScroll"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""46cf4e9d-130c-4790-8e94-69dbed289113"",
                    ""path"": ""<Keyboard>/f"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse + Keyboard"",
                    ""action"": ""CameraSwitch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3a232a77-d7b2-4346-8897-58dbec863f79"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""dedef0e3-ddb4-4912-8480-74fc86572db1"",
                    ""path"": ""<Keyboard>/leftCtrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Crouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1b613de7-eeac-44e2-98a5-3554138ad582"",
                    ""path"": ""<Keyboard>/capsLock"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse + Keyboard"",
                    ""action"": ""Walk"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""07576527-c763-47d7-93d6-53e9efe7a3fd"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse + Keyboard"",
                    ""action"": ""Sprint"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""PlayerMechanics"",
            ""id"": ""9a7b6c6e-a3df-44e2-acbc-b0359e730ade"",
            ""actions"": [
                {
                    ""name"": ""ToggleBuildMode"",
                    ""type"": ""Button"",
                    ""id"": ""96d645cc-b900-4b00-bdb5-8a550d7b83cf"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Build"",
                    ""type"": ""Button"",
                    ""id"": ""e3e365cf-f31f-45bb-ab8c-e611b3c9695a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Demolish"",
                    ""type"": ""Button"",
                    ""id"": ""a53dfc1b-5666-4bcb-b6e3-ae22d472d0f0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Rotate"",
                    ""type"": ""PassThrough"",
                    ""id"": ""26a930d9-d14f-459f-866c-363605b2cf32"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""238c131c-d847-49e1-9976-816e9bde21e7"",
                    ""path"": ""<Keyboard>/b"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse + Keyboard"",
                    ""action"": ""ToggleBuildMode"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a0baf16c-b7e5-4935-a978-df2d3a05aa54"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse + Keyboard"",
                    ""action"": ""Build"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3e964024-1f68-4d47-aabe-620c747f4a6c"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse + Keyboard"",
                    ""action"": ""Demolish"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3b7217eb-d294-42df-aa80-3269680b1fb7"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Mouse + Keyboard"",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Mouse + Keyboard"",
            ""bindingGroup"": ""Mouse + Keyboard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // GroundMovement
        m_GroundMovement = asset.FindActionMap("GroundMovement", throwIfNotFound: true);
        m_GroundMovement_MouseX = m_GroundMovement.FindAction("MouseX", throwIfNotFound: true);
        m_GroundMovement_MouseY = m_GroundMovement.FindAction("MouseY", throwIfNotFound: true);
        m_GroundMovement_HorizontalMovement = m_GroundMovement.FindAction("HorizontalMovement", throwIfNotFound: true);
        m_GroundMovement_MouseScroll = m_GroundMovement.FindAction("MouseScroll", throwIfNotFound: true);
        m_GroundMovement_CameraSwitch = m_GroundMovement.FindAction("CameraSwitch", throwIfNotFound: true);
        m_GroundMovement_Jump = m_GroundMovement.FindAction("Jump", throwIfNotFound: true);
        m_GroundMovement_Crouch = m_GroundMovement.FindAction("Crouch", throwIfNotFound: true);
        m_GroundMovement_Walk = m_GroundMovement.FindAction("Walk", throwIfNotFound: true);
        m_GroundMovement_Sprint = m_GroundMovement.FindAction("Sprint", throwIfNotFound: true);
        // PlayerMechanics
        m_PlayerMechanics = asset.FindActionMap("PlayerMechanics", throwIfNotFound: true);
        m_PlayerMechanics_ToggleBuildMode = m_PlayerMechanics.FindAction("ToggleBuildMode", throwIfNotFound: true);
        m_PlayerMechanics_Build = m_PlayerMechanics.FindAction("Build", throwIfNotFound: true);
        m_PlayerMechanics_Demolish = m_PlayerMechanics.FindAction("Demolish", throwIfNotFound: true);
        m_PlayerMechanics_Rotate = m_PlayerMechanics.FindAction("Rotate", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // GroundMovement
    private readonly InputActionMap m_GroundMovement;
    private IGroundMovementActions m_GroundMovementActionsCallbackInterface;
    private readonly InputAction m_GroundMovement_MouseX;
    private readonly InputAction m_GroundMovement_MouseY;
    private readonly InputAction m_GroundMovement_HorizontalMovement;
    private readonly InputAction m_GroundMovement_MouseScroll;
    private readonly InputAction m_GroundMovement_CameraSwitch;
    private readonly InputAction m_GroundMovement_Jump;
    private readonly InputAction m_GroundMovement_Crouch;
    private readonly InputAction m_GroundMovement_Walk;
    private readonly InputAction m_GroundMovement_Sprint;
    public struct GroundMovementActions
    {
        private @PlayerControls m_Wrapper;
        public GroundMovementActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @MouseX => m_Wrapper.m_GroundMovement_MouseX;
        public InputAction @MouseY => m_Wrapper.m_GroundMovement_MouseY;
        public InputAction @HorizontalMovement => m_Wrapper.m_GroundMovement_HorizontalMovement;
        public InputAction @MouseScroll => m_Wrapper.m_GroundMovement_MouseScroll;
        public InputAction @CameraSwitch => m_Wrapper.m_GroundMovement_CameraSwitch;
        public InputAction @Jump => m_Wrapper.m_GroundMovement_Jump;
        public InputAction @Crouch => m_Wrapper.m_GroundMovement_Crouch;
        public InputAction @Walk => m_Wrapper.m_GroundMovement_Walk;
        public InputAction @Sprint => m_Wrapper.m_GroundMovement_Sprint;
        public InputActionMap Get() { return m_Wrapper.m_GroundMovement; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GroundMovementActions set) { return set.Get(); }
        public void SetCallbacks(IGroundMovementActions instance)
        {
            if (m_Wrapper.m_GroundMovementActionsCallbackInterface != null)
            {
                @MouseX.started -= m_Wrapper.m_GroundMovementActionsCallbackInterface.OnMouseX;
                @MouseX.performed -= m_Wrapper.m_GroundMovementActionsCallbackInterface.OnMouseX;
                @MouseX.canceled -= m_Wrapper.m_GroundMovementActionsCallbackInterface.OnMouseX;
                @MouseY.started -= m_Wrapper.m_GroundMovementActionsCallbackInterface.OnMouseY;
                @MouseY.performed -= m_Wrapper.m_GroundMovementActionsCallbackInterface.OnMouseY;
                @MouseY.canceled -= m_Wrapper.m_GroundMovementActionsCallbackInterface.OnMouseY;
                @HorizontalMovement.started -= m_Wrapper.m_GroundMovementActionsCallbackInterface.OnHorizontalMovement;
                @HorizontalMovement.performed -= m_Wrapper.m_GroundMovementActionsCallbackInterface.OnHorizontalMovement;
                @HorizontalMovement.canceled -= m_Wrapper.m_GroundMovementActionsCallbackInterface.OnHorizontalMovement;
                @MouseScroll.started -= m_Wrapper.m_GroundMovementActionsCallbackInterface.OnMouseScroll;
                @MouseScroll.performed -= m_Wrapper.m_GroundMovementActionsCallbackInterface.OnMouseScroll;
                @MouseScroll.canceled -= m_Wrapper.m_GroundMovementActionsCallbackInterface.OnMouseScroll;
                @CameraSwitch.started -= m_Wrapper.m_GroundMovementActionsCallbackInterface.OnCameraSwitch;
                @CameraSwitch.performed -= m_Wrapper.m_GroundMovementActionsCallbackInterface.OnCameraSwitch;
                @CameraSwitch.canceled -= m_Wrapper.m_GroundMovementActionsCallbackInterface.OnCameraSwitch;
                @Jump.started -= m_Wrapper.m_GroundMovementActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_GroundMovementActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_GroundMovementActionsCallbackInterface.OnJump;
                @Crouch.started -= m_Wrapper.m_GroundMovementActionsCallbackInterface.OnCrouch;
                @Crouch.performed -= m_Wrapper.m_GroundMovementActionsCallbackInterface.OnCrouch;
                @Crouch.canceled -= m_Wrapper.m_GroundMovementActionsCallbackInterface.OnCrouch;
                @Walk.started -= m_Wrapper.m_GroundMovementActionsCallbackInterface.OnWalk;
                @Walk.performed -= m_Wrapper.m_GroundMovementActionsCallbackInterface.OnWalk;
                @Walk.canceled -= m_Wrapper.m_GroundMovementActionsCallbackInterface.OnWalk;
                @Sprint.started -= m_Wrapper.m_GroundMovementActionsCallbackInterface.OnSprint;
                @Sprint.performed -= m_Wrapper.m_GroundMovementActionsCallbackInterface.OnSprint;
                @Sprint.canceled -= m_Wrapper.m_GroundMovementActionsCallbackInterface.OnSprint;
            }
            m_Wrapper.m_GroundMovementActionsCallbackInterface = instance;
            if (instance != null)
            {
                @MouseX.started += instance.OnMouseX;
                @MouseX.performed += instance.OnMouseX;
                @MouseX.canceled += instance.OnMouseX;
                @MouseY.started += instance.OnMouseY;
                @MouseY.performed += instance.OnMouseY;
                @MouseY.canceled += instance.OnMouseY;
                @HorizontalMovement.started += instance.OnHorizontalMovement;
                @HorizontalMovement.performed += instance.OnHorizontalMovement;
                @HorizontalMovement.canceled += instance.OnHorizontalMovement;
                @MouseScroll.started += instance.OnMouseScroll;
                @MouseScroll.performed += instance.OnMouseScroll;
                @MouseScroll.canceled += instance.OnMouseScroll;
                @CameraSwitch.started += instance.OnCameraSwitch;
                @CameraSwitch.performed += instance.OnCameraSwitch;
                @CameraSwitch.canceled += instance.OnCameraSwitch;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Crouch.started += instance.OnCrouch;
                @Crouch.performed += instance.OnCrouch;
                @Crouch.canceled += instance.OnCrouch;
                @Walk.started += instance.OnWalk;
                @Walk.performed += instance.OnWalk;
                @Walk.canceled += instance.OnWalk;
                @Sprint.started += instance.OnSprint;
                @Sprint.performed += instance.OnSprint;
                @Sprint.canceled += instance.OnSprint;
            }
        }
    }
    public GroundMovementActions @GroundMovement => new GroundMovementActions(this);

    // PlayerMechanics
    private readonly InputActionMap m_PlayerMechanics;
    private IPlayerMechanicsActions m_PlayerMechanicsActionsCallbackInterface;
    private readonly InputAction m_PlayerMechanics_ToggleBuildMode;
    private readonly InputAction m_PlayerMechanics_Build;
    private readonly InputAction m_PlayerMechanics_Demolish;
    private readonly InputAction m_PlayerMechanics_Rotate;
    public struct PlayerMechanicsActions
    {
        private @PlayerControls m_Wrapper;
        public PlayerMechanicsActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @ToggleBuildMode => m_Wrapper.m_PlayerMechanics_ToggleBuildMode;
        public InputAction @Build => m_Wrapper.m_PlayerMechanics_Build;
        public InputAction @Demolish => m_Wrapper.m_PlayerMechanics_Demolish;
        public InputAction @Rotate => m_Wrapper.m_PlayerMechanics_Rotate;
        public InputActionMap Get() { return m_Wrapper.m_PlayerMechanics; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerMechanicsActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerMechanicsActions instance)
        {
            if (m_Wrapper.m_PlayerMechanicsActionsCallbackInterface != null)
            {
                @ToggleBuildMode.started -= m_Wrapper.m_PlayerMechanicsActionsCallbackInterface.OnToggleBuildMode;
                @ToggleBuildMode.performed -= m_Wrapper.m_PlayerMechanicsActionsCallbackInterface.OnToggleBuildMode;
                @ToggleBuildMode.canceled -= m_Wrapper.m_PlayerMechanicsActionsCallbackInterface.OnToggleBuildMode;
                @Build.started -= m_Wrapper.m_PlayerMechanicsActionsCallbackInterface.OnBuild;
                @Build.performed -= m_Wrapper.m_PlayerMechanicsActionsCallbackInterface.OnBuild;
                @Build.canceled -= m_Wrapper.m_PlayerMechanicsActionsCallbackInterface.OnBuild;
                @Demolish.started -= m_Wrapper.m_PlayerMechanicsActionsCallbackInterface.OnDemolish;
                @Demolish.performed -= m_Wrapper.m_PlayerMechanicsActionsCallbackInterface.OnDemolish;
                @Demolish.canceled -= m_Wrapper.m_PlayerMechanicsActionsCallbackInterface.OnDemolish;
                @Rotate.started -= m_Wrapper.m_PlayerMechanicsActionsCallbackInterface.OnRotate;
                @Rotate.performed -= m_Wrapper.m_PlayerMechanicsActionsCallbackInterface.OnRotate;
                @Rotate.canceled -= m_Wrapper.m_PlayerMechanicsActionsCallbackInterface.OnRotate;
            }
            m_Wrapper.m_PlayerMechanicsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @ToggleBuildMode.started += instance.OnToggleBuildMode;
                @ToggleBuildMode.performed += instance.OnToggleBuildMode;
                @ToggleBuildMode.canceled += instance.OnToggleBuildMode;
                @Build.started += instance.OnBuild;
                @Build.performed += instance.OnBuild;
                @Build.canceled += instance.OnBuild;
                @Demolish.started += instance.OnDemolish;
                @Demolish.performed += instance.OnDemolish;
                @Demolish.canceled += instance.OnDemolish;
                @Rotate.started += instance.OnRotate;
                @Rotate.performed += instance.OnRotate;
                @Rotate.canceled += instance.OnRotate;
            }
        }
    }
    public PlayerMechanicsActions @PlayerMechanics => new PlayerMechanicsActions(this);
    private int m_MouseKeyboardSchemeIndex = -1;
    public InputControlScheme MouseKeyboardScheme
    {
        get
        {
            if (m_MouseKeyboardSchemeIndex == -1) m_MouseKeyboardSchemeIndex = asset.FindControlSchemeIndex("Mouse + Keyboard");
            return asset.controlSchemes[m_MouseKeyboardSchemeIndex];
        }
    }
    public interface IGroundMovementActions
    {
        void OnMouseX(InputAction.CallbackContext context);
        void OnMouseY(InputAction.CallbackContext context);
        void OnHorizontalMovement(InputAction.CallbackContext context);
        void OnMouseScroll(InputAction.CallbackContext context);
        void OnCameraSwitch(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnCrouch(InputAction.CallbackContext context);
        void OnWalk(InputAction.CallbackContext context);
        void OnSprint(InputAction.CallbackContext context);
    }
    public interface IPlayerMechanicsActions
    {
        void OnToggleBuildMode(InputAction.CallbackContext context);
        void OnBuild(InputAction.CallbackContext context);
        void OnDemolish(InputAction.CallbackContext context);
        void OnRotate(InputAction.CallbackContext context);
    }
}
