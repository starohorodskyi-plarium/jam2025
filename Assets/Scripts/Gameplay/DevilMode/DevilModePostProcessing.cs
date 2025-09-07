using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DevilModePostProcessing : MonoBehaviour
{
    [SerializeField] private Volume volume;
    [SerializeField] private bool createVolumeIfMissing = true;

    private LensDistortion lensDistortion;
    private ChromaticAberration chromaticAberration;
    private ColorAdjustments colorAdjustments;

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
