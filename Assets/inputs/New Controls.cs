// GENERATED AUTOMATICALLY FROM 'Assets/inputs/New Controls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @NewControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @NewControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""New Controls"",
    ""maps"": [
        {
            ""name"": ""Track"",
            ""id"": ""9e28a29c-91bb-44ea-bd81-987dc11783b1"",
            ""actions"": [
                {
                    ""name"": ""Throttle"",
                    ""type"": ""Value"",
                    ""id"": ""b26ec5cf-4811-4907-94c0-dd28f62b92c2"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                },
                {
                    ""name"": ""Brake"",
                    ""type"": ""Value"",
                    ""id"": ""cc3921b5-7345-448c-a739-25ed3d61f470"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                },
                {
                    ""name"": ""Steering"",
                    ""type"": ""Value"",
                    ""id"": ""89e1f623-6a36-471d-b5e3-13d3766bd84f"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                },
                {
                    ""name"": ""AxisThrottle"",
                    ""type"": ""Value"",
                    ""id"": ""007f0f46-9f12-4b55-82d5-134cd8723e1a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": ""Press(behavior=2)""
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""80a10106-baa6-4f2d-b725-0e4486882e4e"",
                    ""path"": ""<HID::obp̌pedals̞obp ped obp pedals⟥⟩⟱Ϧ>/rx"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Throttle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""01ac6beb-02f0-4101-bc54-d88112e28f8e"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Throttle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cc72bbbe-df2f-4242-adc4-999910344a0c"",
                    ""path"": ""<XInputController>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Throttle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9c267555-4d0c-434c-8a77-04f2500bf05c"",
                    ""path"": ""<HID::obp̌pedals̞obp ped obp pedals⟥⟩⟱Ϧ>/ry"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Brake"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2131447b-f3b5-4b4d-9623-71da866a69bd"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Brake"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""fb3b7d49-f234-4c85-9c19-adcf807e5f83"",
                    ""path"": ""<XInputController>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Brake"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0809ea5e-bc2b-4652-b1fd-363a79ff2834"",
                    ""path"": ""<HID::Granite Devices Simucube 2 Pro>/z"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Steering"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""SteeringAxis"",
                    ""id"": ""208c6df6-7153-4bbb-b037-bb62c5be44e5"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Steering"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""fab3694b-7f5f-47b6-be6f-873dc335aef5"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Steering"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""767efd75-b87c-4bab-802b-931bb846039f"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Steering"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""5b2637f4-abe7-4d9b-9602-f2f0b9ae8ec5"",
                    ""path"": ""<XInputController>/leftStick/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Steering"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0bb70f72-4dfd-4257-8960-c521f94bbe6d"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""AxisThrottle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Forwards"",
                    ""id"": ""039b43c3-0553-4483-95d6-a5ffb9c62d46"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""AxisThrottle"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""c71edc56-96d5-4ea6-9d17-7aea7069279f"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""AxisThrottle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""9c7f6ff6-d2fa-4219-9675-e9636df6f772"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""AxisThrottle"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Track
        m_Track = asset.FindActionMap("Track", throwIfNotFound: true);
        m_Track_Throttle = m_Track.FindAction("Throttle", throwIfNotFound: true);
        m_Track_Brake = m_Track.FindAction("Brake", throwIfNotFound: true);
        m_Track_Steering = m_Track.FindAction("Steering", throwIfNotFound: true);
        m_Track_AxisThrottle = m_Track.FindAction("AxisThrottle", throwIfNotFound: true);
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

    // Track
    private readonly InputActionMap m_Track;
    private ITrackActions m_TrackActionsCallbackInterface;
    private readonly InputAction m_Track_Throttle;
    private readonly InputAction m_Track_Brake;
    private readonly InputAction m_Track_Steering;
    private readonly InputAction m_Track_AxisThrottle;
    public struct TrackActions
    {
        private @NewControls m_Wrapper;
        public TrackActions(@NewControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Throttle => m_Wrapper.m_Track_Throttle;
        public InputAction @Brake => m_Wrapper.m_Track_Brake;
        public InputAction @Steering => m_Wrapper.m_Track_Steering;
        public InputAction @AxisThrottle => m_Wrapper.m_Track_AxisThrottle;
        public InputActionMap Get() { return m_Wrapper.m_Track; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(TrackActions set) { return set.Get(); }
        public void SetCallbacks(ITrackActions instance)
        {
            if (m_Wrapper.m_TrackActionsCallbackInterface != null)
            {
                @Throttle.started -= m_Wrapper.m_TrackActionsCallbackInterface.OnThrottle;
                @Throttle.performed -= m_Wrapper.m_TrackActionsCallbackInterface.OnThrottle;
                @Throttle.canceled -= m_Wrapper.m_TrackActionsCallbackInterface.OnThrottle;
                @Brake.started -= m_Wrapper.m_TrackActionsCallbackInterface.OnBrake;
                @Brake.performed -= m_Wrapper.m_TrackActionsCallbackInterface.OnBrake;
                @Brake.canceled -= m_Wrapper.m_TrackActionsCallbackInterface.OnBrake;
                @Steering.started -= m_Wrapper.m_TrackActionsCallbackInterface.OnSteering;
                @Steering.performed -= m_Wrapper.m_TrackActionsCallbackInterface.OnSteering;
                @Steering.canceled -= m_Wrapper.m_TrackActionsCallbackInterface.OnSteering;
                @AxisThrottle.started -= m_Wrapper.m_TrackActionsCallbackInterface.OnAxisThrottle;
                @AxisThrottle.performed -= m_Wrapper.m_TrackActionsCallbackInterface.OnAxisThrottle;
                @AxisThrottle.canceled -= m_Wrapper.m_TrackActionsCallbackInterface.OnAxisThrottle;
            }
            m_Wrapper.m_TrackActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Throttle.started += instance.OnThrottle;
                @Throttle.performed += instance.OnThrottle;
                @Throttle.canceled += instance.OnThrottle;
                @Brake.started += instance.OnBrake;
                @Brake.performed += instance.OnBrake;
                @Brake.canceled += instance.OnBrake;
                @Steering.started += instance.OnSteering;
                @Steering.performed += instance.OnSteering;
                @Steering.canceled += instance.OnSteering;
                @AxisThrottle.started += instance.OnAxisThrottle;
                @AxisThrottle.performed += instance.OnAxisThrottle;
                @AxisThrottle.canceled += instance.OnAxisThrottle;
            }
        }
    }
    public TrackActions @Track => new TrackActions(this);
    public interface ITrackActions
    {
        void OnThrottle(InputAction.CallbackContext context);
        void OnBrake(InputAction.CallbackContext context);
        void OnSteering(InputAction.CallbackContext context);
        void OnAxisThrottle(InputAction.CallbackContext context);
    }
}
