using System;
using UnityEngine;
using UnityEngine.InputSystem;
using GyroscopeDevice = UnityEngine.InputSystem.Gyroscope;

namespace Gyro
{
    public class GyroscopeSystem : MonoBehaviour
    {
        public static Action<Vector2> GyroInput;

        public bool IsGyroEnabled { get; private set; }

        private void Awake()
        {
            IsGyroEnabled = GyroscopeDevice.current != null;
            Debug.Log("Device Gyro is active? - " + IsGyroEnabled);
        }

        private void OnEnable() => ActivateGyro();

        private void OnDisable() => DeactivateGyro();

        private void ActivateGyro()
        {
            if (!IsGyroEnabled) 
                return;
            
            InputSystem.EnableDevice(GyroscopeDevice.current);
            Debug.Log("Gyro on");
        }
        private void DeactivateGyro()
        {
            if (!IsGyroEnabled) 
                return;
            
            InputSystem.DisableDevice(GyroscopeDevice.current);
            Debug.Log("Gyro off");
        }

        private void OnApplicationPause (bool isPaused)
        {
            if (isPaused) DeactivateGyro();
            else ActivateGyro();
        }

        private void OnApplicationFocus (bool hasFocus)
        {
            if (!hasFocus) DeactivateGyro();
            else ActivateGyro();
        }

        private void Update()
        {
            if (IsGyroEnabled) 
                GyroInput?.Invoke(GyroscopeDevice.current.angularVelocity.ReadValue());
        }
    }
}