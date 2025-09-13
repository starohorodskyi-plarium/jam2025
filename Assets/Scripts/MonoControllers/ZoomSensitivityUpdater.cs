using UnityEngine;

namespace MonoControllers
{
    public class ZoomSensitivityUpdater : MonoBehaviour
    {
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private ZoomController _zoomController;

        [SerializeField] private float _zoomSensitivityX = 12;
        [SerializeField] private float _zoomSensitivityY = 12;
        [SerializeField] private float _maxAngleX = 1.5f;
        [SerializeField] private float _maxAngleY = 1.5f;
        
        private float _zoomMinPitch = -10f;
        private float _zoomMaxPitch = 10f;
        private float _zoomMinYaw   = -10f;
        private float _zoomMaxYaw   = 10f;
        
        private float _defaultMinPitch;
        private float _defaultMaxPitch;
        private float _defaultMinYaw;
        private float _defaultMaxYaw;

        private float _defaultSensitivityX;
        private float _defaultSensitivityY;
        private float _defaultAngleX;
        private float _defaultAngleY;
        
        private void Awake()
        {
            _defaultSensitivityX = _cameraController.sensitivityX;
            _defaultSensitivityY = _cameraController.sensitivityY;
            _defaultAngleX = _cameraController.maxXAngle;
            _defaultAngleY = _cameraController.maxYAngle;
            
            _defaultMinPitch = _cameraController.minPitch;
            _defaultMaxPitch = _cameraController.maxPitch;
            _defaultMinYaw   = _cameraController.minYaw;
            _defaultMaxYaw   = _cameraController.maxYaw;
        }

        private void OnEnable() => 
            _zoomController.OnZoomChanged += UpdateSensitivity;

        private void OnDisable() => 
            _zoomController.OnZoomChanged -= UpdateSensitivity;

        private void UpdateSensitivity(bool zoomed)
        {
            _cameraController.sensitivityX = zoomed ? _zoomSensitivityX : _defaultSensitivityX;
            _cameraController.sensitivityY = zoomed ? _zoomSensitivityY : _defaultSensitivityY;
            _cameraController.maxXAngle  = zoomed ? _maxAngleX : _defaultAngleX;
            _cameraController.maxYAngle  = zoomed ? _maxAngleY : _defaultAngleY;
            
            // clamp limits as well
            _cameraController.minPitch = zoomed ? _zoomMinPitch : _defaultMinPitch;
            _cameraController.maxPitch = zoomed ? _zoomMaxPitch : _defaultMaxPitch;
            _cameraController.minYaw   = zoomed ? _zoomMinYaw   : _defaultMinYaw;
            _cameraController.maxYaw   = zoomed ? _zoomMaxYaw   : _defaultMaxYaw;
        }
    }
}
