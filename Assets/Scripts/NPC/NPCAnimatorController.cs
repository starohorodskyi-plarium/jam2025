using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace NPC
{
  public class NPCAnimatorController : MonoBehaviour
  {
    private static readonly int StartIdle = Animator.StringToHash(TriggerIdleKey);
    private static readonly int IsIdle = Animator.StringToHash(IdleStateKey);

    [FormerlySerializedAs("animator")] [SerializeField] private Animator _animator;
    [Space]
    [FormerlySerializedAs("minDelay")] [SerializeField] private float _minDelay;
    [FormerlySerializedAs("maxDelay")] [SerializeField] private float _maxDelay;
  
    private const string TriggerIdleKey = "StartIdle";
    private const string IdleStateKey = "IsIdle";

    private void Start() => 
      StartCoroutine(TriggerIdleAfterDelay());

    private IEnumerator TriggerIdleAfterDelay()
    {
      var min = Mathf.Min(_minDelay, _maxDelay);
      var max = Mathf.Max(_minDelay, _maxDelay);
      var delay = Mathf.Max(0f, Random.Range(min, max));
      
      if (delay > 0f)
        yield return new WaitForSeconds(delay);
      
      _animator.SetTrigger(StartIdle);
      _animator.SetBool(IsIdle, true);
    }

    public void StartMoving() => 
      _animator.SetBool(IsIdle, false);

    public void EndMoving() => 
      _animator.SetBool(IsIdle, true);
  }
}
