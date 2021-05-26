// GENERATED AUTOMATICALLY FROM 'Assets/PlayerControls.inputactions'

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
            ""name"": ""Gameplay"",
            ""id"": ""fd462b8d-e570-4cbb-a5cb-6f6a8c07d615"",
            ""actions"": [
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""db48b010-6e6c-4778-8143-dbb81240b54e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""8971fb32-191a-4cb2-aa31-acdf3ddea034"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Attack"",
                    ""type"": ""Button"",
                    ""id"": ""14eaef13-6244-48b7-8f46-a4d31f07d2ce"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""d5abe83e-8ff9-4d92-90d1-e5dc1f3deabe"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6f09b11f-0ba2-45ea-aed9-0d45d881ef92"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9b93717a-591c-4306-90ca-a232ddd4dd49"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""d060aa59-ff0e-4e7f-bb27-a120b03f59e9"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""ddc87849-a15f-4e3f-b20c-a6dd651c9765"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""a475ce8f-a6d9-474d-82ca-b9e5a117e7a7"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""80adff23-15b7-4cc2-8b9c-0696e44d240a"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""9373c23b-c0e5-4e16-8842-419a680aa42e"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""62c2d52e-818c-40c9-8cde-25874a971afe"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""71acfe91-ab35-4169-a5da-8ef18db44f06"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""93d642cc-e081-49bb-a274-0b5877535b23"",
                    ""path"": ""<Keyboard>/k"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Ui"",
            ""id"": ""3b73d429-4ae4-47c2-93ee-6a3679c16568"",
            ""actions"": [
                {
                    ""name"": ""Select"",
                    ""type"": ""Button"",
                    ""id"": ""66929086-cd38-4416-8980-02608048dbc2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Cancel"",
                    ""type"": ""Button"",
                    ""id"": ""cb0ad4bd-7812-46e5-961b-c4b5983f9034"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""18074844-b5ef-4004-b1ec-4c59d07bb4c4"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""b39c8712-92c2-4623-8520-f9ba9768f32d"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Select"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""273ad052-4c62-44c3-a8f1-0d2ad7cedfd2"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Select"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""54962aa7-bf56-485d-9c17-3b690b937d8c"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d5bd472b-9386-4be7-ba51-1feadf93f258"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Cancel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ce8850da-46a5-4cfb-a25a-b2e67c21e5b1"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": ""StickDeadzone(min=0.4)"",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""1a6c6cd9-3d07-497a-985a-328f149779da"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""b2219742-b314-444b-aab7-294540826465"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""ce40bf2f-0b74-4cfe-aa4f-d4eed88e1b3c"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""fa4ce089-649b-4162-a582-bf0b1c89ef44"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""49a0ac62-4bac-4e99-8d95-09ee00c48d20"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        },
        {
            ""name"": ""LevelEditor"",
            ""id"": ""ddc4a653-cd29-4a56-a8cb-4e9538646645"",
            ""actions"": [
                {
                    ""name"": ""MoveMarker"",
                    ""type"": ""Value"",
                    ""id"": ""983e2c4f-0ab1-4204-8acb-fb7cebe5863f"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Place"",
                    ""type"": ""Button"",
                    ""id"": ""29a1a754-ea46-479c-9553-170de33cfe8c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RaiseMarker"",
                    ""type"": ""Button"",
                    ""id"": ""f9a804ec-3160-4b50-b799-dd020fc34669"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""LowerMarker"",
                    ""type"": ""Button"",
                    ""id"": ""44cd64f6-597d-4723-a436-820891dcabb2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""b95674fa-4b2f-4069-8a5f-cdb0762ffa0e"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""MoveMarker"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""e99e18c5-dc1d-45a6-ad70-abc026ec7a2a"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveMarker"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""eda424ca-ac81-4941-946d-55e70ba39d73"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""MoveMarker"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""acdb5044-9e9d-4bf1-a087-df7d75cb6b59"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""MoveMarker"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""8d98009b-ae43-4b81-8b04-a4a9563e8e7c"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""MoveMarker"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""55dc45c4-64aa-4e43-8b91-1b9927a388b9"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""MoveMarker"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""35bf67ee-cef7-4a7d-9ba2-e572ad4f16ef"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Place"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4abb6359-da9f-4893-b50d-b0671040353d"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Place"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1179740a-3b3d-4d54-9200-b196e99f538b"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""RaiseMarker"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""24f4d26b-7d31-4857-a688-214f2966c951"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""LowerMarker"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Keyboard"",
            ""bindingGroup"": ""Keyboard"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Gameplay
        m_Gameplay = asset.FindActionMap("Gameplay", throwIfNotFound: true);
        m_Gameplay_Jump = m_Gameplay.FindAction("Jump", throwIfNotFound: true);
        m_Gameplay_Move = m_Gameplay.FindAction("Move", throwIfNotFound: true);
        m_Gameplay_Attack = m_Gameplay.FindAction("Attack", throwIfNotFound: true);
        // Ui
        m_Ui = asset.FindActionMap("Ui", throwIfNotFound: true);
        m_Ui_Select = m_Ui.FindAction("Select", throwIfNotFound: true);
        m_Ui_Cancel = m_Ui.FindAction("Cancel", throwIfNotFound: true);
        m_Ui_Move = m_Ui.FindAction("Move", throwIfNotFound: true);
        // LevelEditor
        m_LevelEditor = asset.FindActionMap("LevelEditor", throwIfNotFound: true);
        m_LevelEditor_MoveMarker = m_LevelEditor.FindAction("MoveMarker", throwIfNotFound: true);
        m_LevelEditor_Place = m_LevelEditor.FindAction("Place", throwIfNotFound: true);
        m_LevelEditor_RaiseMarker = m_LevelEditor.FindAction("RaiseMarker", throwIfNotFound: true);
        m_LevelEditor_LowerMarker = m_LevelEditor.FindAction("LowerMarker", throwIfNotFound: true);
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

    // Gameplay
    private readonly InputActionMap m_Gameplay;
    private IGameplayActions m_GameplayActionsCallbackInterface;
    private readonly InputAction m_Gameplay_Jump;
    private readonly InputAction m_Gameplay_Move;
    private readonly InputAction m_Gameplay_Attack;
    public struct GameplayActions
    {
        private @PlayerControls m_Wrapper;
        public GameplayActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Jump => m_Wrapper.m_Gameplay_Jump;
        public InputAction @Move => m_Wrapper.m_Gameplay_Move;
        public InputAction @Attack => m_Wrapper.m_Gameplay_Attack;
        public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
        public void SetCallbacks(IGameplayActions instance)
        {
            if (m_Wrapper.m_GameplayActionsCallbackInterface != null)
            {
                @Jump.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnJump;
                @Move.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                @Attack.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAttack;
                @Attack.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAttack;
                @Attack.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAttack;
            }
            m_Wrapper.m_GameplayActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Attack.started += instance.OnAttack;
                @Attack.performed += instance.OnAttack;
                @Attack.canceled += instance.OnAttack;
            }
        }
    }
    public GameplayActions @Gameplay => new GameplayActions(this);

    // Ui
    private readonly InputActionMap m_Ui;
    private IUiActions m_UiActionsCallbackInterface;
    private readonly InputAction m_Ui_Select;
    private readonly InputAction m_Ui_Cancel;
    private readonly InputAction m_Ui_Move;
    public struct UiActions
    {
        private @PlayerControls m_Wrapper;
        public UiActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Select => m_Wrapper.m_Ui_Select;
        public InputAction @Cancel => m_Wrapper.m_Ui_Cancel;
        public InputAction @Move => m_Wrapper.m_Ui_Move;
        public InputActionMap Get() { return m_Wrapper.m_Ui; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(UiActions set) { return set.Get(); }
        public void SetCallbacks(IUiActions instance)
        {
            if (m_Wrapper.m_UiActionsCallbackInterface != null)
            {
                @Select.started -= m_Wrapper.m_UiActionsCallbackInterface.OnSelect;
                @Select.performed -= m_Wrapper.m_UiActionsCallbackInterface.OnSelect;
                @Select.canceled -= m_Wrapper.m_UiActionsCallbackInterface.OnSelect;
                @Cancel.started -= m_Wrapper.m_UiActionsCallbackInterface.OnCancel;
                @Cancel.performed -= m_Wrapper.m_UiActionsCallbackInterface.OnCancel;
                @Cancel.canceled -= m_Wrapper.m_UiActionsCallbackInterface.OnCancel;
                @Move.started -= m_Wrapper.m_UiActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_UiActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_UiActionsCallbackInterface.OnMove;
            }
            m_Wrapper.m_UiActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Select.started += instance.OnSelect;
                @Select.performed += instance.OnSelect;
                @Select.canceled += instance.OnSelect;
                @Cancel.started += instance.OnCancel;
                @Cancel.performed += instance.OnCancel;
                @Cancel.canceled += instance.OnCancel;
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
            }
        }
    }
    public UiActions @Ui => new UiActions(this);

    // LevelEditor
    private readonly InputActionMap m_LevelEditor;
    private ILevelEditorActions m_LevelEditorActionsCallbackInterface;
    private readonly InputAction m_LevelEditor_MoveMarker;
    private readonly InputAction m_LevelEditor_Place;
    private readonly InputAction m_LevelEditor_RaiseMarker;
    private readonly InputAction m_LevelEditor_LowerMarker;
    public struct LevelEditorActions
    {
        private @PlayerControls m_Wrapper;
        public LevelEditorActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @MoveMarker => m_Wrapper.m_LevelEditor_MoveMarker;
        public InputAction @Place => m_Wrapper.m_LevelEditor_Place;
        public InputAction @RaiseMarker => m_Wrapper.m_LevelEditor_RaiseMarker;
        public InputAction @LowerMarker => m_Wrapper.m_LevelEditor_LowerMarker;
        public InputActionMap Get() { return m_Wrapper.m_LevelEditor; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(LevelEditorActions set) { return set.Get(); }
        public void SetCallbacks(ILevelEditorActions instance)
        {
            if (m_Wrapper.m_LevelEditorActionsCallbackInterface != null)
            {
                @MoveMarker.started -= m_Wrapper.m_LevelEditorActionsCallbackInterface.OnMoveMarker;
                @MoveMarker.performed -= m_Wrapper.m_LevelEditorActionsCallbackInterface.OnMoveMarker;
                @MoveMarker.canceled -= m_Wrapper.m_LevelEditorActionsCallbackInterface.OnMoveMarker;
                @Place.started -= m_Wrapper.m_LevelEditorActionsCallbackInterface.OnPlace;
                @Place.performed -= m_Wrapper.m_LevelEditorActionsCallbackInterface.OnPlace;
                @Place.canceled -= m_Wrapper.m_LevelEditorActionsCallbackInterface.OnPlace;
                @RaiseMarker.started -= m_Wrapper.m_LevelEditorActionsCallbackInterface.OnRaiseMarker;
                @RaiseMarker.performed -= m_Wrapper.m_LevelEditorActionsCallbackInterface.OnRaiseMarker;
                @RaiseMarker.canceled -= m_Wrapper.m_LevelEditorActionsCallbackInterface.OnRaiseMarker;
                @LowerMarker.started -= m_Wrapper.m_LevelEditorActionsCallbackInterface.OnLowerMarker;
                @LowerMarker.performed -= m_Wrapper.m_LevelEditorActionsCallbackInterface.OnLowerMarker;
                @LowerMarker.canceled -= m_Wrapper.m_LevelEditorActionsCallbackInterface.OnLowerMarker;
            }
            m_Wrapper.m_LevelEditorActionsCallbackInterface = instance;
            if (instance != null)
            {
                @MoveMarker.started += instance.OnMoveMarker;
                @MoveMarker.performed += instance.OnMoveMarker;
                @MoveMarker.canceled += instance.OnMoveMarker;
                @Place.started += instance.OnPlace;
                @Place.performed += instance.OnPlace;
                @Place.canceled += instance.OnPlace;
                @RaiseMarker.started += instance.OnRaiseMarker;
                @RaiseMarker.performed += instance.OnRaiseMarker;
                @RaiseMarker.canceled += instance.OnRaiseMarker;
                @LowerMarker.started += instance.OnLowerMarker;
                @LowerMarker.performed += instance.OnLowerMarker;
                @LowerMarker.canceled += instance.OnLowerMarker;
            }
        }
    }
    public LevelEditorActions @LevelEditor => new LevelEditorActions(this);
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get
        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
    private int m_KeyboardSchemeIndex = -1;
    public InputControlScheme KeyboardScheme
    {
        get
        {
            if (m_KeyboardSchemeIndex == -1) m_KeyboardSchemeIndex = asset.FindControlSchemeIndex("Keyboard");
            return asset.controlSchemes[m_KeyboardSchemeIndex];
        }
    }
    public interface IGameplayActions
    {
        void OnJump(InputAction.CallbackContext context);
        void OnMove(InputAction.CallbackContext context);
        void OnAttack(InputAction.CallbackContext context);
    }
    public interface IUiActions
    {
        void OnSelect(InputAction.CallbackContext context);
        void OnCancel(InputAction.CallbackContext context);
        void OnMove(InputAction.CallbackContext context);
    }
    public interface ILevelEditorActions
    {
        void OnMoveMarker(InputAction.CallbackContext context);
        void OnPlace(InputAction.CallbackContext context);
        void OnRaiseMarker(InputAction.CallbackContext context);
        void OnLowerMarker(InputAction.CallbackContext context);
    }
}
