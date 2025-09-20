using UnityEngine;

namespace Core
{
    public class Destroyer : MonoBehaviour
    {
        [SerializeField]
        [Min(0f)]
        [Tooltip("Delay in seconds before destroying this GameObject.")]
        private float delayBeforeDestroy = 0f;

        private void OnEnable()
        {
            Destroy(gameObject, Mathf.Max(0f, delayBeforeDestroy));
        }
    }
}


