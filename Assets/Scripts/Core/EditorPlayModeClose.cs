using UnityEngine;

public class EditorPlayModeClose : MonoBehaviour
{
    public void StopPlayModeIfActive()
    {
#if UNITY_EDITOR
        if (!UnityEditor.EditorApplication.isPlaying)
        {
            return;
        }

        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

