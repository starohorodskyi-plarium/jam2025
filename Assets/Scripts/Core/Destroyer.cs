using UnityEngine;
using UnityEngine.Serialization;

namespace Core
{
    public class Destroyer : MonoBehaviour
    {
        [FormerlySerializedAs("delayBeforeDestroy")]
        [SerializeField]
        [Min(0f)]
        [Tooltip("Delay in seconds before destroying this GameObject.")]
        private float _delayBeforeDestroy = 0f;

        private void OnEnable() => 
            Destroy(gameObject, Mathf.Max(0f, _delayBeforeDestroy));
    }
}


