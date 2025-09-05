using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    [RequireComponent(typeof(RectTransform))]
    public class SafeAreaFitter : MonoBehaviour
    {
        public Canvas canvas;

        public bool ignoreHeight;

        [HideInInspector] public int leftOffset;

        [HideInInspector] public int rightOffset;

        private void Awake() => FitRect();

        private void FitRect()
        {
            var rectTransform = GetComponent<RectTransform>();
            var safeArea = Screen.safeArea;

            var coefficient = Screen.width / canvas.GetComponent<RectTransform>().sizeDelta.x;
            var isMatchWidth = canvas.GetComponent<CanvasScaler>().matchWidthOrHeight == 0;

            leftOffset = (int) (safeArea.x / (isMatchWidth ? coefficient : 1));
            rightOffset = (int) ((Screen.width - (safeArea.width + safeArea.x)) / (isMatchWidth ? coefficient : 1));

            rectTransform.SetLeft(leftOffset);
            rectTransform.SetRight(rightOffset);

            if (ignoreHeight) return;
            rectTransform.SetTop((Screen.height - (safeArea.height + safeArea.y)) / (isMatchWidth ? 1 : coefficient));
            rectTransform.SetBottom(safeArea.y / (isMatchWidth ? 1 : coefficient));
        }
    }

    public static class RectTransformExtensions
    {
        public static void SetLeft(this RectTransform rt, float left)
        {
            rt.offsetMin = new Vector2(left, rt.offsetMin.y);
        }

        public static void SetRight(this RectTransform rt, float right)
        {
            rt.offsetMax = new Vector2(-right, rt.offsetMax.y);
        }

        public static void SetTop(this RectTransform rt, float top)
        {
            rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
        }

        public static void SetBottom(this RectTransform rt, float bottom)
        {
            rt.offsetMin = new Vector2(rt.offsetMin.x, bottom);
        }
    }
}