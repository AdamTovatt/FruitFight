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
                },
                {
                    ""name"": ""RotateCameraRight"",
                    ""type"": ""Button"",
                    ""id"": ""963e6dea-cf91-439d-9bc5-70f1046419bf"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RotateCameraLeft"",
                    ""type"": ""Button"",
                    ""id"": ""0505abbe-c107-418b-beef-386bec8877cc"",
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
                },
                {
                    ""name"": """",
                    ""id"": ""bd24fc55-580b-4dd8-ae3d-f1c984e94dea"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""RotateCameraRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e2700dc5-1327-496c-9938-42f5d1fa30d5"",
                    ""path"": ""<Keyboard>/l"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""RotateCameraRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c1c4f773-8d43-4aae-905b-d5767b89c6e3"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""RotateCameraLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""930e5a18-fc68-472d-a520-1c090f460efe"",
                    ""path"": ""<Keyboard>/j"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""RotateCameraLeft"",
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
                },
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""8b6bf0cc-2541-4301-a32b-7f029317dc2b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MoveBlockSelection"",
                    ""type"": ""Value"",
                    ""id"": ""1a52df1a-a053-47a1-8656-30f19602498d"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Remove"",
                    ""type"": ""Button"",
                    ""id"": ""edc1528b-57f6-44c8-aaf1-9a84951c4492"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""NextPage"",
                    ""type"": ""Button"",
                    ""id"": ""950f6777-e0bd-4882-9227-8aedeea3049b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""PreviousPage"",
                    ""type"": ""Button"",
                    ""id"": ""243b6969-07e4-463f-a912-85e673bc3fc5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Rotate"",
                    ""type"": ""Value"",
                    ""id"": ""c6886104-c59a-4d25-95a1-c60b4b8e005b"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ToggleObjectRotation"",
                    ""type"": ""Button"",
                    ""id"": ""652ce3ee-ec59-4988-b5e1-46fa4a747886"",
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
                    ""id"": ""cf14fa6c-bba7-4a48-886f-b9200aebbc60"",
                    ""path"": ""<Gamepad>/rightShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
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
                },
                {
                    ""name"": """",
                    ""id"": ""5db256f0-ed7d-49ab-8c68-c9065a9d7582"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""LowerMarker"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""69e914b3-116a-4b2b-b699-dcc1fd2ce57d"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""546070a1-71db-492b-a745-a60c8115dfb7"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""b20dfead-76e9-4364-a0e2-c054517aa1f5"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveBlockSelection"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""8c111860-fcf1-4eaf-8908-a8c863737ec2"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""MoveBlockSelection"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""d9abd5ff-268f-410f-9ed4-3a899bd85cb4"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""MoveBlockSelection"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""a92cc248-7a47-44b6-8742-90b269be30b4"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""MoveBlockSelection"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""2adbcacc-c4c1-428f-8af4-df40c7736606"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""MoveBlockSelection"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""43ce1b4a-3605-4d13-9b0d-aee97748dce0"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveBlockSelection"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""b095baef-7337-4ac4-b114-db502da12ac5"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""MoveBlockSelection"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""c90155fc-41ba-4713-93f0-b233f48f5524"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""MoveBlockSelection"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""6a3ce4a7-67c2-4867-a0d2-90d5c97e7df6"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""MoveBlockSelection"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""6b630ff6-4536-4f0c-bdd9-b0c11fda4dfc"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""MoveBlockSelection"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""5e5fc9ab-0cd6-40e3-abf2-606fe9ec2ec9"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Remove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b90a5893-2d1e-40c7-8010-569f1a64d7e1"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Remove"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3ad7e55b-2e0e-4171-96d5-16ba9264a9f4"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""NextPage"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d5030773-07fc-40e6-93ff-df141b4701ea"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""NextPage"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6ca38b7e-127c-463c-8dcd-a601806e3740"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""PreviousPage"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""179683e4-5ce1-4ba2-8a09-f350059c3ace"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""PreviousPage"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c46e1c09-3eae-48b6-97ae-76b43800d4a1"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""1d7c88dc-4f24-48dd-ad24-2ed15c36c34b"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotate"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""17e642f5-099c-4c7f-a396-1097f2929702"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""7da1bde6-b621-4b98-9bba-5cb10f8563b3"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""efd15635-ae64-4a35-9550-c90206753299"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""58c9558c-c4b4-4702-a77c-9ddde43a84c3"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""Rotate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""a58b3b35-7c6d-4c18-a56f-14078143734e"",
                    ""path"": ""<Gamepad>/buttonNorth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""ToggleObjectRotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""40890860-42da-488a-83da-e91d41fd278b"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard"",
                    ""action"": ""ToggleObjectRotation"",
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
        m_Gameplay_RotateCameraRight = m_Gameplay.FindAction("RotateCameraRight", throwIfNotFound: true);
        m_Gameplay_RotateCameraLeft = m_Gameplay.FindAction("RotateCameraLeft", throwIfNotFound: true);
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
        m_LevelEditor_Pause = m_LevelEditor.FindAction("Pause", throwIfNotFound: true);
        m_LevelEditor_MoveBlockSelection = m_LevelEditor.FindAction("MoveBlockSelection", throwIfNotFound: true);
        m_LevelEditor_Remove = m_LevelEditor.FindAction("Remove", throwIfNotFound: true);
        m_LevelEditor_NextPage = m_LevelEditor.FindAction("NextPage", throwIfNotFound: true);
        m_LevelEditor_PreviousPage = m_LevelEditor.FindAction("PreviousPage", throwIfNotFound: true);
        m_LevelEditor_Rotate = m_LevelEditor.FindAction("Rotate", throwIfNotFound: true);
        m_LevelEditor_ToggleObjectRotation = m_LevelEditor.FindAction("ToggleObjectRotation", throwIfNotFound: true);
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
    private readonly InputAction m_Gameplay_RotateCameraRight;
    private readonly InputAction m_Gameplay_RotateCameraLeft;
    public struct GameplayActions
    {
        private @PlayerControls m_Wrapper;
        public GameplayActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Jump => m_Wrapper.m_Gameplay_Jump;
        public InputAction @Move => m_Wrapper.m_Gameplay_Move;
        public InputAction @Attack => m_Wrapper.m_Gameplay_Attack;
        public InputAction @RotateCameraRight => m_Wrapper.m_Gameplay_RotateCameraRight;
        public InputAction @RotateCameraLeft => m_Wrapper.m_Gameplay_RotateCameraLeft;
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
                @RotateCameraRight.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRotateCameraRight;
                @RotateCameraRight.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRotateCameraRight;
                @RotateCameraRight.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRotateCameraRight;
                @RotateCameraLeft.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRotateCameraLeft;
                @RotateCameraLeft.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRotateCameraLeft;
                @RotateCameraLeft.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRotateCameraLeft;
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
                @RotateCameraRight.started += instance.OnRotateCameraRight;
                @RotateCameraRight.performed += instance.OnRotateCameraRight;
                @RotateCameraRight.canceled += instance.OnRotateCameraRight;
                @RotateCameraLeft.started += instance.OnRotateCameraLeft;
                @RotateCameraLeft.performed += instance.OnRotateCameraLeft;
                @RotateCameraLeft.canceled += instance.OnRotateCameraLeft;
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
    private readonly InputAction m_LevelEditor_Pause;
    private readonly InputAction m_LevelEditor_MoveBlockSelection;
    private readonly InputAction m_LevelEditor_Remove;
    private readonly InputAction m_LevelEditor_NextPage;
    private readonly InputAction m_LevelEditor_PreviousPage;
    private readonly InputAction m_LevelEditor_Rotate;
    private readonly InputAction m_LevelEditor_ToggleObjectRotation;
    public struct LevelEditorActions
    {
        private @PlayerControls m_Wrapper;
        public LevelEditorActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @MoveMarker => m_Wrapper.m_LevelEditor_MoveMarker;
        public InputAction @Place => m_Wrapper.m_LevelEditor_Place;
        public InputAction @RaiseMarker => m_Wrapper.m_LevelEditor_RaiseMarker;
        public InputAction @LowerMarker => m_Wrapper.m_LevelEditor_LowerMarker;
        public InputAction @Pause => m_Wrapper.m_LevelEditor_Pause;
        public InputAction @MoveBlockSelection => m_Wrapper.m_LevelEditor_MoveBlockSelection;
        public InputAction @Remove => m_Wrapper.m_LevelEditor_Remove;
        public InputAction @NextPage => m_Wrapper.m_LevelEditor_NextPage;
        public InputAction @PreviousPage => m_Wrapper.m_LevelEditor_PreviousPage;
        public InputAction @Rotate => m_Wrapper.m_LevelEditor_Rotate;
        public InputAction @ToggleObjectRotation => m_Wrapper.m_LevelEditor_ToggleObjectRotation;
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
                @Pause.started -= m_Wrapper.m_LevelEditorActionsCallbackInterface.OnPause;
                @Pause.performed -= m_Wrapper.m_LevelEditorActionsCallbackInterface.OnPause;
                @Pause.canceled -= m_Wrapper.m_LevelEditorActionsCallbackInterface.OnPause;
                @MoveBlockSelection.started -= m_Wrapper.m_LevelEditorActionsCallbackInterface.OnMoveBlockSelection;
                @MoveBlockSelection.performed -= m_Wrapper.m_LevelEditorActionsCallbackInterface.OnMoveBlockSelection;
                @MoveBlockSelection.canceled -= m_Wrapper.m_LevelEditorActionsCallbackInterface.OnMoveBlockSelection;
                @Remove.started -= m_Wrapper.m_LevelEditorActionsCallbackInterface.OnRemove;
                @Remove.performed -= m_Wrapper.m_LevelEditorActionsCallbackInterface.OnRemove;
                @Remove.canceled -= m_Wrapper.m_LevelEditorActionsCallbackInterface.OnRemove;
                @NextPage.started -= m_Wrapper.m_LevelEditorActionsCallbackInterface.OnNextPage;
                @NextPage.performed -= m_Wrapper.m_LevelEditorActionsCallbackInterface.OnNextPage;
                @NextPage.canceled -= m_Wrapper.m_LevelEditorActionsCallbackInterface.OnNextPage;
                @PreviousPage.started -= m_Wrapper.m_LevelEditorActionsCallbackInterface.OnPreviousPage;
                @PreviousPage.performed -= m_Wrapper.m_LevelEditorActionsCallbackInterface.OnPreviousPage;
                @PreviousPage.canceled -= m_Wrapper.m_LevelEditorActionsCallbackInterface.OnPreviousPage;
                @Rotate.started -= m_Wrapper.m_LevelEditorActionsCallbackInterface.OnRotate;
                @Rotate.performed -= m_Wrapper.m_LevelEditorActionsCallbackInterface.OnRotate;
                @Rotate.canceled -= m_Wrapper.m_LevelEditorActionsCallbackInterface.OnRotate;
                @ToggleObjectRotation.started -= m_Wrapper.m_LevelEditorActionsCallbackInterface.OnToggleObjectRotation;
                @ToggleObjectRotation.performed -= m_Wrapper.m_LevelEditorActionsCallbackInterface.OnToggleObjectRotation;
                @ToggleObjectRotation.canceled -= m_Wrapper.m_LevelEditorActionsCallbackInterface.OnToggleObjectRotation;
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
                @Pause.started += instance.OnPause;
                @Pause.performed += instance.OnPause;
                @Pause.canceled += instance.OnPause;
                @MoveBlockSelection.started += instance.OnMoveBlockSelection;
                @MoveBlockSelection.performed += instance.OnMoveBlockSelection;
                @MoveBlockSelection.canceled += instance.OnMoveBlockSelection;
                @Remove.started += instance.OnRemove;
                @Remove.performed += instance.OnRemove;
                @Remove.canceled += instance.OnRemove;
                @NextPage.started += instance.OnNextPage;
                @NextPage.performed += instance.OnNextPage;
                @NextPage.canceled += instance.OnNextPage;
                @PreviousPage.started += instance.OnPreviousPage;
                @PreviousPage.performed += instance.OnPreviousPage;
                @PreviousPage.canceled += instance.OnPreviousPage;
                @Rotate.started += instance.OnRotate;
                @Rotate.performed += instance.OnRotate;
                @Rotate.canceled += instance.OnRotate;
                @ToggleObjectRotation.started += instance.OnToggleObjectRotation;
                @ToggleObjectRotation.performed += instance.OnToggleObjectRotation;
                @ToggleObjectRotation.canceled += instance.OnToggleObjectRotation;
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
        void OnRotateCameraRight(InputAction.CallbackContext context);
        void OnRotateCameraLeft(InputAction.CallbackContext context);
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
        void OnPause(InputAction.CallbackContext context);
        void OnMoveBlockSelection(InputAction.CallbackContext context);
        void OnRemove(InputAction.CallbackContext context);
        void OnNextPage(InputAction.CallbackContext context);
        void OnPreviousPage(InputAction.CallbackContext context);
        void OnRotate(InputAction.CallbackContext context);
        void OnToggleObjectRotation(InputAction.CallbackContext context);
    }
}
