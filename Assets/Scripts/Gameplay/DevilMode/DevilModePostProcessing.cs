using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

namespace Gameplay.DevilMode
{
    public class DevilModePostProcessing : MonoBehaviour
    {
        [SerializeField] private GameObject _horns;
        [Space] 
        [FormerlySerializedAs("volume")] [SerializeField] private Volume _volume;
        [FormerlySerializedAs("createVolumeIfMissing")] [SerializeField] private bool _createVolumeIfMissing = true;

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

        private void EnableDevilLensDistortion()
        {
            EnsureVolumeAndProfile();
            if (_volume == null || _volume.profile == null)
            {
                Debug.LogWarning("DevilModePostProcessing: Volume or Profile is missing and could not be created.");
                return;
            }

            if (!_volume.profile.TryGet(out lensDistortion)) lensDistortion = _volume.profile.Add<LensDistortion>(true);

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

        private void EnableDevilColorAdjustments()
        {
            EnsureVolumeAndProfile();
            if (_volume == null || _volume.profile == null)
            {
                Debug.LogWarning("DevilModePostProcessing: Volume or Profile is missing and could not be created.");
                return;
            }

            if (!_volume.profile.TryGet(out colorAdjustments)) 
                colorAdjustments = _volume.profile.Add<ColorAdjustments>(true);

            colorAdjustments.active = true;
            colorAdjustments.colorFilter.overrideState = true;
            colorAdjustments.colorFilter.value = new Color(166f / 255f, 0f, 0f, 0f);

            colorAdjustments.contrast.overrideState = true;
            colorAdjustments.contrast.value = -37f;

            colorAdjustments.saturation.overrideState = true;
            colorAdjustments.saturation.value = 26f;
        }

        private void EnableDevilVignette()
        {
            EnsureVolumeAndProfile();
            if (_volume == null || _volume.profile == null)
            {
                Debug.LogWarning("DevilModePostProcessing: Volume or Profile is missing and could not be created.");
                return;
            }

            var existed = _volume.profile.TryGet(out vignette);
            if (!existed) 
                vignette = _volume.profile.Add<Vignette>(true);

            if (!vignetteBaselineSaved)
            {
                vignetteBaselineSaved = true;
                vignetteExistedBefore = existed;
                vignetteActiveBefore = existed && vignette.active;

                vignetteIntensityBefore = vignette.intensity.value;
                vignetteSmoothnessBefore = vignette.smoothness.value;
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
            if (_volume == null || _volume.profile == null)
            {
                Debug.LogWarning("DevilModePostProcessing: Volume or Profile is missing and could not be created.");
                return;
            }

            if (_volume.profile.TryGet(out lensDistortion)) 
                lensDistortion.active = false;

            if (_volume.profile.TryGet(out colorAdjustments)) 
                colorAdjustments.active = false;

            if (!vignetteBaselineSaved) 
                return;
            if (!_volume.profile.TryGet(out vignette)) 
                return;
            
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
                _volume.profile.Remove<Vignette>();
            }
        }

        private void EnsureVolumeAndProfile()
        {
            if (_volume == null && _createVolumeIfMissing)
            {
                _volume = GetComponent<Volume>();
                if (_volume == null)
                {
                    _volume = gameObject.AddComponent<Volume>();
                    _volume.isGlobal = true;
                }
            }

            if (_volume != null && _volume.profile == null) 
                _volume.profile = ScriptableObject.CreateInstance<VolumeProfile>();
        }
    }
}
