using System.Collections;
using UnityEngine;

namespace NPC
{
  public class NPCAnimatorController : MonoBehaviour
  {
    [SerializeField] private Animator animator;
    [Space]
    [SerializeField] private float minDelay;
    [SerializeField] private float maxDelay;
  
    private string _triggerIdleKey = "StartIdle";
    private string _idleStateKey = "IsIdle";

    private void Start()
    {
      StartCoroutine(TriggerIdleAfterDelay());
    }

    private IEnumerator TriggerIdleAfterDelay()
    {
      float min = Mathf.Min(minDelay, maxDelay);
      float max = Mathf.Max(minDelay, maxDelay);
      float delay = Mathf.Max(0f, UnityEngine.Random.Range(min, max));
      if (delay > 0f)
        yield return new WaitForSeconds(delay);
      animator.SetTrigger(_triggerIdleKey);
      animator.SetBool(_idleStateKey, true);
    }

    public void StartMoving()
    {
      animator.SetBool(_idleStateKey, false);
    }
  
    public void EndMoving()
    {
      animator.SetBool(_idleStateKey, true);
    }
  }
}
