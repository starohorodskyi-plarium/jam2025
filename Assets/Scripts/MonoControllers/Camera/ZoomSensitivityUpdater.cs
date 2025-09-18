using UnityEngine;
using DG.Tweening;

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

		private void OnDisable()
		{
			_zoomController.OnZoomChanged -= UpdateSensitivity;
			if (_transitionTween != null && _transitionTween.IsActive())
			{
				_transitionTween.Kill();
			}
		}

		private void UpdateSensitivity(bool zoomed)
		{
			if (_transitionTween != null && _transitionTween.IsActive())
			{
				_transitionTween.Kill();
			}

			float targetSensitivityX = zoomed ? _zoomSensitivityX : _defaultSensitivityX;
			float targetSensitivityY = zoomed ? _zoomSensitivityY : _defaultSensitivityY;
			float targetMaxXAngle = zoomed ? _maxAngleX : _defaultAngleX;
			float targetMaxYAngle = zoomed ? _maxAngleY : _defaultAngleY;
			float targetMinPitch = zoomed ? _zoomMinPitch : _defaultMinPitch;
			float targetMaxPitch = zoomed ? _zoomMaxPitch : _defaultMaxPitch;
			float targetMinYaw = zoomed ? _zoomMinYaw : _defaultMinYaw;
			float targetMaxYaw = zoomed ? _zoomMaxYaw : _defaultMaxYaw;

			_transitionTween = DOTween.Sequence();
			_transitionTween.SetEase(_transitionEase);
			_transitionTween.Join(DOTween.To(() => _cameraController.sensitivityX, v => _cameraController.sensitivityX = v, targetSensitivityX, _transitionDuration));
			_transitionTween.Join(DOTween.To(() => _cameraController.sensitivityY, v => _cameraController.sensitivityY = v, targetSensitivityY, _transitionDuration));
			_transitionTween.Join(DOTween.To(() => _cameraController.maxXAngle, v => _cameraController.maxXAngle = v, targetMaxXAngle, _transitionDuration));
			_transitionTween.Join(DOTween.To(() => _cameraController.maxYAngle, v => _cameraController.maxYAngle = v, targetMaxYAngle, _transitionDuration));
			_transitionTween.Join(DOTween.To(() => _cameraController.minPitch, v => _cameraController.minPitch = v, targetMinPitch, _transitionDuration));
			_transitionTween.Join(DOTween.To(() => _cameraController.maxPitch, v => _cameraController.maxPitch = v, targetMaxPitch, _transitionDuration));
			_transitionTween.Join(DOTween.To(() => _cameraController.minYaw, v => _cameraController.minYaw = v, targetMinYaw, _transitionDuration));
			_transitionTween.Join(DOTween.To(() => _cameraController.maxYaw, v => _cameraController.maxYaw = v, targetMaxYaw, _transitionDuration));
			_transitionTween.SetLink(gameObject);
		}
    }
}
