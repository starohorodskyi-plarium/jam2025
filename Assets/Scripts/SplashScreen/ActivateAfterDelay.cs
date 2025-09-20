using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace SplashScreen
{
    public class ActivateAfterDelay : MonoBehaviour
    {
        [FormerlySerializedAs("targetToActivate")] [SerializeField] private GameObject _targetToActivate;
        [FormerlySerializedAs("delaySeconds")] [SerializeField, Min(0f)] private float _delaySeconds = 1f;

        private void OnEnable() => 
            StartCoroutine(ActivateRoutine());

        private IEnumerator ActivateRoutine()
        {
            if (_targetToActivate == null)
            {
                Debug.LogWarning("ActivateAfterDelay: No target assigned in the Inspector.", this);
                yield break;
            }

            if (_delaySeconds > 0f)
                yield return new WaitForSeconds(_delaySeconds);

            _targetToActivate.SetActive(true);
        }
    }
}


