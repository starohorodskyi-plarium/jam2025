using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace Wizard
{
    public class NewspapperAppearTween : MonoBehaviour
    {
        [FormerlySerializedAs("canvasGroup")] [SerializeField] private CanvasGroup _canvasGroup;
        [FormerlySerializedAs("duration")] [SerializeField] private float _duration;
        
        public void Show() => 
            _canvasGroup.DOFade(1f, _duration);

        public void Hide() =>
            _canvasGroup.alpha = 0f;
    }
}
