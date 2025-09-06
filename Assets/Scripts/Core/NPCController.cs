using System.Collections;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    public enum Faction
    {
        Ally,
        Enemy
    }

    [SerializeField] private Faction faction = Faction.Enemy;
    [SerializeField] private GameObject allyHitEffect;
    [SerializeField] private GameObject enemyHitEffect;
    public void Hit()
    {
        Debug.Log($"{gameObject.name} was hit!");
        GameObject effectPrefab = faction == Faction.Ally ? allyHitEffect : enemyHitEffect;
        if (effectPrefab != null)
        {
            Instantiate(effectPrefab, transform.position, Quaternion.identity);
        }
        StartCoroutine(HitRoutine());

        IEnumerator HitRoutine()
        {
            yield return new WaitForSeconds(0.35f);
            Destroy(gameObject);
        }
    }
}
