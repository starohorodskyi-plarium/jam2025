using UnityEngine;

namespace Platform
{
    /// <summary>
    /// На сенсоре касания должны оставаться касаниями. Legacy Input Manager иначе
    /// эмулирует по ним мышь, и в мобильном браузере второй палец приходит как Mouse1 —
    /// то есть как правый клик, которого браузер на самом деле не присылает.
    /// </summary>
    public static class InputBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void DisableMouseSimulationOnTouch()
        {
            if (!WGPlatform.IsMobile)
                return;

            Input.simulateMouseWithTouches = false;
        }
    }
}
