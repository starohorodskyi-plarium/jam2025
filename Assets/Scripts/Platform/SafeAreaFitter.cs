using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Platform
{
    [RequireComponent(typeof(RectTransform))]
    public class SafeAreaFitter : MonoBehaviour
    {
        [FormerlySerializedAs("canvas")] public Canvas Canvas;

        [FormerlySerializedAs("ignoreHeight")] public bool IgnoreHeight;

        [Header("Запасные отступы для мобильного браузера")]
        [Tooltip("В вебе настоящие вырезы экрана недоступны, поэтому берутся усреднённые значения.\n" +
                 "Отступ слева и справа, % от ширины экрана.")]
        [SerializeField, Range(0f, 15f)] private float _webSideInsetPercent = 3.5f;

        [Tooltip("Отступ снизу (жест-бар, домашняя полоса), % от высоты экрана.")]
        [SerializeField, Range(0f, 15f)] private float _webBottomInsetPercent = 5f;

        [Tooltip("Применять эти отступы и в редакторе — чтобы подобрать значения, не собирая билд.")]
        [SerializeField] private bool _previewInEditor;

        [FormerlySerializedAs("leftOffset")] [HideInInspector] public int LeftOffset;

        [FormerlySerializedAs("rightOffset")] [HideInInspector] public int RightOffset;

        private Vector2Int _lastScreenSize;

        private void Awake() =>
            FitRect();

        private void Update()
        {
            // браузер меняет размер канваса при повороте и при сворачивании адресной строки
            if (_lastScreenSize.x != Screen.width || _lastScreenSize.y != Screen.height)
                FitRect();
        }

        private void FitRect()
        {
            _lastScreenSize = new Vector2Int(Screen.width, Screen.height);

            if (!Canvas)
                Canvas = GetComponentInParent<Canvas>();

            if (!Canvas)
            {
                Debug.LogWarning($"[{nameof(SafeAreaFitter)}] {name}: {nameof(Canvas)} is not assigned", this);
                return;
            }

            var rectTransform = GetComponent<RectTransform>();
            var safeArea = GetSafeArea();

            // sizeDelta корневого Canvas — это размер экрана в единицах канваса,
            // поэтому отношение к нему и есть scaleFactor: им переводим пиксели в единицы.
            var canvasSize = Canvas.GetComponent<RectTransform>().sizeDelta;
            var scaleFactor = canvasSize.x > 0f ? Screen.width / canvasSize.x : 1f;

            LeftOffset = (int) (safeArea.x / scaleFactor);
            RightOffset = (int) ((Screen.width - (safeArea.width + safeArea.x)) / scaleFactor);

            rectTransform.SetLeft(LeftOffset);
            rectTransform.SetRight(RightOffset);

            if (IgnoreHeight)
                return;

            rectTransform.SetTop((Screen.height - (safeArea.height + safeArea.y)) / scaleFactor);
            rectTransform.SetBottom(safeArea.y / scaleFactor);
        }

        /// <summary>
        /// В вебе Screen.safeArea всегда равен всему экрану: браузер не отдаёт вырезы в Unity.
        /// Для мобильного браузера подставляем усреднённые отступы по бокам и снизу.
        /// </summary>
        private Rect GetSafeArea()
        {
            var safeArea = Screen.safeArea;

            var useFallback = WGPlatform.IsMobileBrowser || (_previewInEditor && Application.isEditor);
            if (!useFallback)
                return safeArea;

            var isWholeScreen = Mathf.Approximately(safeArea.width, Screen.width)
                                && Mathf.Approximately(safeArea.height, Screen.height);

            // если браузер всё же отдал настоящие вырезы — доверяем им
            if (!isWholeScreen)
                return safeArea;

            var side = Screen.width * (_webSideInsetPercent / 100f);
            var bottom = Screen.height * (_webBottomInsetPercent / 100f);

            return new Rect(side, bottom, Screen.width - side * 2f, Screen.height - bottom);
        }
    }

    public static class RectTransformExtensions
    {
        public static void SetLeft(this RectTransform rt, float left) =>
            rt.offsetMin = new Vector2(left, rt.offsetMin.y);

        public static void SetRight(this RectTransform rt, float right) =>
            rt.offsetMax = new Vector2(-right, rt.offsetMax.y);

        public static void SetTop(this RectTransform rt, float top) =>
            rt.offsetMax = new Vector2(rt.offsetMax.x, -top);

        public static void SetBottom(this RectTransform rt, float bottom) =>
            rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
    }
}
