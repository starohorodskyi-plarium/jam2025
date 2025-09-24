using System;
using Core;
using DG.Tweening;
using GameCursor;
using UnityEngine;
using UnityEngine.Serialization;

namespace MonoControllers.Camera
{
    public class ZoomController : MonoBehaviour
    {
        [FormerlySerializedAs("cursor")] [SerializeField] private CursorTexture _cursor;
        
        [FormerlySerializedAs("zoomFOV")] public float ZoomFOV = 40f;
        [FormerlySerializedAs("normalFOV")] public float NormalFOV = 60f;
        [FormerlySerializedAs("duration")] public float Duration = 0.5f;

        public event Action<bool?> OnZoomChanged; 
        
        private bool isZoomed;

        private UnityEngine.Camera _camera;
        
        public void ResetZoom()
        {
            isZoomed = false;
            
            _camera.DOFieldOfView(NormalFOV, Duration).SetEase(Ease.OutQuad);
            
            OnZoomChanged?.Invoke(null);
        }

        private void Awake() => 
            _camera = UnityEngine.Camera.main;

        private void Update()
        {
#if UNITY_STANDALONE || UNITY_WEBGL || UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Mouse1))
                ToggleFOV();
#endif
        }

        public void ToggleFOV()
        {
            if (!GameManager.Instance.InputEnabled) 
                return;
            
            if (isZoomed)
                _camera.DOFieldOfView(NormalFOV, Duration).SetEase(Ease.OutQuad);
            else
                _camera.DOFieldOfView(ZoomFOV, Duration).SetEase(Ease.OutQuad);

            isZoomed = !isZoomed;
            
            OnZoomChanged?.Invoke(isZoomed);
        }

        private void OnEnable() => 
            OnZoomChanged += ChangeCursor;

        private void OnDisable() =>
            OnZoomChanged -= ChangeCursor;
        
        private void ChangeCursor(bool? zoomed)
        {
            if (!_cursor)
                return;
            
            if (!zoomed.HasValue)
                _cursor.SetDefaultCursor();
            else if (zoomed.Value)
                _cursor.SetCustomZoomCursor();
            else
                _cursor.SetCustomCursor();
        }
    }
}
