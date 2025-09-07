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
        }
    }
}
