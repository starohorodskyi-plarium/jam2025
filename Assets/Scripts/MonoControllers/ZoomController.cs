using System;
using DG.Tweening;
using GameCursor;
using UnityEngine;

namespace MonoControllers
{
    public class ZoomController : MonoBehaviour
    {
        [SerializeField] private CursorTexture cursor;
        
        public float zoomFOV = 40f;
        public float normalFOV = 60f;
        public float duration = 0.5f;

        public event Action<bool> OnZoomChanged; 
        
        private bool isZoomed;

        private Camera _camera;
        
        public void ResetZoom()
        {
            isZoomed = false;
            
            _camera.DOFieldOfView(normalFOV, duration).SetEase(Ease.OutQuad);
            
            OnZoomChanged?.Invoke(false);
        }

        private void Awake() => 
            _camera = Camera.main;

        private void Update()
        {
            if (GameManager.Instance.InputEnabled && Input.GetKeyDown(KeyCode.Mouse1))
                ToggleFOV();
        }

        private void ToggleFOV()
        {
            if (isZoomed)
                _camera.DOFieldOfView(normalFOV, duration).SetEase(Ease.OutQuad);
            else
                _camera.DOFieldOfView(zoomFOV, duration).SetEase(Ease.OutQuad);

            isZoomed = !isZoomed;
            
            OnZoomChanged?.Invoke(isZoomed);
        }

        private void OnEnable() => 
            OnZoomChanged += ChangeCursor;

        private void OnDisable() =>
            OnZoomChanged -= ChangeCursor;
        
        private void ChangeCursor(bool zoomed)
        {
            if (!cursor)
                return;

            if (zoomed)
                cursor.SetCustomZoomCursor();
            else
                cursor.SetCustomCursor();
        }
    }
}
