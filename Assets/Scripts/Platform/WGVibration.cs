#if UNITY_WEBGL && !UNITY_EDITOR
    using System.Runtime.InteropServices;
#endif
using Solo.MOST_IN_ONE;
using UnityEngine;

namespace Platform
{
    /// <summary>
    /// Вибрация, работающая в Web-сборке через navigator.vibrate (мост window.wgVibrate на странице плеера).
    /// На Android/iOS проксирует в Most_HapticFeedback, чтобы сохранить нативные паттерны с амплитудой.
    /// </summary>
    public static class WGVibration
    {
        /// <summary>Минимальная пауза между импульсами: чаще Android склеивает их в гудение.</summary>
        private const float CooldownSec = .05f;

        private static float _lastVibrationTime = float.NegativeInfinity;

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")] private static extern int WGVibrateMs(int ms);
        [DllImport("__Internal")] private static extern int WGVibratePattern(string pattern);
        [DllImport("__Internal")] private static extern void WGVibrateStop();
        [DllImport("__Internal")] private static extern int WGCanVibrate();
#endif

        /// <summary>Доступна ли вибрация на текущей платформе.</summary>
        public static bool IsSupported
        {
            get
            {
#if UNITY_WEBGL && !UNITY_EDITOR
                // canVibrate из данных страницы; WGCanVibrate — резерв, если window.WGPlatform не объявлен.
                return WGPlatform.Info.canVibrate || WGCanVibrate() == 1;
#elif UNITY_ANDROID || UNITY_IOS
                // Most_HapticFeedback.IsSupported() опрашивает вибромотор, но инициализируется
                // только AfterSceneLoad — в Awake он ещё вернёт false, поэтому доверяем платформе.
                return Application.isMobilePlatform;
#else
                return false;
#endif
            }
        }

        /// <summary>
        /// Включена ли вибрация в настройках игры. Хранится в PlayerPrefs самим Most_HapticFeedback,
        /// поэтому настройка одна и та же для вебa и нативных сборок.
        /// </summary>
        public static bool Enabled
        {
            get => Most_HapticFeedback.HapticsEnabled;
            set => Most_HapticFeedback.HapticsEnabled = value;
        }

        /// <summary>Короткий импульс, миллисекунды.</summary>
        public static void Vibrate(int milliseconds = 30)
        {
            if (milliseconds <= 0 || !Enabled || !PassCooldown())
                return;

#if UNITY_WEBGL && !UNITY_EDITOR
            WGVibrateMs(milliseconds);
#elif UNITY_ANDROID || UNITY_IOS
            Most_HapticFeedback.Generate(ToHapticType(milliseconds));
#else
            Debug.Log($"[WGVibration] Vibrate({milliseconds}) — платформа не поддерживает");
#endif
        }

        /// <summary>
        /// Паттерн: пауза, вибрация, пауза, вибрация… в миллисекундах.
        /// Кулдаун не применяется — паттерны используются для редких событий вроде проигрыша.
        /// </summary>
        public static void VibratePattern(params int[] pattern)
        {
            if (pattern == null || pattern.Length == 0 || !Enabled)
                return;

            _lastVibrationTime = Time.unscaledTime;

#if UNITY_WEBGL && !UNITY_EDITOR
            WGVibratePattern(string.Join(",", pattern));
#elif UNITY_ANDROID || UNITY_IOS
            // Нативный аналог многоимпульсного паттерна — готовый waveform Failure.
            Most_HapticFeedback.Generate(Most_HapticFeedback.HapticTypes.Failure);
#else
            Debug.Log("[WGVibration] VibratePattern — платформа не поддерживает");
#endif
        }

        /// <summary>Вибрация по типу хаптики — для данных, где тип задаётся в инспекторе.</summary>
        public static void Generate(Most_HapticFeedback.HapticTypes type)
        {
            switch (type)
            {
                case Most_HapticFeedback.HapticTypes.None:
                    return;
                case Most_HapticFeedback.HapticTypes.Success:
                    VibratePattern(0, 100, 50, 100);
                    return;
                case Most_HapticFeedback.HapticTypes.Failure:
                    VibratePattern(0, 40, 40, 40);
                    return;
                default:
                    Vibrate(ToMilliseconds(type));
                    return;
            }
        }

        public static void Stop()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            WGVibrateStop();
#endif
        }

        private static bool PassCooldown()
        {
            if (Time.unscaledTime - _lastVibrationTime < CooldownSec)
                return false;

            _lastVibrationTime = Time.unscaledTime;
            return true;
        }

        /// <summary>Длительность одиночного импульса из таблицы Most_HapticFeedback.</summary>
        private static int ToMilliseconds(Most_HapticFeedback.HapticTypes type) => type switch
        {
            Most_HapticFeedback.HapticTypes.Selection => 20,
            Most_HapticFeedback.HapticTypes.SelectionPlus => 27,
            Most_HapticFeedback.HapticTypes.RigidImpact => 25,
            Most_HapticFeedback.HapticTypes.LightImpact => 50,
            Most_HapticFeedback.HapticTypes.SoftImpact => 80,
            Most_HapticFeedback.HapticTypes.MediumImpact => 100,
            Most_HapticFeedback.HapticTypes.HeavyImpact => 200,
            Most_HapticFeedback.HapticTypes.Warning => 200,
            _ => 30,
        };

        /// <summary>Обратное отображение: длительность → ближайший нативный тип хаптики.</summary>
        private static Most_HapticFeedback.HapticTypes ToHapticType(int milliseconds) => milliseconds switch
        {
            <= 25 => Most_HapticFeedback.HapticTypes.Selection,
            <= 45 => Most_HapticFeedback.HapticTypes.SelectionPlus,
            <= 90 => Most_HapticFeedback.HapticTypes.SoftImpact,
            <= 150 => Most_HapticFeedback.HapticTypes.MediumImpact,
            _ => Most_HapticFeedback.HapticTypes.HeavyImpact,
        };
    }
}
