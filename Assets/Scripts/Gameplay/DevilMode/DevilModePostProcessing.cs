using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Gameplay.DevilMode
{
    public class DevilModePostProcessing : MonoBehaviour
    {
        [SerializeField] private GameObject _horns;
        [Space] 
        [SerializeField] private Volume volume;
        [SerializeField] private bool createVolumeIfMissing = true;

        private LensDistortion lensDistortion;
        private ChromaticAberration chromaticAberration;
        private ColorAdjustments colorAdjustments;
        private Vignette vignette;

        // Vignette baseline backup for revert
        private bool vignetteBaselineSaved;
        private bool vignetteExistedBefore;
        private bool vignetteActiveBefore;
        private float vignetteIntensityBefore;
        private float vignetteSmoothnessBefore;

        public void EnableDevilLensDistortion()
        {
            EnsureVolumeAndProfile();
            if (volume == null || volume.profile == null)
            {
                Debug.LogWarning("DevilModePostProcessing: Volume or Profile is missing and could not be created.");
                return;
            }

            if (!volume.profile.TryGet(out lensDistortion))
            {
                lensDistortion = volume.profile.Add<LensDistortion>(true);
            }

            lensDistortion.active = true;
            lensDistortion.intensity.overrideState = true;
            lensDistortion.intensity.value = -0.54f;
            lensDistortion.scale.value = 1.33f;
        }
    

        public void EnableDevilPostProcessing()
        {
            EnableDevilLensDistortion();
            EnableDevilColorAdjustments();
            EnableDevilVignette();
        
            _horns.SetActive(true);
        }

        public void EnableDevilColorAdjustments()
        {
            EnsureVolumeAndProfile();
            if (volume == null || volume.profile == null)
            {
                Debug.LogWarning("DevilModePostProcessing: Volume or Profile is missing and could not be created.");
                return;
            }

            if (!volume.profile.TryGet(out colorAdjustments))
            {
                colorAdjustments = volume.profile.Add<ColorAdjustments>(true);
            }

            colorAdjustments.active = true;
            colorAdjustments.colorFilter.overrideState = true;
            colorAdjustments.colorFilter.value = new Color(166f / 255f, 0f, 0f, 0f);

            colorAdjustments.contrast.overrideState = true;
            colorAdjustments.contrast.value = -37f;

            colorAdjustments.saturation.overrideState = true;
            colorAdjustments.saturation.value = 26f;
        }

        public void EnableDevilVignette()
        {
            EnsureVolumeAndProfile();
            if (volume == null || volume.profile == null)
            {
                Debug.LogWarning("DevilModePostProcessing: Volume or Profile is missing and could not be created.");
                return;
            }

            bool existed = volume.profile.TryGet(out vignette);
            if (!existed)
            {
                vignette = volume.profile.Add<Vignette>(true);
            }

            if (!vignetteBaselineSaved)
            {
                vignetteBaselineSaved = true;
                vignetteExistedBefore = existed;
                if (existed)
                {
                    vignetteActiveBefore = vignette.active;
                    vignetteIntensityBefore = vignette.intensity.value;
                    vignetteSmoothnessBefore = vignette.smoothness.value;
                }
                else
                {
                    vignetteActiveBefore = false;
                    vignetteIntensityBefore = vignette.intensity.value;
                    vignetteSmoothnessBefore = vignette.smoothness.value;
                }
            }

            vignette.active = true;
            vignette.intensity.overrideState = true;
            vignette.intensity.value = 0.55f;
            vignette.smoothness.overrideState = true;
            vignette.smoothness.value = 0.5f;
        }

        public void RevertDevilPostProcessing()
        {
            _horns.SetActive(false);
        
            EnsureVolumeAndProfile();
            if (volume == null || volume.profile == null)
            {
                Debug.LogWarning("DevilModePostProcessing: Volume or Profile is missing and could not be created.");
                return;
            }

            if (volume.profile.TryGet(out lensDistortion))
            {
                lensDistortion.active = false;
            }

            if (volume.profile.TryGet(out colorAdjustments))
            {
                colorAdjustments.active = false;
            }

            if (vignetteBaselineSaved)
            {
                if (volume.profile.TryGet(out vignette))
                {
                    if (vignetteExistedBefore)
                    {
                        vignette.active = vignetteActiveBefore;
                        vignette.intensity.value = vignetteIntensityBefore;
                        vignette.smoothness.value = vignetteSmoothnessBefore;
                    }
                    else
                    {
                        // If vignette was created by us, disable it (and remove if supported)
                        vignette.active = false;
#if UNITY_2022_1_OR_NEWER
                        volume.profile.Remove<Vignette>();
#endif
                    }
                }
            }
        }

        private void EnsureVolumeAndProfile()
        {
            if (volume == null && createVolumeIfMissing)
            {
                volume = GetComponent<Volume>();
                if (volume == null)
                {
                    volume = gameObject.AddComponent<Volume>();
                    volume.isGlobal = true;
                }
            }

            if (volume != null && volume.profile == null)
            {
                volume.profile = ScriptableObject.CreateInstance<VolumeProfile>();
            }
        }
    }
}
