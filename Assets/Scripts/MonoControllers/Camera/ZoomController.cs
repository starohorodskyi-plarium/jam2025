using System;
using Core;
using DG.Tweening;
using GameCursor;
using Platform;
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
        [Header("Audio")]
        [SerializeField] private AudioSource _zoomAudio;
        [SerializeField] private AudioClip _zoomAudioClip;
        [SerializeField] private float _zoomInPitch;
        [SerializeField] private float _zoomOutPitch;

        public event Action<bool?> OnZoomChanged; 
        
        public bool IsZoomed;

        private UnityEngine.Camera _camera;
        
        public void ResetZoom()
        {
            IsZoomed = false;
            
            _camera.DOFieldOfView(NormalFOV, Duration).SetEase(Ease.OutQuad);
            
            OnZoomChanged?.Invoke(null);
        }

        private void Awake() => 
            _camera = UnityEngine.Camera.main;

        private void Update()
        {
            // На сенсоре прицел включает только своя экранная зона (MobileControls/Zoom),
            // которая дёргает ToggleFOV через кнопку. Кнопки мыши здесь не читаем:
            // в мобильном браузере второй палец приходит как Mouse1 и включал прицел сам.
            if (WGPlatform.IsMobile)
                return;

#if UNITY_STANDALONE || UNITY_WEBGL || UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Mouse1))
                ToggleFOV();
#endif
        }

        public void ToggleFOV()
        {
            if (!GameManager.Instance.InputEnabled) 
                return;
            
            if (IsZoomed)
            {
                _camera.DOFieldOfView(NormalFOV, Duration).SetEase(Ease.OutQuad);
                PlayZoomSound(false);
            }
            else
            {
                _camera.DOFieldOfView(ZoomFOV, Duration).SetEase(Ease.OutQuad);
                PlayZoomSound(true);
            }

            IsZoomed = !IsZoomed;
            
            OnZoomChanged?.Invoke(IsZoomed);
        }

        private void PlayZoomSound(bool isZoomingIn)
        {
            if (!_zoomAudio || !_zoomAudioClip)
                return;

            _zoomAudio.pitch = isZoomingIn ? _zoomInPitch : _zoomOutPitch;
            _zoomAudio.PlayOneShot(_zoomAudioClip);
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
