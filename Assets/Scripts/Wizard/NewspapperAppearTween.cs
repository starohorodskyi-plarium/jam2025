using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace Wizard
{
    public class NewspapperAppearTween : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private float duration;
        
        public void Show() => 
            canvasGroup.DOFade(1f, duration);

        public void Hide() =>
            canvasGroup.alpha = 0f;
    }
}
