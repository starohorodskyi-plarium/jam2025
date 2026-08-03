using Gyro;
using UnityEngine;

namespace Platform
{
    public class GyroController : MonoBehaviour
    {
        [Header("Gyro Aim (Mobile)")]
        [SerializeField] private GyroscopeSystem _gyroscopeSystem;
        [SerializeField] private float _gyroSensitivity = 5f;
        
        public static Vector2 GyroInputOffset;
        
        public static bool UsingGyroOffset { get; private set; }

        private void Awake() =>
            ResetGyro();

        private void Start() =>
            // в браузере гироскоп доступен не всегда — IsGyroEnabled это учитывает
            UsingGyroOffset = WGPlatform.IsMobile && _gyroscopeSystem && _gyroscopeSystem.IsGyroEnabled;

        private void OnEnable() => 
            GyroscopeSystem.GyroInput += GyroInput;

        private void GyroInput(Vector2 obj) => 
            ActivateMove(obj * _gyroSensitivity);

        private void OnDisable() => 
            GyroscopeSystem.GyroInput -= GyroInput;

        private static void ActivateMove(Vector2 move) => 
            GyroInputOffset = new Vector2(GyroInputOffset.x - move.y, GyroInputOffset.y + move.x);
        
        public static void ResetGyro() => 
            GyroInputOffset = Vector2.zero;
    }
}
