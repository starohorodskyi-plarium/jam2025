using UnityEngine;
using DG.Tweening;

namespace MonoControllers.Camera
{
    public class ZoomSensitivityUpdater : MonoBehaviour
    {
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private ZoomController _zoomController;

        [SerializeField] private float _zoomSensitivityX = 12;
        [SerializeField] private float _zoomSensitivityY = 12;
        [SerializeField] private float _maxAngleX = 1.5f;
        [SerializeField] private float _maxAngleY = 1.5f;
        
        [Header("Limits")]
        [SerializeField] private float _zoomMinPitch = -10f;
        [SerializeField] private float _zoomMaxPitch = 10f;
        [SerializeField] private float _zoomMinYaw   = -10f;
        [SerializeField] private float _zoomMaxYaw   = 10f;
        
        private float _defaultMinPitch;
        private float _defaultMaxPitch;
        private float _defaultMinYaw;
        private float _defaultMaxYaw;

		private float _defaultSensitivityX;
		private float _defaultSensitivityY;
		private float _defaultAngleX;
		private float _defaultAngleY;

		[Space]
		[SerializeField] private float _transitionDuration = 0.5f;
		[SerializeField] private Ease _transitionEase = Ease.OutQuad;

		private Sequence _transitionTween;
        
        private void Awake()
        {
            _defaultSensitivityX = _cameraController.SensitivityX;
            _defaultSensitivityY = _cameraController.SensitivityY;
            _defaultAngleX = _cameraController.MaxXAngle;
            _defaultAngleY = _cameraController.MaxYAngle;
            
            _defaultMinPitch = _cameraController.MinPitch;
            _defaultMaxPitch = _cameraController.MaxPitch;
            _defaultMinYaw   = _cameraController.MinYaw;
            _defaultMaxYaw   = _cameraController.MaxYaw;
        }

        private void OnEnable() => 
            _zoomController.OnZoomChanged += UpdateSensitivity;

		private void OnDisable()
		{
			_zoomController.OnZoomChanged -= UpdateSensitivity;
			if (_transitionTween != null && _transitionTween.IsActive())
				_transitionTween.Kill();
		}

		private void UpdateSensitivity(bool? zoomed)
		{
			var isZoomed = zoomed.HasValue && zoomed.Value;
			
			if (_transitionTween != null && _transitionTween.IsActive())
				_transitionTween.Kill();

			var targetSensitivityX = isZoomed ? _zoomSensitivityX : _defaultSensitivityX;
			var targetSensitivityY = isZoomed ? _zoomSensitivityY : _defaultSensitivityY;
			var targetMaxXAngle = isZoomed ? _maxAngleX : _defaultAngleX;
			var targetMaxYAngle = isZoomed ? _maxAngleY : _defaultAngleY;
			var targetMinPitch = isZoomed ? _zoomMinPitch : _defaultMinPitch;
			var targetMaxPitch = isZoomed ? _zoomMaxPitch : _defaultMaxPitch;
			var targetMinYaw = isZoomed ? _zoomMinYaw : _defaultMinYaw;
			var targetMaxYaw = isZoomed ? _zoomMaxYaw : _defaultMaxYaw;

			_transitionTween = DOTween.Sequence();
			_transitionTween.SetEase(_transitionEase);
			_transitionTween.Join(DOTween.To(() => _cameraController.SensitivityX, v => _cameraController.SensitivityX = v, targetSensitivityX, _transitionDuration));
			_transitionTween.Join(DOTween.To(() => _cameraController.SensitivityY, v => _cameraController.SensitivityY = v, targetSensitivityY, _transitionDuration));
			_transitionTween.Join(DOTween.To(() => _cameraController.MaxXAngle, v => _cameraController.MaxXAngle = v, targetMaxXAngle, _transitionDuration));
			_transitionTween.Join(DOTween.To(() => _cameraController.MaxYAngle, v => _cameraController.MaxYAngle = v, targetMaxYAngle, _transitionDuration));
			_transitionTween.Join(DOTween.To(() => _cameraController.MinPitch, v => _cameraController.MinPitch = v, targetMinPitch, _transitionDuration));
			_transitionTween.Join(DOTween.To(() => _cameraController.MaxPitch, v => _cameraController.MaxPitch = v, targetMaxPitch, _transitionDuration));
			_transitionTween.Join(DOTween.To(() => _cameraController.MinYaw, v => _cameraController.MinYaw = v, targetMinYaw, _transitionDuration));
			_transitionTween.Join(DOTween.To(() => _cameraController.MaxYaw, v => _cameraController.MaxYaw = v, targetMaxYaw, _transitionDuration));
			_transitionTween.SetLink(gameObject);

			if (zoomed == null)
				_cameraController.ResetCameraRotationSmooth();
		}
    }
}
