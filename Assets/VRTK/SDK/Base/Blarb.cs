namespace VRTK
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [AttributeUsage(AttributeTargets.Enum)]
    public sealed class SDK_DeviceEnumAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class SDK_DeviceEnumCaseAttribute : Attribute
    {
        public readonly int minimum;
        public readonly int maximum;

        public SDK_DeviceEnumCaseAttribute(int minimum, int maximum)
        {
            if (minimum > maximum)
            {
                throw new ArgumentException("`maximum` needs to be greater than or equal to `minimum`.", "maximum");
            }

            this.minimum = minimum;
            this.maximum = maximum;
        }
    }

    [AttributeUsage(AttributeTargets.Enum)]
    public sealed class SDK_SurfaceEnumAttribute : Attribute
    {
        public readonly int deviceCaseValue;

        public SDK_SurfaceEnumAttribute(int deviceCaseValue)
        {
            this.deviceCaseValue = deviceCaseValue;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class SDK_SurfaceEnumCaseAttribute : Attribute
    {
        public readonly Type payloadType;

        public SDK_SurfaceEnumCaseAttribute(Type payloadType)
        {
            this.payloadType = payloadType;
        }
    }

    public class SDKFrontendExample
    {
        [SDK_DeviceEnum]
        public enum Device
        {
            [SDK_DeviceEnumCase(1, 1)]
            Headset,
            [SDK_DeviceEnumCase(1, 1)]
            Controller,
            [SDK_DeviceEnumCase(0, 16)]
            Tracker
        }

        [SDK_SurfaceEnum((int)Device.Controller)]
        public enum ControllerSurface
        {
            [SDK_SurfaceEnumCase(typeof(bool))]
            ApplicationButton,
            [SDK_SurfaceEnumCase(typeof(float))]
            TouchpadAxisX,
            [SDK_SurfaceEnumCase(typeof(float))]
            TouchpadAxisY
        }

        // No SDK_SurfaceEnum for Device.Headset or Device.Tracker means:
        // No surfaces available on that device!

        // Need to make sure there is only *one*
        // - SDK_SurfaceEnum per Device
        // - SDK_DeviceEnum per SDK frontend
        //
        // Also: Every SDK_SurfaceEnum needs to have its cases defined with
        // SDK_SurfaceEnumCase(aType)
    }

    public class VirtualEvent
    {
        public string name;
        public Type payloadType
        {
            get
            {
                return cachedPayloadType;
            }
            set
            {
                payloadTypeName = value.FullName;
                cachedPayloadType = value;
            }
        }
        public Dictionary<DeviceScope, List<int>> surfaceEvents;

        //the following field in combination with the payloadType setter above is needed because Unity doesn't serialize Types
        //this should only be allowed to be changed in the above payloadType setter so the cached payloadType will be invalidated correctly
        [SerializeField]
        private string payloadTypeName;
        private Type cachedPayloadType;

        public sealed class DeviceScope
        {
            /// The name of the SDK frontend.
            public readonly string sdkFrontendTypeName;
            /// The enum case index of the enum type annotated with <see cref="SDK_DeviceEnumAttribute"/>.
            public readonly int sdkFrontendDevice;

            public DeviceScope(string sdkFrontendTypeName, int sdkFrontendDevice)
            {
                this.sdkFrontendTypeName = sdkFrontendTypeName;
                this.sdkFrontendDevice = sdkFrontendDevice;
            }
        }
    }

    public class Bridge
    {
        public Dictionary<string, List<VirtualEvent>> virtualEventsByName;
        public Transform trackedTransform;

        private Dictionary<Type, Dictionary<string, List<Delegate>>> callbacksByTypeAndVirtualEventName;

        public void Subscribe(string virtualEventName, Action callback)
        {
            Subscribe(virtualEventName, typeof(void), callback);
        }

        public void Subscribe<TPayload>(string virtualEventName, Action<TPayload> callback)
        {
            Subscribe(virtualEventName, typeof(TPayload), callback);
        }

        public void Unsubscribe(string virtualEventName, Action callback)
        {
            Unsubscribe(virtualEventName, typeof(void), callback);
        }

        public void Unsubscribe<TPayload>(string virtualEventName, Action<TPayload> callback)
        {
            Unsubscribe(virtualEventName, typeof(TPayload), callback);
        }

        private void Subscribe(string virtualEventName, Type payloadType, Delegate callback)
        {
            if (!virtualEventsByName.ContainsKey(virtualEventName))
            {
                throw new ArgumentException(string.Format("There is no `VirtualEvent` with the name '{0}' in `virtualEventsByName` of this `Bridge`.", virtualEventName), "virtualEventName");
            }

            Dictionary<string, List<Delegate>> callbacksByVirtualEventName;
            if (!callbacksByTypeAndVirtualEventName.TryGetValue(payloadType, out callbacksByVirtualEventName))
            {
                callbacksByVirtualEventName = new Dictionary<string, List<Delegate>>();
                callbacksByTypeAndVirtualEventName[payloadType] = callbacksByVirtualEventName;
            }

            List<Delegate> callbacks;
            if (!callbacksByVirtualEventName.TryGetValue(virtualEventName, out callbacks))
            {
                callbacks = new List<Delegate>();
                callbacksByVirtualEventName[virtualEventName] = callbacks;
            }

            callbacks.Add(callback);
        }

        private void Unsubscribe(string virtualEventName, Type payloadType, Delegate callback)
        {
            Dictionary<string, List<Delegate>> callbacksByVirtualEventName;
            if (!callbacksByTypeAndVirtualEventName.TryGetValue(payloadType, out callbacksByVirtualEventName))
            {
                throw new ArgumentException(string.Format("There is no `VirtualEvent` subscription registered for the payload type '{0}' at all.", payloadType.Name));
            }

            List<Delegate> callbacks;
            if (!callbacksByVirtualEventName.TryGetValue(virtualEventName, out callbacks))
            {
                throw new ArgumentException(string.Format("There is no subscription registered to the `VirtualEvent` with the name '{0}' and the payload type '{1}'.", virtualEventName, payloadType.Name), "virtualEventName");
            }

            if (callbacks.Remove(callback)
                && callbacks.Count == 0
                && callbacksByVirtualEventName.Remove(virtualEventName)
                && callbacksByVirtualEventName.Count == 0)
            {
                callbacksByTypeAndVirtualEventName.Remove(payloadType);
            }
        }
    }
}
