using UnityEngine;

namespace Platform
{
    /// <summary>
    /// Переключатель вибрации для кнопки настроек. Вешается на саму кнопку,
    /// метод SwitchVibration привязывается к её OnClick.
    /// </summary>
    public class VibrationSettings : MonoBehaviour
    {
        [Tooltip("Импульс-подтверждение при включении вибрации, миллисекунды.")]
        [SerializeField, Min(0)] private int _confirmPulseMs = 40;

        public void SwitchVibration()
        {
            WGVibration.Enabled = !WGVibration.Enabled;

            // дать почувствовать, что вибрация включилась
            if (WGVibration.Enabled)
                WGVibration.Vibrate(_confirmPulseMs);
            else
                WGVibration.Stop();
        }
    }
}
