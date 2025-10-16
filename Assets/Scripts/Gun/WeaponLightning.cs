using UnityEngine;
using DG.Tweening;

namespace Gun
{
    public class WeaponLightning : MonoBehaviour
    {
        private const float FullOpacity = 1f;
        
        [SerializeField] private SpriteSwapper _spriteSwapper;
        [SerializeField] private float _darkOpacity = 0.3f;
        
        [Header("Tween Settings")]
        [SerializeField] private float _transitionDuration = 0.3f;
        [SerializeField] private Ease _transitionEase = Ease.InOutQuad;
        
        private float _currentOpacity = FullOpacity;
        private Tween _opacityTween;

        private void OnEnable() => 
            SpriteSwapper.SpriteChanged += UpdateOpacity;

        private void OnDisable()
        {
            SpriteSwapper.SpriteChanged -= UpdateOpacity;
            _opacityTween?.Kill();
        }
        
        public void ShootReaction()
        {
            _opacityTween?.Kill();
            _opacityTween = DOTween.To(() => _currentOpacity, v => _currentOpacity = v, FullOpacity, _transitionDuration)
                .SetEase(_transitionEase)
                .OnUpdate(UpdateOpacity)
                .OnComplete(SetDarkMode);
        }
        
        public void ThunderReaction()
        {
            _opacityTween?.Kill();
            _opacityTween = DOTween.To(() => _currentOpacity, v => _currentOpacity = v, FullOpacity, 0.5f)
                .SetEase(_transitionEase)
                .OnUpdate(UpdateOpacity)
                .OnComplete(SetDarkMode);
        }

        private void SetDarkMode()
        {
            _opacityTween?.Kill();
            _opacityTween = DOTween.To(() => _currentOpacity, v => _currentOpacity = v, _darkOpacity, _transitionDuration)
                .SetEase(_transitionEase)
                .OnUpdate(UpdateOpacity);
        }
        
        public void SetLightModeInstant()
        {
            _opacityTween?.Kill();
            _currentOpacity = FullOpacity;
            UpdateOpacityForAll();
        }
        
        public void SetDarkModeInstant()
        {
            _opacityTween?.Kill();
            _currentOpacity = _darkOpacity;
            UpdateOpacityForAll();
        }

        private void UpdateOpacity() => 
            _spriteSwapper.ActiveSprite.color = new Color(_currentOpacity, _currentOpacity, _currentOpacity,1f);
        
        private void UpdateOpacityForAll() => 
            _spriteSwapper.AllSprites.ForEach(x => x.color = new Color(_currentOpacity, _currentOpacity, _currentOpacity,1f));


        private void OnDestroy() => 
            _opacityTween?.Kill();
    }
}
