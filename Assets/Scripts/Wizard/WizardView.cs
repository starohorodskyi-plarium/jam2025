using TMPro;
using UnityEngine;

namespace Wizard
{
    public class WizardView : MonoBehaviour
    {
        [SerializeField] private GameObject _container;
        [SerializeField] private TMP_Text _text;
        
        public void Show(WizardSlide slide)
        {
            _container.SetActive(true);
            _text.text = slide.Text;
        }

        public void Hide()
        {
            _container.SetActive(false);
            _text.text = string.Empty;
        }
    }
}
