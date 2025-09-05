using System.Collections;
using UnityEngine;

public class ActivateAfterDelay : MonoBehaviour
{
    [SerializeField] private GameObject targetToActivate;
    [SerializeField, Min(0f)] private float delaySeconds = 1f;

    private void OnEnable()
    {
        StartCoroutine(ActivateRoutine());
    }

    private IEnumerator ActivateRoutine()
    {
        if (targetToActivate == null)
        {
            Debug.LogWarning("ActivateAfterDelay: No target assigned in the Inspector.", this);
            yield break;
        }

        if (delaySeconds > 0f)
        {
            yield return new WaitForSeconds(delaySeconds);
        }

        targetToActivate.SetActive(true);
    }
}


