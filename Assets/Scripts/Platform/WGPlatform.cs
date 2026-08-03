using System;
#if UNITY_WEBGL && !UNITY_EDITOR
    using System.Runtime.InteropServices;
#endif
using UnityEngine;

namespace Platform
{
    /// <summary>
    /// Снимок данных о платформе. Имена полей совпадают с ключами JSON из window.wgGetPlatformInfo().
    /// </summary>
    [Serializable]
    public class WGPlatformInfo
    {
        public string os = "desktop";        // "android" | "ios" | "desktop"
        public bool isAndroid;
        public bool isIOS;
        public bool isMobile;
        public bool isTouch;
        public bool isStandalone;
        public bool canVibrate;
        public bool isPortrait;
        public int screenWidth;
        public int screenHeight;
        public float devicePixelRatio = 1f;
        public string language = "";
        public string userAgent = "";
    }

    /// <summary>
    /// Настоящая платформа под Web-сборкой: Application.platform в вебе всегда WebGLPlayer,
    /// поэтому Android/iOS различает только браузер через window.wgGetPlatformInfo().
    /// Читать не раньше Awake() — к этому моменту JS-функция страницы уже объявлена.
    /// </summary>
    public static class WGPlatform
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")] private static extern string WGGetPlatformInfo();
#endif

        private static WGPlatformInfo _info;

        /// <summary>Сведения о платформе. Читается один раз, дальше из кеша.</summary>
        public static WGPlatformInfo Info
        {
            get
            {
                if (_info != null)
                    return _info;
#if UNITY_WEBGL && !UNITY_EDITOR
                try
                {
                    var json = WGGetPlatformInfo();
                    _info = JsonUtility.FromJson<WGPlatformInfo>(json) ?? new WGPlatformInfo();
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[WGPlatform] не удалось прочитать данные страницы: {e.Message}");
                    _info = new WGPlatformInfo();
                }
#else
                // нативные сборки и редактор: заполняем сами
                _info = new WGPlatformInfo
                {
                    os = Application.platform == RuntimePlatform.Android ? "android"
                       : Application.platform == RuntimePlatform.IPhonePlayer ? "ios" : "desktop",
                    isAndroid = Application.platform == RuntimePlatform.Android,
                    isIOS = Application.platform == RuntimePlatform.IPhonePlayer,
                    isMobile = Application.isMobilePlatform,
                    isTouch = Input.touchSupported,
                    canVibrate = Application.isMobilePlatform,
                    isPortrait = Screen.height > Screen.width,
                    screenWidth = Screen.width,
                    screenHeight = Screen.height,
                    devicePixelRatio = 1f,
                    language = Application.systemLanguage.ToString()
                };
#endif
                return _info;
            }
        }

        /// <summary>Мобильное устройство: нативная сборка под Android/iOS либо мобильный браузер.</summary>
        public static bool IsMobile => Info.isMobile;

        /// <summary>Игра открыта в браузере на телефоне или планшете.</summary>
        public static bool IsMobileBrowser =>
            Application.platform == RuntimePlatform.WebGLPlayer && Info.isMobile;

        public static bool IsAndroid => Info.isAndroid;
        public static bool IsIOS => Info.isIOS;

        /// <summary>Страница открыта как установленное приложение (с иконки), а не во вкладке браузера.</summary>
        public static bool IsInstalledApp => Info.isStandalone;

        /// <summary>Сбросить кеш (нужно только если страница меняет состояние).</summary>
        public static void Invalidate() => _info = null;
    }
}
