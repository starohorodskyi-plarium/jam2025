using System.Collections;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    public void Hit()
    {
        Debug.Log($"{gameObject.name} was hit!");
        StartCoroutine(HitRoutine());

        IEnumerator HitRoutine()
        {
            yield return new WaitForSeconds(0.2f);
            Destroy(gameObject);
        }
    }
}
