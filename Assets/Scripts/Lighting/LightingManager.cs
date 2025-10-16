using UnityEngine;

namespace Lighting
{
    public class LightingManager : MonoBehaviour
    {
        /// <summary>
        /// Sets the Environment Lighting intensity multiplier
        /// </summary>
        /// <param name="intensity">Value from 0 to 1</param>
        public void SetLightingIntensity(float intensity)
        {
            intensity = Mathf.Clamp01(intensity);
            RenderSettings.ambientIntensity = intensity;
        }
    }
}
