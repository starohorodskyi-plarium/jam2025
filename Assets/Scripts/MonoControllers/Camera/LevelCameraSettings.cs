using UnityEngine;

namespace MonoControllers.Camera
{
    public class LevelCameraSettings : MonoBehaviour
    {
        [SerializeField] private CameraMovementController _cameraMovementController;
        [SerializeField] private AnimationCurve _screenAspectRatioCompensator;

        private void OnEnable() =>
            _cameraMovementController.SetCompensation(_screenAspectRatioCompensator);
    }
}
