using UnityEngine;

namespace Core
{
    public class DontDestroyObject : MonoBehaviour
    {
        private void Awake()
        {
            var objs = GameObject.FindGameObjectsWithTag(gameObject.tag);

            if (objs.Length > 1)
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
        }
    }
}
