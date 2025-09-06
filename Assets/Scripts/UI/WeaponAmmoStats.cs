using Gun;
using TMPro;
using UnityEngine;

namespace UI
{
    public class WeaponAmmoStats : MonoBehaviour
    {
        [SerializeField] private TMP_Text _maxAmmoText;
        [SerializeField] private TMP_Text _currentAmmoText;

        public void UpdateStats(WeaponAmmoData ammoData)
        {
            _maxAmmoText.text = ammoData.Max.ToString();
            _currentAmmoText.text = ammoData.Current.ToString();
        }
    }
}
